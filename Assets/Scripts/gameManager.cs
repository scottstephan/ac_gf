using UnityEngine;
using UnityEngine.SceneManagement;
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

    public static obj_Player currentPlayer;
    public static int maxPlayerMisses = 3; //Should we have a game settings script?
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
        currentPlayer = GameObject.Find("playerObject").GetComponent<obj_Player>();
        //Load in JSON Data
        numAnswers = 10;
        addDebugAnswers();
        //Declare and initalize players
        //Declare and initalize Answers
        //  createAnswerObjects();
        fillManualAnswerObjects();
        //Call UI manager to layout board
     //   m_gameUIManager.layoutAnswersDynamic(); //REMOVED FOR THE SAKE OF SANITY!
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

    void fillManualAnswerObjects()
    {
        obj_Answer[] answerObjectsInScene = GameObject.FindObjectsOfType<obj_Answer>();
        for(int i = 0; i < answerObjectsInScene.Length; i++)
        {
            answerObjectsInScene[i].answerText = roundAnswerStrings[i];
            answerObjectsInScene[i].id = i; //+ playerID
            answerObjectsInScene[i].initAnswer();

            roundAnswers.Add(answerObjectsInScene[i].gameObject);
        }
    }

    public static void checkPlayerInputAgainstAnswers(string playerAnswer)
    {
        bool hasHit = false;
        for(int i = 0; i < roundAnswerStrings.Count; ++i)
        {
            Debug.Log("Looking for " + playerAnswer + "==" + roundAnswerStrings[i]);
            if(playerAnswer == roundAnswerStrings[i].ToLower())
            {//Found a match....
                    if (roundAnswers[i].GetComponent<obj_Answer>().thisAnswerState != obj_Answer.E_answerState.revealed)
                    {//Found a match and we haven't gotten this one yet
                        Debug.Log("Found a hit");
                        roundAnswers[i].GetComponent<obj_Answer>().revealAnswer();
                        hasHit = true;
                        currentPlayer.playerHit(roundAnswers[i].GetComponent<obj_Answer>().scoreValue);
                        break;
                    }
                    else
                    {//Found a match, but we already got this one.
                        Debug.Log("Match, but already picked this answer. Breaking."); //could also store success picks and spare us the GetComps<>, but the perf implications are so low.
                        hasHit = true; //Lets the pick pass the logic gate @ end
                        break;
                    }
            }
            else
            {//No match on this item
                //Debug.Log("NO HIT");
            }
        }

        if (!hasHit)
        {
            currentPlayer.playerMissed();
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

    public static void endGame()
    {
        SceneManager.LoadScene(0);
    }

}