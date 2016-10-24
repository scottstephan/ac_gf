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
 * This is a much larger example of just how things could or would work.
 * 
 * There is a complex data type - CharSlot class
 * A player's account can have multiple CharSlots in addition to a bunch of other data
 * 
 * This shows you how to have it save, load, delete, etc.
 * 
 * A thing to note, DynamoDB is case sensitive, as you'd expect.  As such, all of the DBObject
 * methods, and intermediary methods below, make the name key a toLower() string.  This is because
 * I liked the idea that everything saved lower case, so a player, if they wanted to, could login with
 * any type of UpPeR or lOWeR case name variant they wanted.  Also, this prevents other players from
 * trying to get a similar name with just different case variant.
 * 
 * To setup:
 * Create a table with a hash key and range key.  Indexes don't matter.
 * Edit the PlayerData constant strings with the correct table name, hash key, and range key.
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
    /// <summary>
    /// This is an example of a 'character slot' for a player, recording specific information
    /// for future use.
    /// </summary>
    public class CharSlot
    {
        public short pID;
        public byte pClass;
        public byte pRace;
        public byte pLevel;
        public uint pFame;

        public CharSlot() { }
        public CharSlot(short ID, byte _class, byte _race, byte _level, uint _fame)
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
    public class PlayerData
    {
        // you'll need to change the table name to whatever you create
        public const string TABLE_NAME = "DDBHelper";

        // if you use a different hash key name, this will need to be changed
        public const string KEY_HASH = "Name";

        // if you use a different range key name, this will need to be changed
        public const string KEY_RANGE = "Type";

        public const string KEY_RANGE_ACCOUNT = "acc";
        public const string KEY_RANGE_PFILE = "plr";
        public const string KEY_RANGE_VAULT = "vlt";

        public const string KEY_ACC_EMAIL = "EM";
        public const string KEY_ACC_PASS = "Pass";
        public const string KEY_ACC_UNLOCKED_CLASSES = "Unl";
        public const string KEY_ACC_FLAGS = "Flg";
        public const string KEY_ACC_CHAR_COUNTER = "Cnt";
        public const string KEY_ACC_CHARSLOTS = "Chrs";
        public const string KEY_ACC_FAME = "Fm";
        public const string KEY_ACC_WEALTH = "Wlth";

        /// <summary>
        /// Creates the vault data in the database here, rather than trying to determine the first
        /// time someone enters the vault area
        /// index 2 is the center vault
        /// </summary>
        /// <param name="name"></param>
        public static void CreateNewPlayerVault(string name)
        {
            name = name.ToLower();
            if (DBUnityHelper.SHOW_DEBUG)
                Debug.Log("CreateNewPlayerVault name=" + name);

            DBObject tObj = new DBObject(name, PlayerData.KEY_RANGE_VAULT);
            tObj.PrepareUpdateString("2", "0", true);
            DBWorker.Instance.UpdateItem(PlayerData.TABLE_NAME, tObj, tObj.DBObject_OnUpdated);
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
            DBWorker.Instance.DeleteItem(PlayerData.TABLE_NAME, tObj, tObj.DBObject_OnDeleted);
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

            DBObject tObj = new DBObject(name, PlayerData.KEY_RANGE_ACCOUNT);
            tObj.PrepareNextMessage(owner, methodName);

            DBWorker.Instance.Exists(PlayerData.TABLE_NAME, tObj, tObj.DBObject_OnExists);
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

            DBObject tObj = new DBObject(name, PlayerData.KEY_RANGE_ACCOUNT);
            tObj.PrepareUpdateString(PlayerData.KEY_ACC_PASS, newPassword);
            tObj.PrepareNextMessage(owner, methodName);
            DBWorker.Instance.UpdateItem(PlayerData.TABLE_NAME, tObj, tObj.DBObject_OnUpdated);
        }
    }

    /// <summary>
    /// This is an example of a player account that would be saved/loaded into DynamoDB
    /// This is the same code I use for my game.
    /// </summary>
    [DynamoDBTable(PlayerData.TABLE_NAME)]  // <---------------- UPDATE ME AS NEEDED!!!!!!!!!!!!!!!!!
    public class PlayerAccount
    {
        // PrimaryKey (PK) mapping -- required!
        [DynamoDBHashKey(PlayerData.KEY_HASH)]
        public string mName { get; set; }

        [DynamoDBRangeKey(PlayerData.KEY_RANGE)]
        public string mType = PlayerData.KEY_RANGE_ACCOUNT;

        [DynamoDBProperty(PlayerData.KEY_ACC_EMAIL)]
        public string mEmail { get; set; }

        [DynamoDBProperty(PlayerData.KEY_ACC_PASS)]
        public string mPassword { get; set; }

        [DynamoDBProperty(PlayerData.KEY_ACC_FLAGS)]
        public uint mAccFlags { get; set; }

        [DynamoDBProperty(PlayerData.KEY_ACC_UNLOCKED_CLASSES)]
        public int mUnlockedClasses { get; set; }

        [DynamoDBProperty(PlayerData.KEY_ACC_WEALTH)]
        public uint mWealth { get; set; }

        [DynamoDBProperty(PlayerData.KEY_ACC_FAME)]
        public uint mFame { get; set; }

        [DynamoDBProperty(PlayerData.KEY_ACC_CHAR_COUNTER)]
        public short mCharIDCounter { get; set; }

        [DynamoDBProperty(PlayerData.KEY_ACC_CHARSLOTS, typeof(CharSlotsConverter))]
        public List<CharSlot> mCharSlots { get; set; }

        [DynamoDBIgnore]
        GameObject mPlayer { get; set; }

        [DynamoDBIgnore]
        bool mSaveCompleted { get; set; }

        // While this is available, I don't particularly like it... especially since Unity is single-threaded anyhow
        //[DynamoDBVersion]
        //public int? version { get; set; }

        /// <summary>
        /// Ideally we do not want to use a GameObject here.
        /// Use your player monobehaviour or whatever is the 'player' that identifies them
        /// This way the data object (this) has a reference back to the owner if necessary
        /// On the flip side, it also means this (the data object) can be independent of the player
        /// if necessary (like they try to drop connection moment they die, etc.)
        /// </summary>
        /// <param name="p"></param>
        public void Init(GameObject p)
        {
            //Debug.Log("PlayerData Init()");
            mPlayer = p;
            mName = "";
            mEmail = "";
            mPassword = "";
            mAccFlags = 0x0;
            mUnlockedClasses = 0x0;
            mWealth = 0;
            mFame = 0;
            mCharIDCounter = 0;
            InitCharSlots();
        }

        /// <summary>
        /// Initializes character slots into a string
        /// -1 means it is empty/unused
        /// </summary>
        public void InitCharSlots()
        {
            if (mCharSlots == null)
                mCharSlots = new List<CharSlot>();
            else
                mCharSlots.Clear();
            mCharSlots.Add(new CharSlot(-1, 0, 0, 0, 0));
        }

        /// <summary>
        /// Removes a character slot and ensures there is always an empty slot
        /// </summary>
        /// <param name="charID"></param>
        public void RemoveCharSlot(short charID)
        {
            CharSlot c = mCharSlots.Find(a => a.pID == charID);
            if (c != null)
                mCharSlots.Remove(c);
            if (mCharSlots.Count == 0)
                mCharSlots.Add(new CharSlot(-1, 0, 0, 0, 0));
        }

        /// <summary>
        /// Loads the player's account from DynamoDB
        /// The delegate method being passed as the parameter is the important part of handling this in an elegant way
        /// Once the data has been loaded, that method will be called, such as moving to the next step in a login process
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <param name="methodName"></param>
        public void Load(GameObject owner, string name, string methodName)
        {
            this.mName = name.ToLower();
            DBWorker.Instance.Load(this, LoadResponse, owner, methodName);
        }


        /// <summary>
        /// This is another Load method, but it does not have the GameObject tied to it.
        /// Since this result is returned in a separate thread from Unity, you cannot actually
        /// check whether a GameObject is null (that I'm aware) and therefore, for the test below
        /// which is not done from a monobehavior/unity, this Load method had to be created without
        /// the GameObject references.  Since there is no GameObject, there is no delegate method to
        /// call once it completes either.
        /// </summary>
        /// <param name="name"></param>
        public void Load(string name)
        {
            this.mName = name.ToLower();
            try
            {
                DBWorker.Instance.Load(this, LoadResponse, null, null);
            }
            catch (System.Exception e)
            {
                Debug.LogError("PlayerAccount - Load msg=" + e.Message);
            }
        }
        
        /// <summary>
        /// Response to load, will send message to Unity main thread if GameObject and methodname are provided
        /// </summary>
        /// <param name="response"></param>
        /// <param name="owner"></param>
        /// <param name="methodName"></param>
        /// <param name="e"></param>
        void LoadResponse(PlayerAccount response, GameObject owner, string methodName, Exception e = null)
        {
            if (e != null)
                DBTools.PrintException("Load", e);

            if (response == null)
            {
                if (!string.IsNullOrEmpty(methodName))
                    DBUnityHelper.Register(new SendMessageContext(owner, methodName, false, SendMessageOptions.RequireReceiver));
            }
            else
            {
                Debug.Log("PlayerAccount " + mName + " Loaded Successfully");
                //this.mName = name;  // not necessary
                this.mEmail = response.mEmail;
                this.mPassword = response.mPassword;
                this.mUnlockedClasses = response.mUnlockedClasses;
                this.mAccFlags = response.mAccFlags;
                this.mWealth = response.mWealth;
                this.mFame = response.mFame;
                this.mCharIDCounter = response.mCharIDCounter;
                this.mCharSlots = response.mCharSlots;

                //if (mCharSlots == null)
                //    InitCharSlots();
                if (!string.IsNullOrEmpty(methodName))
                    DBUnityHelper.Register(new SendMessageContext(owner, methodName, true, SendMessageOptions.RequireReceiver));
            }
        }

        /// <summary>
        /// Saves account to DynamoDB
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <param name="methodName"></param>
        public void Save(GameObject owner, string name, string methodName = null)
        {
            this.mName = name.ToLower();
            DBWorker.Instance.Save(this, SaveResponse, null, null);
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
            if (!string.IsNullOrEmpty(methodName))
                DBUnityHelper.Register(new SendMessageContext(owner, methodName, success, SendMessageOptions.RequireReceiver));
        }
        
        /// <summary>
        /// Saves character slots to DynamoDB 
        /// </summary>
        /// <param name="nextMethod"></param>
        public void SaveCharSlots(NextMethod nextMethod = null)
        {
            DBObject tObj = new DBObject(mName, PlayerData.KEY_RANGE_ACCOUNT);
            tObj.PrepareUpdateString(PlayerData.KEY_ACC_CHARSLOTS, CreateCharSlotsString(), true);
            tObj.PrepareNextMessage(nextMethod);

            DBWorker.Instance.UpdateItem(PlayerData.TABLE_NAME, tObj, tObj.DBObject_OnUpdated);
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
            DBObject tObj = new DBObject(mName, PlayerData.KEY_RANGE_ACCOUNT);
            tObj.PrepareUpdateString(PlayerData.KEY_ACC_CHARSLOTS, CreateCharSlotsString(), true);
            tObj.PrepareUpdateNumber(PlayerData.KEY_ACC_CHAR_COUNTER, mCharIDCounter, false);
            tObj.PrepareNextMessage(nextMethod);

            DBWorker.Instance.UpdateItem(PlayerData.TABLE_NAME, tObj, tObj.DBObject_OnUpdated);
        }

        /// <summary>
        /// Saves character slots and wealth, used after buying a new character slot
        /// </summary>
        /// <param name="nextMethod"></param>
        public void SaveCharSlotsAndWealth(NextMethod nextMethod = null)
        {
            DBObject tObj = new DBObject(mName, PlayerData.KEY_RANGE_ACCOUNT);
            tObj.PrepareUpdateString(PlayerData.KEY_ACC_CHARSLOTS, CreateCharSlotsString(), true);
            tObj.PrepareUpdateNumber(PlayerData.KEY_ACC_WEALTH, mWealth, false);
            tObj.PrepareNextMessage(nextMethod);

            DBWorker.Instance.UpdateItem(PlayerData.TABLE_NAME, tObj, tObj.DBObject_OnUpdated);
        }

        /// <summary>
        /// Centralized spot to create the character slot string for saving.
        /// </summary>
        /// <returns></returns>
        public string CreateCharSlotsString()
        {
            CharSlot c;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            int len = mCharSlots.Count;
            for (int i = 0; i < len; i++)
            {
                c = mCharSlots[i];
                sb.Append(c.pID).Append("|").Append(c.pClass).Append("|").Append(c.pRace).Append("|").Append(c.pLevel).Append("|").Append(c.pFame);
                if (i < len - 1)
                    sb.Append("&");
            }
            c = null;
            return sb.ToString();
        }

        /// <summary>
        /// This is a simple way of testing the database.
        /// Set to static method so it can be run most anywhere.
        /// This was pre-everything above.
        /// </summary>
        public static IEnumerator TestDatabase()
        {
            Debug.Log("PlayerAccount - Initiating DynamoDB Test...");
            PlayerAccount p = new PlayerAccount();
            p.mName = "Yuki";
            p.mType = "Test";
            p.mEmail = "alexninja@gmail.com";
            p.mPassword = "passwordYo"; 
            p.mWealth = 2222;
            p.mFame = 3456;
            p.mCharIDCounter = 12;
            p.mCharSlots = new List<CharSlot>();
            p.mCharSlots.Add(new CharSlot(2, 1, 1, 2, 0));
            p.mCharSlots.Add(new CharSlot(4, 1, 1, 2, 12000));
            p.mCharSlots.Add(new CharSlot(7, 2, 6, 2, 3000));

            // I have so many spots making the case lower.  Let me know if I should go about making booleans as to
            // whether you want case checks all over or not
            p.mName = p.mName.ToLower();

            Debug.Log("PlayerAccount - Starting Save Test for " + p.mName + ":" + p.mType);

            // initialize it so you know when save has completed
            p.mSaveCompleted = false;

            // very kludgy check for whether something has saved or not
            // be aware something like this will actually stop the thread while waiting on a response
            // to the save.  A better way would be to invoke a delegate method for the return, or use
            // coroutines, etc.  This is just an example to show something saving, then loading.
            // second try-catch to make sure we exit the loop if issues
            DBWorker.Instance.Save(p, p.TestSaveResult);
 
            // yep do nothing... we're just waiting... 
            while (!p.mSaveCompleted)
                yield return new WaitForSeconds(.1f);
    
            // could load the object directly in the code here...
            Debug.Log("PlayerAccount - Starting Load Test for " + p.mName + ":" + p.mType);
            DBWorker.Instance.Load(p, p.TestLoadResult);

            // or could load the object here using a class method
            Debug.Log("PlayerAccount - Starting Second Load Test for " + p.mName + ":" + p.mType);
            p.Load(p.mName);

            yield return null;
        }

        public void TestSaveResult(bool success, GameObject owner, string methodName, Exception e = null)
        {
            if (e != null)
                DBTools.PrintException("Save", e);

            // confirm it has saved, else it will loop forever
            this.mSaveCompleted = true;
            Debug.Log("PlayerAccount Save result=" + success);
        }

        public void TestLoadResult(PlayerAccount response, GameObject owner, string methodName, Exception e = null)
        {
            if (e != null)
                DBTools.PrintException("Load", e);

            if (response == null)
            {
                Debug.Log("Response is NULL... Load Failure");
            }
            else
            {
                this.mWealth = response.mWealth;
                this.mWealth = response.mFame;

                Debug.Log("Response mWealth=" + response.mWealth);
                Debug.Log("Response mFame=" + response.mFame);
                Debug.Log("Player mWealth=" + this.mWealth);
                Debug.Log("Player mFame=" + this.mFame);
            }
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
