//----------------------------------------------
// DynamoDB Helper
// Copyright � 2014-2015 OuijaPaw Games LLC
//----------------------------------------------

using UnityEngine;
using System;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using System.Collections;

/*
 * This is a newer example using the in-line class loading.
 * 
 * It builds on the previous examples, so a lot looks similar; however, you'll note that the DynamoDB class is actually a child class
 * of the parent class.  The parent class can save/load the child rather efficiently.  The best part comes from loading it as no
 * longer does there have to be a bunch of data copying / etc.  You just assign the returned instance as your data class.
 * 
 * To setup:
 * Create a table with a hash key and range key.  Indexes don't matter.
 * Edit the DBAccountExample constant strings with the correct table name, hash key, and range key.
 * Make sure the DBUnityHelper has all the correct values set.
 * Feel free to change the column names.  That it.
 * 
 * Run the test method, or build out an monobehavior / gameobject using this for data.
 * 
 * Another thing to note - if the database bucket does not exist, the value will be null / empty / 0
 * on the object after it is loaded.  It is like getting a whole new object.  It seems counter-intuitive
 * at first, because you'd think it would only update the values it finds.
 */
namespace DDBHelper
{
	[Flags]
	public enum ClassType
	{
		None = 0,
		Warrior = 1,
		Rogue = 2,
		Priest = 4,
	}

	/// <summary>
	/// This is an example of a 'character slot' for a player, recording specific information
	/// for future use.
	/// </summary>
	public class PlayerSlot
    {
        public short pID;
        public byte pClass;
        public byte pRace;
        public byte pLevel;
        public uint pFame;

        public PlayerSlot() { }
        public PlayerSlot(short ID, byte _class, byte _race, byte _level, uint _fame)
        {
            pID = ID;
            pClass = _class;
            pRace = _race;
            pLevel = _level;
            pFame = _fame;
        }
    }

    /// <summary>
    /// This is an example class that just holds references to the bucket names in DynamoDB,
    /// and static methods to separate them from the member methods of the PlayerAccount class
    /// 
    /// By defining all these here, it just makes accessing the actual names much easier
    /// </summary>
    public class DBAccountExample : MonoBehaviour
    {
        #region Constants
        // you can move all of these constants to somewhere else.  They're just here for 'show'

        // you'll need to change the table name to whatever you create
        public const string TABLE_NAME = "DDBHelper";

        // if you use a different hash key name, this will need to be changed
        public const string KEY_HASH = "Name";

        // if you use a different range key name, this will need to be changed
        public const string KEY_RANGE = "Type";

        public const string KEY_RANGE_ACCOUNT = "acc";
        public const string KEY_RANGE_PFILE = "plr";

        public const string KEY_ACC_EMAIL = "EM";
        public const string KEY_ACC_PASS = "Pass";
        public const string KEY_ACC_UNLOCKED_CLASSES = "Unl";
        public const string KEY_ACC_FLAGS = "Flg";
        public const string KEY_ACC_CHAR_COUNTER = "Cnt";
        public const string KEY_ACC_CHARSLOTS = "Chrs";
        public const string KEY_ACC_FAME = "Fm";
        public const string KEY_ACC_WEALTH = "Wlth";

		#endregion

		public string PlayerName { get; set; }
        public DBAccountExample.AccountData PlayerAccountData { get; private set; }

        /// <summary>
        /// Initialize the data class
        /// </summary>
        void Awake()
        {
            // before you create your account data, you must give yourself a name.
            PlayerName = "NinjaCat";
        }
        
        void Start()
        {
            // uncomment this if you'd like to test
            StartCoroutine(TestDatabase());
        }

