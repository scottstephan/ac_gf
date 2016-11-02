using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class u_acJsonUtility : MonoBehaviour {

    public static u_acJsonUtility instance = null;
    public int numCatsImportedForCallbackComplete = 0;
    int curNewCatsImported = 0;
    qDBInfo tempNewQDB;

    bool useResourcesFolder = false;
    public string jsonToImport;
    public JSONArray categoryQuestions;
    static string baseSavePathString;
    //For resource saving - Only works in Editor! There is no /resources/ path in a binary runtime
    static string baseResourcesPath;
    static string baseCatResourcesPath = "/jsonCategories/";
    //For local device saving
    static string catSavePathSuffix = "/categories/";
    static string qSavePathSuffix = "/questions/";
    static string catInfoSavePathPrefix = "catStatus/";
    static string qdbInfoSavePathSuffix = "/qdbinfo/";
    static string highScoreSavePathSuffix = "/highscores/";
    [System.Serializable]
    public class qDBInfo
    {
        public string QDBVersion;
        public string QDBNote;

        public void readQDBData()
        {
            Debug.Log("QDBVersion: " + QDBVersion + ":: QBNotes: " + QDBNote);
        }
    }

    [System.Serializable]
    public class categoryUnlockInfo
    {
        public string unlockStatus;
        public string categoryName;
        public string categoryID;

        public void readCategoryData()
        {
         //   Debug.Log(categoryName + " is " + unlockStatus + ". ID: " + categoryID);
        }
    }

    [System.Serializable]
    public class categoryHighScore
    {
        public string categoryName;
        public string categoryHighscore;

        public void readHighScore()
        {

        }
    }

    [System.Serializable]
    public class acQ
    {
        public string category;
        public string catID;
      //  public string catUnlockStatus;
        public string questionName;
        public string questionDisplayText;
        public string questionID;
        public JSONArray jsonAnswers;

        [System.Serializable]
        public struct answers
        {
            public string answer_1;
            public string answer_2;
            public string answer_3;
            public string answer_4;
            public string answer_5;
            public string answer_6;
            public string answer_7;
            public string answer_8;
            public string answer_9;
            public string answer_10;
        }
        public answers questionAnswers;

        public void fillAnswersStruct()
        {
            questionAnswers.answer_1 = jsonAnswers[0].ToString();
            questionAnswers.answer_2 = jsonAnswers[1].ToString();
            questionAnswers.answer_3 = jsonAnswers[2].ToString();
            questionAnswers.answer_4 = jsonAnswers[3].ToString();
            questionAnswers.answer_5 = jsonAnswers[4].ToString();
            questionAnswers.answer_6 = jsonAnswers[5].ToString();
            questionAnswers.answer_7 = jsonAnswers[6].ToString();
            questionAnswers.answer_8 = jsonAnswers[7].ToString();
            questionAnswers.answer_9 = jsonAnswers[8].ToString();
            questionAnswers.answer_10 = jsonAnswers[9].ToString();
        }

        public List<string> turnAnswerStructToList()
        {
            List<string> answersList = new List<string>();
            answersList.Add(questionAnswers.answer_1);
            answersList.Add(questionAnswers.answer_2);
            answersList.Add(questionAnswers.answer_3);
            answersList.Add(questionAnswers.answer_4);
            answersList.Add(questionAnswers.answer_5);
            answersList.Add(questionAnswers.answer_6);
            answersList.Add(questionAnswers.answer_7);
            answersList.Add(questionAnswers.answer_8);
            answersList.Add(questionAnswers.answer_9);
            answersList.Add(questionAnswers.answer_10);
            for(int i = 0; i < answersList.Count; ++i)
            {
               answersList[i] = answersList[i].Replace("\"","");
            }
            return answersList;
        }

        public void displayQCatInfo()
        {
            Debug.Log("QCat: " + category + ":: qCatID: " + catID);
        }

        public void displayQInfo()
        {
            Debug.Log("QName: " + questionName + ":: QID: " + questionID + ":: QdisplayText: " + questionDisplayText);
        }

        public void displayAnswers()
        {
            Debug.Log("A1: " + questionAnswers.answer_1);
            Debug.Log("A2: " + questionAnswers.answer_2);
            Debug.Log("A3: " + questionAnswers.answer_3);
            Debug.Log("A4: " + questionAnswers.answer_4);
            Debug.Log("A5: " + questionAnswers.answer_5);
            Debug.Log("A6: " + questionAnswers.answer_6);
            Debug.Log("A7: " + questionAnswers.answer_7);
            Debug.Log("A8: " + questionAnswers.answer_8);
            Debug.Log("A9: " + questionAnswers.answer_9);
            Debug.Log("A10: " + questionAnswers.answer_10);
        }
    }

    [System.Serializable]
    public class acCat
    {
        public string categoryName;
        public string categoryID;
        public string categoryDisplayName;

        public void displayCatInfo()
        {
            Debug.Log("Cat Name: " + categoryName + ":: CatID: " + categoryID + ":: CatDispName:" + categoryDisplayName);
        }
    }


    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

        // Use this for initialization
    void Start () {
        if (useResourcesFolder)
            baseSavePathString = "Assets/Resources/";
        else
            baseSavePathString = Application.persistentDataPath; //pDP necessary for Android
        baseResourcesPath = "Assets/Resources";
        createImportDirectories();
    }

    public void readJson()
    {
        JSONValue cSS;
        JSONArray questionsArray;
        JSONObject tempJObj;
        List<string> catList = getAllCategoryRawJson();
            
        for (int i = 0; i < catList.Count; ++i)
        {
            acCat thisCat;
            tempJObj = JSONObject.Parse(catList[i]);
            cSS = tempJObj.GetValue("Category"); //This yields the high level category object 
                                                 //I THINK that when importing the WHOLE thing, this would be an array and we'd cycle over it
            thisCat = createCategoryObject(cSS);

            tempJObj = JSONObject.Parse(cSS.ToString()); //This yields an array of the questions
            questionsArray = tempJObj.GetArray("questions");
            createQuestionsObject(questionsArray, thisCat.categoryName, thisCat.categoryID); 
        }
        //Save Resources QDB Object to local
        qDBInfo tqdb = returnCurQDBObject(); // First time, this returns NOTHING. Need to copy existing!
        string qdbJSON = JsonUtility.ToJson(tqdb);
        SaveData(qdbJSON, baseSavePathString + qdbInfoSavePathSuffix + "qdbinfo.json");
        appManager.instance.setCurQDBInfo(tqdb.QDBVersion);
    }

    public void writeBaseCatsToCategoryDirectory()
    {
        //This should run only the very first time- We should copy all the base stuff from the categoru directory to the player's local storage
    }

    public List<string> getAllCategoryRawJson()
    {
        TextAsset[] resourceJsonCats;
        List<string> unparsedCatJSon = new List<string>();
        resourceJsonCats = Resources.LoadAll<TextAsset>("jsonCategories");

        for (int i =0; i < resourceJsonCats.Length; i++)
        {
            string j = resourceJsonCats[i].ToString();
            unparsedCatJSon.Add(j);
        }

        return unparsedCatJSon;
    }

    public void createImportDirectories()
    {
        string catPath = baseSavePathString + catSavePathSuffix;
        string qPath = baseSavePathString + qSavePathSuffix;
        string qdbInfoPath = baseSavePathString + qdbInfoSavePathSuffix;
        string catStatusPath = catPath + catInfoSavePathPrefix;
        string highScorePath = baseSavePathString + highScoreSavePathSuffix;

        if (!Directory.Exists(catPath))
            Directory.CreateDirectory(catPath);
        if (!Directory.Exists(qPath))
            Directory.CreateDirectory(qPath);
        if (!Directory.Exists(catStatusPath))
            Directory.CreateDirectory(catStatusPath);
        if (!Directory.Exists(qdbInfoPath))
            Directory.CreateDirectory(qdbInfoPath);
        if (!Directory.Exists(highScorePath))
            Directory.CreateDirectory(highScorePath);
    }

    acCat createCategoryObject(JSONValue categorySuperString)
    {
        JSONObject category = JSONObject.Parse(categorySuperString.ToString()); // This gives us the category section
        string catName = category.GetString("categoryName");
        string catID = category.GetString("categoryID");
        string catDispName = category.GetString("categoryDisplayText");
        string catUnlockStatus = category.GetString("categoryUnlockStatus");
        //CREATE CAT Object
        acCat tCat = new acCat();
        tCat.categoryDisplayName = catDispName;
        tCat.categoryID = catID;
        tCat.categoryName = catName;
     //   tCat.displayCatInfo();
        //Create cat info object
        categoryUnlockInfo tCI = new categoryUnlockInfo();
        tCI.categoryName = catName;
        tCI.categoryID = catID;
        tCI.unlockStatus = catUnlockStatus;
        //Create highscore object
        categoryHighScore tHS = new categoryHighScore();
        tHS.categoryName = catName;
        tHS.categoryHighscore = "0";

        string catJson = JsonUtility.ToJson(tCat);
        string catSavePath = baseSavePathString + catSavePathSuffix + catName + ".json";

        SaveData(catJson, catSavePath);
        
        string catInfoJson = JsonUtility.ToJson(tCI);
        string catInfoSavePath = baseSavePathString + catSavePathSuffix + catInfoSavePathPrefix + catName + ".json";
        Debug.Log("Saving cat info json to: " + catInfoSavePath);

        string highScoreJson = JsonUtility.ToJson(tHS);
        string catHSPath = baseSavePathString + highScoreSavePathSuffix + tHS.categoryName + ".json";

        if (!File.Exists(catInfoSavePath))
            SaveData(catInfoJson, catInfoSavePath);
        if (!File.Exists(catHSPath))
            SaveData(highScoreJson, catHSPath);

        return tCat;
    }

    void createQuestionsObject(JSONArray questionsArray, string catName, string catID)
    { //SHOULD NUKE ANY EXISTING QUESTIONS
        Debug.Log("Questions length:" + questionsArray.Length);
        string fPathBase = baseSavePathString + qSavePathSuffix + catName + "/";
        string tempPath = baseSavePathString + qSavePathSuffix + catName + "/temp/";

        if (!Directory.Exists(tempPath))
            Directory.CreateDirectory(tempPath);
        if (!Directory.Exists(fPathBase))
            Directory.CreateDirectory(fPathBase);
        
        for (int i = 0; i < questionsArray.Length; i++)
        {
            JSONObject jsonQParser = JSONObject.Parse(questionsArray[i].ToString());
            acQ tQ = new acQ();
            //Create the question data
            string qName = jsonQParser.GetString("questionName");
            string qID = jsonQParser.GetString("questionID");
            string qDisp = jsonQParser.GetString("questionDisplayText");
            //Assign it a a questoon object
            tQ.questionName = qName;
            tQ.questionID = qID;
            tQ.questionDisplayText = qDisp;
            tQ.category = catName;
            tQ.catID = catID;
            
            //Get the answers to that question
            JSONArray answersArray = jsonQParser.GetArray("answers");
            tQ.jsonAnswers = answersArray;
            tQ.fillAnswersStruct();
            //Iterate and assign to the question

            string questionJson = JsonUtility.ToJson(tQ);

            
            string qPath = tempPath + tQ.questionID + ".json";

            SaveData(questionJson, qPath);
        }

        DirectoryInfo dir = new DirectoryInfo(tempPath);
        FileInfo[] files = dir.GetFiles();
        for(int i = 0; i < files.Length; i++)
        {
            files[i].CopyTo(fPathBase + files[i].Name,true);
        }
        Directory.Delete(tempPath,true);
    }

    public void SaveData(string jsonToSave, string fPath)
    {
        //string fPath = baseSavePathString + catSavePathSuffix + "test.json";
     //   Debug.Log("Saving to: " + fPath);
        File.WriteAllText(fPath, jsonToSave);
    }

    public void loadCategoryData()
    {
        string fPath = baseSavePathString + catSavePathSuffix + "test.json";
        string loadedJson = File.ReadAllText(fPath);
        acCat tCat = JsonUtility.FromJson<acCat>(loadedJson);
        tCat.displayCatInfo();
    }
   
    public acQ loadRandomQuestionData(string categoryName)
    {
        string qRootDir = baseSavePathString + qSavePathSuffix + categoryName + "/";
        string filePrefix = "q";
        string[] questionFiles = Directory.GetFiles(qRootDir, "*");

        string minNum = questionFiles[0].Substring(questionFiles[0].Length - 8, 3);
        int lastIndex = questionFiles.Length - 1;
        string maxNum = questionFiles[lastIndex].Substring(questionFiles[lastIndex].Length - 8, 3);

        int minN = 0;
        int.TryParse(minNum, out minN);

        int maxN = 0;
        int.TryParse(maxNum, out maxN);
        Debug.Log("minNum: " + minN + ":: maxNum: " + maxN);

        int qNum = Random.Range(minN, maxN);
        string fileNumber = qNum.ToString();
        if (qNum < 100)
            fileNumber = "0" + fileNumber;

        string fPath = qRootDir + filePrefix + fileNumber + ".json";

        string loadedJson = File.ReadAllText(fPath);
        acQ tQ = JsonUtility.FromJson<acQ>(loadedJson);

        tQ.displayQCatInfo();
        tQ.displayQInfo();
        tQ.displayAnswers();

        return tQ;
    }

    public acQ loadSpecificQuestionData(string questionID, string categoryName)
    {
        Debug.Log("---LOADING SPECIFIC Q FROM JSON ARCHIVES: " + questionID + "::" + categoryName);
        string qRootDir = baseSavePathString + qSavePathSuffix + categoryName + "/";
        string filePrefix = questionID;
        string fileExtension = ".json";

        string fPath = qRootDir + filePrefix + fileExtension;
        Debug.Log("Trying to load: " + fPath);
        string loadedJson = File.ReadAllText(fPath);
        acQ tQ = JsonUtility.FromJson<acQ>(loadedJson);

        tQ.displayQInfo();
        return tQ;
    }

    public List<string> discoverCategories(bool returnWithoutDefaults)
    {
        string catRoot = baseSavePathString + catSavePathSuffix;
        string[] categoryDirectories = Directory.GetFiles(catRoot);
        List<string> categoryNames = new List<string>();

        for(int i = 0; i < categoryDirectories.Length; ++i)
        {
            Debug.Log("Categories:" + categoryDirectories[i]);
            categoryNames.Add(Path.GetFileNameWithoutExtension(categoryDirectories[i]));
        }

        return categoryNames;
    }

    public List<categoryUnlockInfo> discoverAllCategoryUnlockInfo()
    {
        string fileRoot = baseSavePathString + catSavePathSuffix + catInfoSavePathPrefix;
        string[] catInfoNames = Directory.GetFiles(fileRoot);
        List<categoryUnlockInfo> catInfo = new List<categoryUnlockInfo>(); 

        for(int i = 0; i <catInfoNames.Length; i++)
        {
            string catInfoJson = File.ReadAllText(catInfoNames[i]);
            categoryUnlockInfo tCUI = JsonUtility.FromJson<categoryUnlockInfo>(catInfoJson);
            tCUI.readCategoryData();
            catInfo.Add(tCUI);
        }

        return catInfo;
    }

    public categoryUnlockInfo getCategoryUnlockInfo(string cName)
    {
        string fileRoot = baseSavePathString + catSavePathSuffix + catInfoSavePathPrefix;
        string[] catInfoNames = Directory.GetFiles(fileRoot);

        for (int i = 0; i < catInfoNames.Length; i++)
        {
            string catInfoJson = File.ReadAllText(catInfoNames[i]);
            categoryUnlockInfo tCUI = JsonUtility.FromJson<categoryUnlockInfo>(catInfoJson);
            if(tCUI.categoryName == cName)
            {
                return tCUI;
            }
        }

        return null;
    }

    public qDBInfo returnCurQDBObject()
    {
        qDBInfo tInfo = new qDBInfo();
        
        string lJ = File.ReadAllText(baseSavePathString + qdbInfoSavePathSuffix + "qdbinfo.json");
        
        Debug.Log("QDB JSON: " + lJ);
        tInfo = JsonUtility.FromJson<qDBInfo>(lJ);
        tInfo.readQDBData();

        return tInfo;
    }

    public void createCatInfoFile()
    {
        categoryUnlockInfo tCI = new categoryUnlockInfo();
        tCI.categoryID = "test";
        tCI.categoryName = "testtest";
        tCI.unlockStatus = "locked";
        string s_tci = JsonUtility.ToJson(tCI);
        Debug.Log(s_tci);
        string fPath = baseSavePathString + catSavePathSuffix + catInfoSavePathPrefix;
        Debug.Log("Saving cat info to: " + fPath);
      //  SaveData(s_tci, fPath);
    }

    public void findAndUnlockCategory(string cName)
    {
        string fileRoot = baseSavePathString + catSavePathSuffix + catInfoSavePathPrefix+cName+".json";

        categoryUnlockInfo catInfoObj = getCategoryUnlockInfo(cName);
        catInfoObj.unlockStatus = "unlocked";
        string newInfo = JsonUtility.ToJson(catInfoObj);

        SaveData(newInfo, fileRoot);
    }

    public void findAndLockCategory(string cName)
    {
        string fileRoot = baseSavePathString + catSavePathSuffix + catInfoSavePathPrefix + cName + ".json";

        categoryUnlockInfo catInfoObj = getCategoryUnlockInfo(cName);
        catInfoObj.unlockStatus = "locked";
        string newInfo = JsonUtility.ToJson(catInfoObj);

        SaveData(newInfo, fileRoot);
    }

    IEnumerator checkWebQDB()
    {
        string url = "https://s3.amazonaws.com/autocompete/qDBInfo.json";
        WWW www = new WWW(url);
        yield return www;
        if (www.error == null)
        {
//             Debug.Log(www.data);
            compareWebQDBToLocalQDB(www.data);
        }
        else
        {
            Debug.Log("ERROR: " + www.error);
            m_loadScreenManager.instance.appInitComplete(); //Prevent app lockup if not online or server error
        }
    
    }

    void compareWebQDBToLocalQDB(string s_webQDB)
    {
        JSONObject localQDB = JSONObject.Parse(File.ReadAllText(baseSavePathString + qdbInfoSavePathSuffix + "qdbinfo.json"));
       // JSONObject localQDB = JSONObject.Parse(Resources.Load<TextAsset>("qdbInfo").ToString()); //O-DO: This needs to be local
        JSONObject webQDB = JSONObject.Parse(s_webQDB);

 //       Debug.Log("Local: " + localQDB.GetString("QDBVersion") + "::" + "Web: " + webQDB.GetString("QDBVersion"));

        if(localQDB.GetString("QDBVersion") != webQDB.GetString("QDBVersion"))
        {
            Debug.Log("QDB VERSION MISMATCH!");
            processNewQDB(webQDB);
        }
        else
        {
            m_loadScreenManager.instance.appInitComplete(); //They match, so proceed
        }
    }

    public void processNewQDB(JSONObject qdbObj)
    {
        Debug.Log(qdbObj.GetString("QDBVersion"));
        Debug.Log(qdbObj.GetString("QDBNote"));
        tempNewQDB = new qDBInfo();
        tempNewQDB.QDBVersion = qdbObj.GetString("QDBVersion");
        tempNewQDB.QDBNote = qdbObj.GetString("QDBNote");

        JSONArray catsChanged = qdbObj.GetArray("categoriesUpdated");
        numCatsImportedForCallbackComplete = catsChanged.Length;
        for (int i = 0; i < catsChanged.Length; i++)
        {
            Debug.Log(catsChanged[i].Obj.GetString("catName"));
        }

        for (int i = 0; i < catsChanged.Length; i++)
        {
            StartCoroutine("getNewCategoryJson", catsChanged[i].Obj.GetString("catName"));
        }
    }

    public void newCatImportComplete()
    {
        curNewCatsImported++;
        if(curNewCatsImported == numCatsImportedForCallbackComplete)
        {
            Debug.Log("All new cats imported");
            SaveData(JsonUtility.ToJson(tempNewQDB), baseSavePathString + qdbInfoSavePathSuffix + "qdbinfo.json");
            appManager.instance.setCurQDBInfo(tempNewQDB.QDBVersion);
            m_loadScreenManager.instance.appInitComplete();
        }

    }

    IEnumerator getNewCategoryJson(string catChanged)
    {
        Debug.Log("Getting new category data for: " + catChanged);

        string url = "https://s3.amazonaws.com/autocompete/categories/" + catChanged + ".json";
        WWW www = new WWW(url);
        yield return www;
        if (www.error == null)
        {
            Debug.Log(www.data);
           
            JSONObject catData = JSONObject.Parse(www.data);
            JSONValue catValue = catData.GetValue("Category");
            JSONObject categorySubObject = JSONObject.Parse(catValue.ToString());

            acCat thisCat = createCategoryObject(catValue);
            Debug.Log(catData);

            JSONArray questionsArray = categorySubObject.GetArray("questions");

            createQuestionsObject(questionsArray, thisCat.categoryName, thisCat.categoryID);
            newCatImportComplete(); //faux callback. god help us all. 
        }
        else
        {
            Debug.Log("ERROR: " + www.error);
            numCatsImportedForCallbackComplete--; //To prevent callback lockup
        }
    }

    public void checkFirstTimeCopy()
    {//if the dirs exist, then we've already copied the default q set or its been updated. Nuke it!
        string defaultDirURL = baseSavePathString +"/categories/";
        if (Directory.GetFiles(defaultDirURL).Length > 0)
      {
            Debug.Log("Cat files in directory; Initial import has been completed. Returning");
            return;
        }
        else
        {
            Debug.Log("No cat files in directory! Starting base q set import!");
            //Save Resoureces QDB json to local
            string qdbJson = Resources.Load<TextAsset>("qDBInfo").ToString();
            SaveData(qdbJson, baseSavePathString + qdbInfoSavePathSuffix + "qdbinfo.json");
            readJson();
        }
    }

    public int getCatHighscore(string catName)
    {
        string hsFilePath = baseSavePathString + highScoreSavePathSuffix + catName + ".json"; ;
        categoryHighScore tHS = JsonUtility.FromJson<categoryHighScore>(File.ReadAllText(hsFilePath));
        Debug.Log(tHS.categoryName + " :: " + tHS.categoryHighscore);
        return int.Parse(tHS.categoryHighscore);
    }

    public void updateHighScore(string catName, int scoreVal)
    {
        string hsFilePath = baseSavePathString + highScoreSavePathSuffix + catName + ".json"; ;

        categoryHighScore tHS = new categoryHighScore();
        tHS.categoryName = catName;
        tHS.categoryHighscore = scoreVal.ToString();

        SaveData(JsonUtility.ToJson(tHS), hsFilePath);
    }
}

