using UnityEngine;
using System.Collections;
using System.Collections.Generic;      

public class gameManager : MonoBehaviour
{
    public static GameObject masterUICanvas;
    public static GameObject answerLayoutGrid;
    public GameObject answersPrefab;
    public static gameManager instance = null;
    public static List<string> roundAnswerStrings = new List<string>(); //WILL BE ANSWERS class objects
    public static List<GameObject> roundAnswers = new List<GameObject>();
    public static int numAnswers; //Will probably always be 10
    
    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        InitGame();
    }

    void InitGame()
    {
        masterUICanvas = GameObject.Find("g_UICanvas");
        answerLayoutGrid = GameObject.Find("answerLayoutGrid");
        //Load in JSON Data
        numAnswers = 10;
        addDebugAnswers();
        //Declare and initalize players
        //Declare and initalize Answers
        createAnswerObjects();
        //Call UI manager to layout board
        m_gameUIManager.layoutAnswers();
        //Gather up answers
    }

    void createAnswerObjects()
    {
        for(int i = 0; i < roundAnswerStrings.Count; i++)
        {
            GameObject obj_tempAnswer = Instantiate(answersPrefab);
            obj_Answer tempAnswer = obj_tempAnswer.GetComponent<obj_Answer>();
            tempAnswer.answerText = roundAnswerStrings[i];
            tempAnswer.id = i; //+ playerID
            tempAnswer.initAnswer();

            roundAnswers.Add(obj_tempAnswer);
        }
    }

    public static void checkPlayerInputAgainstAnswers(string playerAnswer)
    {
        for(int i = 0; i < roundAnswerStrings.Count; ++i)
        {
            Debug.Log("Looking for " + playerAnswer + "==" + roundAnswerStrings[i]);
            if(playerAnswer == roundAnswerStrings[i])
            { //Should maybe just be searching the Answer objects.
                Debug.Log("Found a hit");
                roundAnswers[i].GetComponent<obj_Answer>().revealAnswer();
                break;
            }
            else
            {
                //Debug.Log("NO HIT");
            }
            Debug.Log("END OF LOOP");
        }
    }

    void addDebugAnswers()
    {
        roundAnswerStrings.Add("Scott");
        roundAnswerStrings.Add("Suzie");
        roundAnswerStrings.Add("Justin");
        roundAnswerStrings.Add("Rosie");
        roundAnswerStrings.Add("Mike");
        roundAnswerStrings.Add("Sally");
        roundAnswerStrings.Add("Narek");
        roundAnswerStrings.Add("Douglas");
        roundAnswerStrings.Add("Frank");
        roundAnswerStrings.Add("Miller");
    }

}