        /// <summary>
        /// This is a simple way of testing the database.
        /// Set to static method so it can be run most anywhere.
        /// This was pre-everything above.
        /// </summary>
        public IEnumerator TestDatabase()
        {
            Debug.Log("<DBAccountExample> PlayerAccount - Initiating Test...");
            PlayerAccountData = new DBAccountExample.AccountData(this);
            PlayerAccountData.Email = "bananaMonkeyPants";
            PlayerAccountData.Password = "123BestPassEver";
            PlayerAccountData.Wealth = 13321;
            PlayerAccountData.Fame = 1290;

			// this particular data just uses an enumeration -- no conversion needed
			PlayerAccountData.PlayerClass = ClassType.Rogue;

			PlayerAccountData.CharIDCounter = 12;
            PlayerAccountData.CharSlots = new List<CharSlot>();
            PlayerAccountData.CharSlots.Add(new CharSlot(2, 1, 1, 2, 0));
            PlayerAccountData.CharSlots.Add(new CharSlot(4, 1, 1, 2, 12000));
            PlayerAccountData.CharSlots.Add(new CharSlot(7, 2, 6, 2, 3000));

            Debug.Log("<DBAccountExample> Starting Save Test for " + PlayerAccountData.Name + ":" + PlayerAccountData.Type);

            // initialize it so you know when save has completed
            PlayerAccountData.IsSaveCompleted = false;

            // Since we're saving within a Coroutine, this will not block the thread while waiting
            // Alternatives are to have the AccountData class send a response to this class via sendmessage and DBUnityHelper
            // so you know it has saved
            PlayerAccountData.Save();

            // Wait for it to save, shouldn't be too long
            while (!PlayerAccountData.IsSaveCompleted)
                yield return new WaitForSeconds(.1f);

            // could load the object directly in the code here...
            LoadAccount();

            // reset loaded status just to be sure we have a clean slate
            PlayerAccountData.IsLoaded = false;

            // wait for the load
            while (!PlayerAccountData.IsLoaded)
                yield return new WaitForSeconds(.1f);

            Debug.Log("<DBAccountExample> Load Completed for " + PlayerAccountData.Name + ":" + PlayerAccountData.Type);

            yield return null;
        }

        /// <summary>
        /// Three step process to get around Unity's Single Threading enforcement
        /// 1. Load the data using new thread to keep main thread (Unity) unblocked
        /// 2. DDBHelper responds on new thread.  New thread sends update to DDBUnityHelper of whether successful or not
        /// 3. DDBUnityHelper via SendMessage sends update to Unity Main Thread
        /// 
        /// From there, validate load data, continue, etc.
        /// </summary>
        /// <returns></returns>
        public void LoadAccount()
        {
            Debug.Log("<DBAccountExample> Starting Load Test for " + PlayerAccountData.Name.ToLower() + ":" + PlayerAccountData.Type);
            DBWorker.Instance.Load<DBAccountExample.AccountData>(PlayerName.ToLower(), PlayerAccountData.Type, LoadAccountResponse, gameObject, "LoadComplete");
        }

        /// <summary>
        /// DDBHelper will use this delegate method as a response - it is on a different thread though!
        /// </summary>
        /// <param name="response"></param>
        /// <param name="owner"></param>
        /// <param name="nextMethod"></param>
        /// <param name="e"></param>
        public void LoadAccountResponse(DBAccountExample.AccountData response, GameObject owner, string nextMethod, Exception e = null)
        {
            bool success = false;

            if (e != null)
            {
                Debug.LogError("<DBAccountExample> Load Exception: msg=" + e.Message);
            }
            else if (response == null)
            {
                Debug.LogError("<DBAccountExample> Could not Load Account Data, " + response.Name + " could not be found");
            }
            else
            {
                PlayerAccountData = response;
                success = true;
            }

            if (!string.IsNullOrEmpty(nextMethod))
                DBUnityHelper.Register(new SendMessageContext(owner, nextMethod, success, SendMessageOptions.DontRequireReceiver));
        }

