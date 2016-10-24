//----------------------------------------------
// DynamoDB Helper
// Copyright © 2014-2015 OuijaPaw Games LLC
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;

/*
 * http://docs.aws.amazon.com/amazondynamodb/latest/developerguide/DotNetSDKHighLevel.html
 * 
 * Here is a class that does not actually need to use the low-level API calls
 * You define the persistence model mapping and then you can save/load the whole class in a single call
 * 
 * There is a 64 kb limit for a single read/write
 * 
 * Any arbitrary types can be used, but you will have to write your own converter
 * 
 * Additionally, as per: http://docs.aws.amazon.com/amazondynamodb/latest/APIReference/API_PutItem.html
 * "When you add an item, the primary key attribute(s) are the only required attributes. 
 * >>>>>>>> Attribute values cannot be null. <<<<<<<<<<<<
 * -- String and binary type attributes must have lengths greater than zero. 
 * -- Set type attributes cannot be empty. 
 * -- Requests with empty values will be rejected with a ValidationException.
 * You can request that PutItem return either a copy of the old item (before the update) or a copy of the new item (after the update). 
 * For more information, see the ReturnValues description.
 * 
 * Note: To prevent a new item from replacing an existing item, use a conditional put operation with 
 * Exists set to false for the primary key attribute, or attributes"
 * 
 * This means, in a nutshell, that no null values, empty strings, or empty sets are allowed.
 * If you make something as a DynamoDB Attribute, you _must_ set it to something
 * 
 * Lastly... using this method you cannot delete data easily.  
 * If you rename an item for the DDB, the old item will stay there indefinitely.
 * You will need to explicitely request to delete items from a bucket
 * 
 */

namespace DDBHelper
{
    /// <summary>
    /// Example of complex data type
    /// </summary>
    public class ComplexType
    {
        public string name;
        public int x;
        public int y;
    }

    /// <summary>
    // DynamoDB table context must be a const, or edited in the actual file.  As such, edit this
    // here, or just edit the table name directly.  DBExample shows where the table name is referenced.
    /// </summary>
    [DynamoDBTable("DDBHelper")] // <---------------- UPDATE ME AS NEEDED!!!!!!!!!!!!!!!!!
    public class DBExample
    {
        // PrimaryKey (PK) mapping -- required!
        [DynamoDBHashKey]
        public string Name { get; set; }

        // RangeKey only used if your DynamoDB has a range key
        [DynamoDBRangeKey]
        public string Type { get; set; }

        // Implicit mapping - uses name of variable, has ups and downs
        public string Email { get; set; }
        public string Pass { get; set; }
        public bool Reg { get; set; }
        public uint Wealth { get; set; }
        public uint Fame { get; set; }
        public int Unlk { get; set; }

        //public List<int> ListTest = new List<int>(20);

        // Ignore variables and do not map them
        [DynamoDBIgnore]
        public string TempInfo { get; set; }

        // Explicit mapping - property and table attribute names are different
        [DynamoDBProperty("Tags")]
        public List<string> KeywordTags { get; set; }

        // Complex data type, must define your own converter, such as:
        //[DynamoDBProperty(typeof(DataConverters.ComplexDataConverter))]
        // Can also define converter and give explicit naming, such as:
        [DynamoDBProperty("TData", typeof(DBDataConverter.ComplexDataConverter))]
        public ComplexType testData { get; set; }

        // This is for optimistic locking, it must be a nullable type
        // Note: there is a very good chance you'll get an AmazonDynamoDBException: The conditional request failed
        // when using this, you are ensuring you're working with the most recent set of data.  You don't need to handle
        // incrementing this; however, you must read data before you save it else it will think it's out of sync.
        // http://docs.aws.amazon.com/amazondynamodb/latest/developerguide/VersionSupportHLAPI.html
        // Be aware that this is a nullable primitive.  If you know what you're doing and want to work with versioning,
        // uncomment, make sure the version is the same as the database object, and you can save.  If the versions
        // are not the same, it will fail.
        // Ideal use is:  load, get version, save + update version, which prevents stale data should someone else have
        // loaded it from being able to save/overwrite your data.
        //[DynamoDBVersion]
        //public int? Version { get; set; }

        public event DDBLoadObjResponseDelegate<DBExample> OnLoaded;
        public event DDBCompletionResponseDelegate OnSaved;
        public event DDBCompletionResponseDelegate OnDeleted;

        /// <summary>
        /// Constructor adding the event callbacks
        /// </summary>
        public DBExample()
        {
            OnLoaded += DBExample_OnLoaded;
            OnSaved += DBExample_OnSaved;
            OnDeleted += DBExample_OnDeleted;
        }

        /// <summary>
        /// Destructor to make sure the events have been removed
        /// </summary>
        ~DBExample()
        {
            OnLoaded -= DBExample_OnLoaded;
            OnSaved -= DBExample_OnSaved;
            OnDeleted -= DBExample_OnDeleted;
        }

