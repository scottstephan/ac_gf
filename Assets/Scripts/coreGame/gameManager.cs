using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Amazon.DynamoDBv2.DataModel;
using DDBHelper;
using System;
using Assets.autoCompete.games;

public class gameManager : MonoBehaviour
{
    public static gameManager instance = null;

    public static GameObject masterUICanvas;
    public static GameObject answerLayoutGrid;
    public GameObject answersPrefab;
    public static List<string> roundAnswerStrings = new List<string>(); //WILL BE ANSWERS class objects
    public static List<GameObject> roundAnswers = new List<GameObject>();
    public static int numAnswers; //Will probably always be 10

    public static obj_Player currentPlayer;
    public static int maxPlayerMisses = 4; //Should we have a game settings script?
    public static int numPlayerHits = 0;

    public enum E_gameRoundStatus {
        waitingToStart,
        waitingForInput,
        endOfRound
    }

    public static E_gameRoundStatus currentRoundStatus;

    public enum E_endOfRoundAction
    {
        timerEnded,
        capturedPlayerInput
    }

    public enum E_endOfRoundResult
    {
        playerHit,
        playerMiss,
        playerHitMax,
        playerHitTwice,
        timerExpired
    }

    public E_endOfRoundResult currentRoundEndResult;

    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    void Start()
    {
       
    }

    IEnumerator InitGame()
    {
        masterUICanvas = GameObject.Find("g_UICanvas");
        answerLayoutGrid = GameObject.Find("answerLayoutGrid");
        currentPlayer = GameObject.Find("playerObject").GetComponent<obj_Player>();
        //Load in JSON Data
        numAnswers = 10;
        addDebugAnswers();
        //Declare and initalize players
        //Declare and initalize Answers
        fillManualAnswerObjects();
        currentRoundStatus = E_gameRoundStatus.waitingToStart;
        m_scoreAndGameStateManager.instance.roundStatusText.text = m_scoreAndGameStateManager.instance.gameStartText;
        //delay before starting round....
        yield return new WaitForSeconds(2f);
        //Start the round
        StartCoroutine(startRound());
        yield return null;
    }

    public void endGame(bool playerHasLost)
    {
        if (appManager.curLiveGame.isMPGame)
        {
            Debug.Log("Ending MP Game");
            if (appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated)
            {
                appManager.curLiveGame.p1_score = currentPlayer.totalScore;
                appManager.curLiveGame.p1_Fin = true;
            }
            else if(appManager.devicePlayerRoleInCurGame == appManager.playerRoles.challenged)
            {
                appManager.curLiveGame.p2_score = currentPlayer.totalScore;
                appManager.curLiveGame.p2_Fin = true;
            }

            appManager.roundPlayerObject = currentPlayer;
        }
        else
        {
            Debug.Log("Ending SP Game");
        }
        appManager.loadScene(appManager.sceneNames.scoreComp);

    }


    IEnumerator startRound()
    {
        //Show some ready text
        m_scoreAndGameStateManager.instance.roundStatusText.text = m_scoreAndGameStateManager.instance.roundStartText;
        //delay
        yield return new WaitForSeconds(1.5f);
        m_scoreAndGameStateManager.instance.roundStatusText.text = "";

        m_scoreAndGameStateManager.instance.setInputFieldAccessibility(true);
        //Start the timer
        obj_Timer.instance.setTimer();
        obj_Timer.instance.startTimer();
        currentRoundStatus = E_gameRoundStatus.waitingForInput;

        yield return null;
    }

    IEnumerator endRound(E_endOfRoundResult roundResult)
    {
        m_scoreAndGameStateManager.instance.roundStatusText.text = m_scoreAndGameStateManager.instance.roundEndWithInputText;

        switch (roundResult)
        {
            case E_endOfRoundResult.playerHit:
                m_scoreAndGameStateManager.instance.roundStatusText.text = m_scoreAndGameStateManager.instance.roundEndWithPlayerHit;
                break;
            case E_endOfRoundResult.playerMiss:
                m_scoreAndGameStateManager.instance.roundStatusText.text = m_scoreAndGameStateManager.instance.roundEndWithPlayerMiss;
                break;
            case E_endOfRoundResult.timerExpired:
                m_scoreAndGameStateManager.instance.roundStatusText.text = m_scoreAndGameStateManager.instance.roundEndWithTimeoutText;
                break;
            case E_endOfRoundResult.playerHitMax:
                m_scoreAndGameStateManager.instance.roundStatusText.text = m_scoreAndGameStateManager.instance.roundEndWithPlayerHitMax;
                break;
            case E_endOfRoundResult.playerHitTwice:
                m_scoreAndGameStateManager.instance.roundStatusText.text = m_scoreAndGameStateManager.instance.roundEndWithPlayerHitTwice;
                break;
        }
        yield return new WaitForSeconds(2f);
       //if game not over...
        StartCoroutine(startRound());
       //else endGame
        yield return null;
    }

    public void inputPhaseEnd(E_endOfRoundAction endRoundReason, string playerInput = "")
    {
        m_scoreAndGameStateManager.instance.setInputFieldAccessibility(false);
        bool playerHit = false;

        if (endRoundReason == E_endOfRoundAction.capturedPlayerInput)
        {
            obj_Timer.instance.stopTimer();
            playerHit = checkPlayerInputAgainstAnswers(playerInput); //Could be miss/hit
            currentRoundEndResult = playerHit == true ? E_endOfRoundResult.playerHit : E_endOfRoundResult.playerMiss;
            if (currentPlayer.numHits == 10) currentRoundEndResult = E_endOfRoundResult.playerHitMax;
            //NEED A 3rd CASE: Player hit, BUT they already guessed that, so NO BUENO
        }
        else if (endRoundReason == E_endOfRoundAction.timerEnded)
        {
            playerInputTimerEnded();
            playerHit = false; //Always a miss.
            currentRoundEndResult = E_endOfRoundResult.timerExpired;
        }

        StartCoroutine(endRound(currentRoundEndResult));
    }

    public static void playerInputTimerEnded()
    {
        obj_Player.instance.playerMissed();
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

    public static bool checkPlayerInputAgainstAnswers(string playerAnswer)
    {
        bool hasHit = false;
        bool hasHitAnswerBefore = false;
        for(int i = 0; i < roundAnswerStrings.Count; ++i)
        {
         //   Debug.Log("Looking for " + playerAnswer + "==" + roundAnswerStrings[i]);
            if(playerAnswer == roundAnswerStrings[i].ToLower())
            {//Found a match....
                    if (roundAnswers[i].GetComponent<obj_Answer>().thisAnswerState != obj_Answer.E_answerState.revealed)
                    {//Found a match and we haven't gotten this one yet
                        Debug.Log("Found a hit");
                        roundAnswers[i].GetComponent<obj_Answer>().revealAnswer();
                        hasHit = true;
                        currentPlayer.playerHit(roundAnswers[i].GetComponent<obj_Answer>().scoreValue);
                        return true;
                        break;
                    }
                    else
                    {//Found a match, but we already got this one.
                        Debug.Log("Match, but already picked this answer. Breaking."); //could also store success picks and spare us the GetComps<>, but the perf implications are so low.
                        hasHit = false; //Counts as a miss as per Justin. Should have a special error case for this.
                        hasHitAnswerBefore = true;
                        break;
                    }
            }
            else
            {//No match on this item
                
            }
        }

        if (!hasHit)
        {
            currentPlayer.playerMissed();
            return false;
        }

        return false; //Wtach out for this
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



    //Game works as follows: Pause -> Round Start -> Player Guess || Timer Ends -> Round End -> Pause -> Round Start
}