        /// <summary>
        /// DBUnityHelper has used SendMessage to get us back on the main thread and we can do validation here, etc.
        /// </summary>
        /// <param name="success"></param>
        private void LoadComplete(bool success)
        {
            if (!success)
            {
                Debug.Log("<DBAccountExample> Response is NULL... Load Failure");
            }
            else
            {
                PlayerAccountData.IsLoaded = true;
                Debug.Log("<DBAccountExample> Loaded Successfully n=" + PlayerAccountData.Name + " p=" + PlayerAccountData.Password + " em=" + 
                    PlayerAccountData.Email + " class=" + PlayerAccountData.PlayerClass + " w=" + PlayerAccountData.Wealth + " f=" + PlayerAccountData.Fame);
            }
        }

        /// <summary>
        /// Static method for deleting a character.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="charID"></param>
        /// <param name="nextMethod"></param>
        public static void DeleteChararacter(string name, short charID, NextMethod nextMethod = null)
        {
            name = name.ToLower();
            if (DBUnityHelper.SHOW_DEBUG)
                Debug.Log("DeleteCharacter name=" + name + " charID=" + charID);

            DBObject tObj = new DBObject(name, charID.ToString());
            tObj.PrepareNextMessage(nextMethod);
            DBWorker.Instance.DeleteItem(DBAccountExample.TABLE_NAME, tObj, tObj.DBObject_OnDeleted);
        }

        /// <summary>
        /// Static method for checking if the name exists in the DDB - checks only the name, very efficient
        /// </summary>
        /// <param name="name"></param>
        /// <param name="owner"></param>
        /// <param name="methodName"></param>
        public static void CheckNameInUse(string name, GameObject owner = null, string methodName = null)
        {
            name = name.ToLower();
            if (DBUnityHelper.SHOW_DEBUG)
                Debug.Log("CheckNameInUse name=" + name);

            DBObject tObj = new DBObject(name, DBAccountExample.KEY_RANGE_ACCOUNT);
            tObj.PrepareNextMessage(owner, methodName);

            DBWorker.Instance.Exists(DBAccountExample.TABLE_NAME, tObj, tObj.DBObject_OnExists);
        }

        /// <summary>
        /// Static method for changing the password of a player
        /// </summary>
        /// <param name="name"></param>
        /// <param name="newPassword"></param>
        /// <param name="owner"></param>
        /// <param name="methodName"></param>
        public static void ChangePassword(string name, string newPassword, GameObject owner = null, string methodName = null)
        {
            name = name.ToLower();
            if (DBUnityHelper.SHOW_DEBUG)
                Debug.Log("ChangePassword name=" + name);
            
            DBObject tObj = new DBObject(name, DBAccountExample.KEY_RANGE_ACCOUNT);
            tObj.PrepareUpdateString(DBAccountExample.KEY_ACC_PASS, newPassword);
            tObj.PrepareNextMessage(owner, methodName);
            DBWorker.Instance.UpdateItem(DBAccountExample.TABLE_NAME, tObj, tObj.DBObject_OnUpdated);
        }

		/// <summary>   
		/// This is an example of a player account that would be saved/loaded into DynamoDB
		/// This is the same code I use for my game.
		/// </summary>
		[DynamoDBTable(DBAccountExample.TABLE_NAME)]  // <---------------- UPDATE ME AS NEEDED!!!!!!!!!!!!!!!!!
        public class AccountData
        {
			// PrimaryKey (PK) mapping -- required!
			[DynamoDBHashKey(DBAccountExample.KEY_HASH)]
            public string Name { get; set; }

            [DynamoDBRangeKey(DBAccountExample.KEY_RANGE)]
            public string Type = DBAccountExample.KEY_RANGE_ACCOUNT;

            [DynamoDBProperty(DBAccountExample.KEY_ACC_EMAIL)]
            public string Email { get; set; }

            [DynamoDBProperty(DBAccountExample.KEY_ACC_PASS)]
            public string Password { get; set; }

            [DynamoDBProperty(DBAccountExample.KEY_ACC_FLAGS)]
            public uint AccountFlags { get; set; }

            [DynamoDBProperty(DBAccountExample.KEY_ACC_UNLOCKED_CLASSES)]
            public int UnlockedClasses { get; set; }