        /// <summary>
        /// test method
        /// </summary>
        public static void TestSaveAndLoad()
        {
            Debug.Log("TestSaveAndLoad starting...");

            // cannot stress this enough, all values selected for DDB MUST NOT BE NULL OR EMPTY STRINGS
            DBExample testObj = new DBExample();

            testObj.Name = "Alex";
            testObj.Type = "Noodle";
            testObj.Email = "alexninja@gmail.com";
            testObj.Pass = "password";
            testObj.Reg = false;
            testObj.Unlk = 0x8;

            //testObj.ListTest = new List<int>(20);
            //for (int i = 0; i < 20; i++)
            //    testObj.ListTest.Add(UnityEngine.Random.Range(0, 100));

            // we can set this to an empty string because it is being ignored for DynamoDB as declared above
            testObj.TempInfo = "";

            // all collections must have unique values, as they are 'sets' in DDB world
            testObj.KeywordTags = new List<string>();
            testObj.KeywordTags.Add("test");
            testObj.KeywordTags.Add("test2");
            testObj.KeywordTags.Add("test3");
            testObj.KeywordTags.Add("test4");
            testObj.KeywordTags.Add("test5");

            // complex data needs to be set as well
            testObj.testData = new ComplexType();
            testObj.testData.name = "Namio";
            testObj.testData.x = 12;
            testObj.testData.y = 134;

            // uncomment ONLY if you're using versions
            //testObj.Version = 2;
            testObj.Save();

            System.Threading.Thread.Sleep(500);

            // we need the key (hash+range, or just hash) in order to actually load an object from the database
            DBExample example = new DBExample();
            example.Name = "Alex";
            example.Type = "Noodle";

            //// delegate will be the callback "response"
            DBWorker.Instance.Load(example, DBExample_OnLoaded);
        }

        /// <summary>
        /// Generic load method
        /// </summary>
        public void Load()
        {
            // you could use this without the delegate, but then you would lose the reference to the response object
            DBWorker.Instance.Load(this, DBExample_OnLoaded);
        }

        /// <summary>
        /// Generic save method, no response as to whether it worked or not, or when it completed
        /// </summary>
        public void SaveWithNoResponse()
        {
            // save with no confirmation or exception check
            DBWorker.Instance.Save(this, null);
        }

        /// <summary>
        /// Generic save method, delegate method allows indication of when it completed
        /// </summary>
        public void Save()
        {
            // delegate will be the callback "response"
            DBWorker.Instance.Save(this, DBExample_OnSaved);
        }

        /// <summary>
        /// More complex save method... where you're passing a delegate method that will be
        /// called once the save is completed.
        /// Note we're using the delegate we've defined above, but could be from anywhere actually
        /// </summary>
        /// <param name="responseMethod"></param>
        public void SaveWithCallbackMethod(DDBCompletionResponseDelegate responseMethod)
        {
            DBWorker.Instance.Save(this, responseMethod);
        }

        /// <summary>
        /// Generic delete method
        /// </summary>
        public void Delete()
        {
            // deletion with no confirmation
            DBWorker.Instance.Delete(this, null);

            // alternatively, you can get confirmation of deletion
            // delegate will be the callback "response"
            DBWorker.Instance.Delete(this, DBExample_OnDeleted);
        }


        #region Event Delegate Reponses

        /// <summary>
        /// Event for Load response
        /// </summary>
        /// <param name="response"></param>
        /// <param name="e"></param>
        static void DBExample_OnLoaded(DBExample response, GameObject obj, string nextMethod, Exception e = null)
        {
            if (response != null)
            {
                Debug.Log("LoadResponse Result -- Name=" + response.Name
                    + " Type=" + response.Type
                    + " Pass=" + response.Pass
                    + " Email=" + response.Email
                    + " KeywordTags.Length=" + response.KeywordTags.Count
                    + " ComplexData Name=" + response.testData.name
                    + " x=" + response.testData.x
                    + " y=" + response.testData.y);

                //if (response.ListTest == null)
                //    Debug.Log("ListTest: response.ListTest is NULL");
                //else
                //{
                //    for (int i = 0; i < response.ListTest.Count; i++)
                //        Debug.Log("ListTest: response.ListTest[" + i + "] = " + response.ListTest[i);
                //}
            }

            if (e != null)
                DBTools.PrintException("DBExample Load", e);
        }

        /// <summary>
        /// Event for Saved response
        /// </summary>
        /// <param name="success"></param>
        /// <param name="e"></param>
        static void DBExample_OnSaved(bool success, GameObject obj, string nextMethod, Exception e = null)
        {
            Debug.Log("Save Result =" + success);

            if (e != null)
                DBTools.PrintException("DBExample Save", e);
        }

        /// <summary>
        /// Event for Deleted response
        /// </summary>
        /// <param name="success"></param>
        /// <param name="e"></param>
        static void DBExample_OnDeleted(bool success, GameObject obj, string nextMethod, Exception e = null)
        {
            Debug.Log("Save Result =" + success);

            if (e != null)
                DBTools.PrintException("DBExample Save", e);
        }

        #endregion
    }
}

