using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Boomlagoon.JSON;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


public class jsonImportTest : MonoBehaviour {
    public string testJSONText;
    string testQ = "{ \"questionName\": \"how long to grill\", \"questionDisplayText\": \"how long to grill\", \"questionID\": \"q052\",\"answers\": [\"chicken\",\"corn\",\"salmon\",\"chicken breast\",\"steak\",\"pork chops\",\"burgers\",\"chicken thighs\",\"corn on the cob\",\"chicken kabobs\"]}";
    public JSONArray categoryQuestions;
    string baseSavePathString;
    string catSavePathSuffix = "/categories/";
    string qSavePathSuffix = "/questions/";

    [System.Serializable]
    public class acQ
    {
        public string category;
        public string catID;
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

    void Start()
    {
        baseSavePathString = Application.persistentDataPath;
        createImportDirectories();
    }

	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
            readJsonSample();
        else if (Input.GetKeyDown(KeyCode.L))
            loadCategoryData();
        else if (Input.GetKeyDown(KeyCode.Q))
            loadRandomQuestionData();
	}

    void readJsonSample()
    {
        JSONValue cSS;
        JSONArray questionsArray;
        JSONObject tempJObj;

        acCat thisCat;
        tempJObj = JSONObject.Parse(testJSONText);
        cSS = tempJObj.GetValue("Category"); //This yields the high level category object
        thisCat = createCategoryObject(cSS);

        tempJObj = JSONObject.Parse(cSS.ToString()); //This yields an array of the questions
        questionsArray = tempJObj.GetArray("questions");
        createQuestionsObject(questionsArray, thisCat);
    }
    void createImportDirectories()
    {
        string catPath = baseSavePathString + catSavePathSuffix;
        string qPath = baseSavePathString + qSavePathSuffix;

        if (!Directory.Exists(catPath))
            Directory.CreateDirectory(catPath);
        if (!Directory.Exists(qPath))
            Directory.CreateDirectory(qPath);
    }
    acCat createCategoryObject(JSONValue categorySuperString)
    {
        JSONObject category = JSONObject.Parse(categorySuperString.ToString()); // This gives us the category section
        string catName = category.GetString("categoryName");
        string catID = category.GetString("categoryID");
        string catDispName = category.GetString("categoryDisplayText");

        acCat tCat = new acCat();
        tCat.categoryDisplayName = catDispName;
        tCat.categoryID = catID;
        tCat.categoryName = catName;
        tCat.displayCatInfo();

        string catJson = JsonUtility.ToJson(tCat);
        string catSavePath = baseSavePathString + catSavePathSuffix + catName + ".json";
        Debug.Log("CatsJson: " + catJson);
        SaveData(catJson, catSavePath);

        return tCat;
    }
    void createQuestionsObject(JSONArray questionsArray, acCat thisCat)
    {
        Debug.Log("Questions length:" + questionsArray.Length);

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
            tQ.category = thisCat.categoryName;
            tQ.catID = thisCat.categoryID;
            //Check the data
            tQ.displayQInfo();
            tQ.displayQCatInfo();
            //Get the answers to that question
            JSONArray answersArray = jsonQParser.GetArray("answers");
            tQ.jsonAnswers = answersArray;
            tQ.fillAnswersStruct();
            //Iterate and assign to the question
            
            string questionJson = JsonUtility.ToJson(tQ);
            string fPath = baseSavePathString + qSavePathSuffix + tQ.questionID + ".json";
            SaveData(questionJson, fPath);
            Debug.Log("Question Json " + questionJson);
        }
    }
    public void SaveData(string jsonToSave, string fPath)
    {
        //string fPath = baseSavePathString + catSavePathSuffix + "test.json";
        Debug.Log("Saving to: " + fPath);
        File.WriteAllText(fPath, jsonToSave);
    }
    public void loadCategoryData()
    {
        string fPath = baseSavePathString + catSavePathSuffix + "test.json";
        string loadedJson = File.ReadAllText(fPath);
        acCat tCat = JsonUtility.FromJson<acCat>(loadedJson);
        tCat.displayCatInfo();
    }

    public void loadRandomQuestionData()
    {
        string qRootDir = baseSavePathString + qSavePathSuffix;
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
    }
}