            [DynamoDBProperty(DBAccountExample.KEY_ACC_WEALTH)]
            public uint Wealth { get; set; }

            [DynamoDBProperty(DBAccountExample.KEY_ACC_FAME)]
            public uint Fame { get; set; }

            [DynamoDBProperty(DBAccountExample.KEY_ACC_CHAR_COUNTER)]
            public short CharIDCounter { get; set; }

            [DynamoDBProperty(DBAccountExample.KEY_ACC_CHARSLOTS, typeof(CharSlotsConverter))]
            public List<CharSlot> CharSlots { get; set; }

            [DynamoDBIgnore]
            private DBAccountExample AccountOwner { get; set; }

            [DynamoDBIgnore]
            public bool IsSaveCompleted { get; set; }

            [DynamoDBIgnore]
            public bool IsLoaded { get; set; }

			// this particular data just uses an enumeration -- no conversion needed
			public ClassType PlayerClass { get; set; }

			// While this is available, I don't particularly like it... especially since Unity is single-threaded anyhow
			//[DynamoDBVersion]
			//public int? version { get; set; }

			/// <summary>
			/// Base Constructor, should not be used directly
			/// </summary>
			public AccountData()
            {
                // Assign the name if able, else you'll have to assign is explicitely later
                Email = "";
                Password = "";
                AccountFlags = 0;
                UnlockedClasses = 0;
                Wealth = 0;
                Fame = 0;
                CharIDCounter = 0;
                IsLoaded = false;
                IsSaveCompleted = false;
				PlayerClass = ClassType.None;
				InitCharSlots();
            }

            /// <summary>
            /// Constructor assigning the owner object, which is necessary
            /// Once set, calls consructor with no args
            /// </summary>
            /// <param name="newOwner"></param>
            public AccountData(DBAccountExample newOwner) : this()
            {
                AccountOwner = newOwner;

                // very important to make the name lower case so all checks later match!  -well, just be aware case matters and handle it accordingly
                Name = AccountOwner.PlayerName.ToLower();
            }

            /// <summary>
            /// Initializes character slots into a string
            /// -1 means it is empty/unused
            /// </summary>
            public void InitCharSlots()
            {
                if (CharSlots == null)
                    CharSlots = new List<CharSlot>();
                else
                    CharSlots.Clear();
                CharSlots.Add(new CharSlot(-1, 0, 0, 0, 0));
            }

            /// <summary>
            /// Removes a character slot and ensures there is always an empty slot
            /// </summary>
            /// <param name="charID"></param>
            public void RemoveCharSlot(short charID)
            {
                CharSlot c = CharSlots.Find(a => a.pID == charID);
                if (c != null)
                    CharSlots.Remove(c);
                if (CharSlots.Count == 0)
                    CharSlots.Add(new CharSlot(-1, 0, 0, 0, 0));
            }

            /// <summary>
            /// Saves account to DynamoDB
            /// </summary>
            /// <param name="owner"></param>
            /// <param name="name"></param>
            /// <param name="methodName"></param>
            public void Save(string methodName = null)
            {
                DBWorker.Instance.Save(this, SaveResponse, AccountOwner.gameObject, methodName);
            }

            /// <summary>
            /// Response to Save, will send message to Unity main thread if GameObject and methodname are provided
            /// </summary>
            /// <param name="response"></param>
            /// <param name="owner"></param>
            /// <param name="methodName"></param>
            /// <param name="e"></param>
            void SaveResponse(bool success, GameObject owner, string methodName, Exception e = null)
            {
                if (e != null)
                    DBTools.PrintException("Save", e);

                Debug.Log("PlayerAccount SaveResponse result=" + success);
                
                IsSaveCompleted = true;

                if (!string.IsNullOrEmpty(methodName))
                    DBUnityHelper.Register(new SendMessageContext(owner, methodName, success, SendMessageOptions.RequireReceiver));
            }

            /// <summary>
            /// Saves character slots to DynamoDB 
            /// </summary>
            /// <param name="nextMethod"></param>
            public void SaveCharSlots(NextMethod nextMethod = null)
            {
                DBObject tObj = new DBObject(Name, DBAccountExample.KEY_RANGE_ACCOUNT);
                tObj.PrepareUpdateString(DBAccountExample.KEY_ACC_CHARSLOTS, CreateCharSlotsString(), true);
                tObj.PrepareNextMessage(nextMethod);

                DBWorker.Instance.UpdateItem(DBAccountExample.TABLE_NAME, tObj, tObj.DBObject_OnUpdated);
            }

            /// <summary>
            /// Lets you save the character slots and the current character ID counter at the same time,
            /// Used after creating a new character so the ID counter that was incremented is saved as well.
            /// Shows an example of how the last parameter of the 'PrepareUpdateX' methods will clear out
            /// any previous values (true) or not (false)
            /// </summary>
            /// <param name="nextMethod"></param>
            public void SaveCharSlotsAndCounter(NextMethod nextMethod = null)
            {
                DBObject tObj = new DBObject(Name, DBAccountExample.KEY_RANGE_ACCOUNT);
                tObj.PrepareUpdateString(DBAccountExample.KEY_ACC_CHARSLOTS, CreateCharSlotsString(), true);
                tObj.PrepareUpdateNumber(DBAccountExample.KEY_ACC_CHAR_COUNTER, CharIDCounter, false);
                tObj.PrepareNextMessage(nextMethod);

                DBWorker.Instance.UpdateItem(DBAccountExample.TABLE_NAME, tObj, tObj.DBObject_OnUpdated);
            }

            /// <summary>
            /// Saves character slots and wealth, used after buying a new character slot
            /// </summary>
            /// <param name="nextMethod"></param>
            public void SaveCharSlotsAndWealth(NextMethod nextMethod = null)
            {
                DBObject tObj = new DBObject(Name, DBAccountExample.KEY_RANGE_ACCOUNT);
                tObj.PrepareUpdateString(DBAccountExample.KEY_ACC_CHARSLOTS, CreateCharSlotsString(), true);
                tObj.PrepareUpdateNumber(DBAccountExample.KEY_ACC_WEALTH, Wealth, false);
                tObj.PrepareNextMessage(nextMethod);

                DBWorker.Instance.UpdateItem(DBAccountExample.TABLE_NAME, tObj, tObj.DBObject_OnUpdated);
            }

            /// <summary>
            /// Centralized spot to create the character slot string for saving.
            /// </summary>
            /// <returns></returns>
            public string CreateCharSlotsString()
            {
                CharSlot c;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                int len = CharSlots.Count;
                for (int i = 0; i < len; i++)
                {
                    c = CharSlots[i];
                    sb.Append(c.pID).Append("|").Append(c.pClass).Append("|").Append(c.pRace).Append("|").Append(c.pLevel).Append("|").Append(c.pFame);
                    if (i < len - 1)
                        sb.Append("&");
                }
                c = null;
                return sb.ToString();
            }
        }



        /// <summary>
        /// Converts the complex type DimensionType to string and vice-versa
        /// You still have to use the defined types of string, number, or binary and handle it in some fashion
        /// It is much more limited than the low-level API calls, unless you're a code genius
        /// </summary>
        public class CharSlotConverter : IPropertyConverter
        {
            private const int DATA_LENGTH = 5;
            /// <summary>
            /// Converts complex data type into a string for DynamoDB
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public DynamoDBEntry ToEntry(object value)
            {
                DynamoDBEntry entry = null;
                try
                {
                    CharSlot type = value as CharSlot;
                    if (type == null)
                        throw new ArgumentOutOfRangeException();

                    // saving 5 variables with string delimiter
                    string data = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}", "|", type.pID, type.pClass, type.pRace, type.pLevel, type.pFame);

                    entry = new Primitive { Value = data };
                }
                catch (Exception e)
                {
                    Debug.Log("CharSlotConverter Exception  msg=" + e.Message);
                }
                return entry;
            }

            /// <summary>
            /// Converts string from DynamoDB and expands it into the complex data type
            /// </summary>
            /// <param name="entry"></param>
            /// <returns></returns>
            public object FromEntry(DynamoDBEntry entry)
            {
                CharSlot cslot = null;
                try
                {
                    Primitive primitive = entry as Primitive;
                    if (primitive == null || !(primitive.Value is String) || string.IsNullOrEmpty((string)primitive.Value))
                        throw new ArgumentOutOfRangeException();

                    // reading 3 variables using string delimiter
                    string[] data = ((string)(primitive.Value)).Split(new string[] { "|" }, StringSplitOptions.None);
                    if (data.Length != DATA_LENGTH)
                        throw new ArgumentOutOfRangeException();

                    cslot = new CharSlot
                    {
                        pID = Convert.ToInt16(data[0]),
                        pClass = Convert.ToByte(data[1]),
                        pRace = Convert.ToByte(data[2]),
                        pLevel = Convert.ToByte(data[3]),
                        pFame = Convert.ToUInt32(data[4])
                    };
                }
                catch (Exception e)
                {
                    Debug.Log("CharSlotConverter Exception  msg=" + e.Message);
                }
                return cslot;
            }
        }

        /// <summary>
        /// Converts the complex type DimensionType to string and vice-versa
        /// You still have to use the defined types of string, number, or binary and handle it in some fashion
        /// It is much more limited than the low-level API calls, unless you're a code genius
        /// </summary>
        public class CharSlotsConverter : IPropertyConverter
        {
            public static char[] CharSlotDelimiter = new char[] { '&' };
            public static char[] CharDataDelimiter = new char[] { '|' };
            public string[] mSlots;
            public string[] mData;

            /// <summary>
            /// Converts complex data type into a string for DynamoDB
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public DynamoDBEntry ToEntry(object value)
            {
                DynamoDBEntry entry = null;
                try
                {
                    List<CharSlot> type = value as List<CharSlot>;
                    CharSlot c;
                    if (type == null)
                        throw new ArgumentOutOfRangeException();

                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    for (int i = 0; i < type.Count; i++)
                    {
                        c = type[i];
                        sb.Append(c.pID).Append("|").Append(c.pClass).Append("|").Append(c.pRace).Append("|").Append(c.pLevel).Append("|").Append(c.pFame);
                        if (i < type.Count - 1)
                            sb.Append("&");
                    }
                    c = null;
                    entry = new Primitive(sb.ToString());
                }
                catch (Exception e)
                {
                    Debug.Log("CharSlotsConverter:" + e.Message);
                }
                return entry;
            }

            /// <summary>
            /// Converts string from DynamoDB and expands it into the complex data type
            /// </summary>
            /// <param name="entry"></param>
            /// <returns></returns>
            public object FromEntry(DynamoDBEntry entry)
            {
                List<CharSlot> charSlots = new List<CharSlot>();

                try
                {
                    Primitive primitive = entry as Primitive;
                    if (primitive == null || !(primitive.Value is String) || string.IsNullOrEmpty((string)primitive.Value))
                        throw new ArgumentOutOfRangeException();

                    // reading 3 variables using string delimiter
                    mSlots = ((string)(primitive.Value)).Split(CharSlotDelimiter, StringSplitOptions.None);

                    for (int i = 0; i < mSlots.Length; i++)
                    {
                        mData = mSlots[i].Split(CharDataDelimiter, StringSplitOptions.None);

                        charSlots.Add(new CharSlot
                        {
                            pID = Convert.ToInt16(mData[0]),
                            pClass = Convert.ToByte(mData[1]),
                            pRace = Convert.ToByte(mData[2]),
                            pLevel = Convert.ToByte(mData[3]),
                            pFame = Convert.ToUInt32(mData[4])
                        });
                    }
                }
                catch (Exception e)
                {
                    Debug.Log("CharSlotsConverter:" + e.Message);
                }
                return charSlots;
            }
        }

    }

}
