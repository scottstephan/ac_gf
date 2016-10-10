﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class m_gameManager : MonoBehaviour {
    public GameObject roundObject;
    public List<u_acJsonUtility.acQ> questionSet = new List<u_acJsonUtility.acQ>();
    public List<GameObject> roundObjects = new List<GameObject>();
    public static m_gameManager instance = null;
    public string currentSelectedCategory;
    public int numRounds = 3;
    public int roundIndex = 0;
    public int maxNumMisses = 4;
    int playerMisses = 0;
    public List<GameObject> lightBulbs = new List<GameObject>();

    public InputField playerInput;
    public Text playerScoreText;
    public Text roundNumberText;

    public obj_Timer timer;
    public enum roundPhases
    {
        input,
        validation,
        result,
        swapRound
    }
    public enum delayTypes
    {
        resultToInput,
        startToInput,
        endRoundToStartRound
    }

    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void init()
    {
        reset();
        loadQuestions();
        createRoundInterfaces();
        moveInRoundInterface(0);
        timer.setTimer();
        timer.resetTimer();
        changePhase(roundPhases.input);  //WIll probs want a delay here
    }

    public void reset()
    {
        for(int i = 0; i < roundObjects.Count; i++)
        {
            Destroy(roundObjects[i]);
        }

        questionSet.Clear();
        roundObjects.Clear();
        resetLightBulbs();

        roundIndex = 0;
        playerMisses = 0;
        playerScoreText.text = "0";

    }

    public void resetRoundInfo()
    {
        playerMisses = 0;
        resetLightBulbs();
    }

    public void resetLightBulbs()
    {
        for (int i = 0; i < lightBulbs.Count; ++i)
        { //Need to wait for old anim to finish!
            lightBulbs[i].GetComponent<Animation>().Stop(); //Stop-gap solve for color stomping
            lightBulbs[i].GetComponent<Image>().color = Color.yellow;
        }
    }

    public void playerInputComplete()
    {
        changePhase(roundPhases.validation);
    }

    public void changePhase(roundPhases rP)
    {
        if (playerMisses >= maxNumMisses)
            rP = roundPhases.swapRound;

        switch (rP)
        {
            case roundPhases.input:
                startInputPhase(); 
                break;
            case roundPhases.result:
                break;
            case roundPhases.validation:
                endInputPhase();    
                break;
            case roundPhases.swapRound:
                swapRounds();
                break;
        }
    }

    public void swapRounds()
    {
        roundObjects[roundIndex].GetComponent<m_roundManager>().revealAllAnswers();
        incrementRoundsPlayed();
        if (roundIndex < numRounds)
        {
            StartCoroutine("delayAndCall", delayTypes.endRoundToStartRound);
        }
        else
        {
            endGame();
        }
    }

    public void endGame()
    {
        if (appManager.curLiveGame.isMPGame)
        {
            Debug.Log("Ending MP Game");
            if (appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated)
            {
                appManager.curLiveGame.p1_score = int.Parse(playerScoreText.text);
                appManager.curLiveGame.p1_Fin = true;
            }
            else if (appManager.devicePlayerRoleInCurGame == appManager.playerRoles.challenged)
            {
                appManager.curLiveGame.p2_score = int.Parse(playerScoreText.text);
                appManager.curLiveGame.p2_Fin = true;
            }

           // appManager.roundPlayerObject = currentPlayer;
        }
        else
        {
            Debug.Log("Ending SP Game");
        }
        //This is where we need to move in a new round OR move to scoreComp
        m_phaseManager.instance.changePhase(m_phaseManager.phases.scoreComp);
    }

    public void startInputPhase()
    {
        timer.resetTimer();
        timer.startTimer();
        playerInput.text = ""; 
        playerInput.ActivateInputField();
    }

    public void endInputPhase()
    {
        timer.stopTimer();
        roundObjects[roundIndex].GetComponent<m_roundManager>().startValidationPhase();
        playerInput.DeactivateInputField();
    }

    public void loadQuestions()
    {
        string fullQString = "";
        for(int i = 0; i < numRounds; i++)
        {
            u_acJsonUtility.acQ tQ = new u_acJsonUtility.acQ();
            tQ = u_acJsonUtility.instance.loadRandomQuestionData(currentSelectedCategory);
            questionSet.Add(tQ);
            fullQString += tQ.questionID; //Capturing for now. Used in save/load on MP games
        }
    }

    public void createRoundInterfaces()
    { // We could also create them 1 at a time...
        for(int i = 0; i < numRounds; i++)
        { 
            GameObject tR = Instantiate(roundObject);
            m_roundManager tRM = tR.GetComponent<m_roundManager>();
            tRM.setQuestion(questionSet[i]);
            tRM.initRound();

            roundObjects.Add(tR);
        }
       
    }

    public void moveInRoundInterface(int index)
    {
        if (index > 0)
            roundObjects[index - 1].GetComponent<m_roundManager>().moveRoundOut();
        roundObjects[index].GetComponent<m_roundManager>().moveRoundIn();
        
    }

    public void incrementScore(int scoreAmt)
    {
        int tS = int.Parse(playerScoreText.text);
        tS += scoreAmt;
        playerScoreText.text = tS.ToString();
    }

    public void incrementMisses()
    {
        playerMisses++;
    }

    public void animateLightbulb()
    {
        lightBulbs[playerMisses - 1].GetComponent<Animation>().Play();
    }

    public void incrementRoundsPlayed()
    {
        roundIndex++;
    }

    public void setCurrentSelectCategory(string catName)
    {
        currentSelectedCategory = catName;
    }

    public void timerOver()
    {
        roundObjects[roundIndex].GetComponent<m_roundManager>().actOnValidationResult(m_roundManager.validationRoundEndResult.playerTimeOut);
    }

    IEnumerator delayAndCall(delayTypes delayType)
    {
        float delay = 2f;
        if (delayType == delayTypes.endRoundToStartRound)
            delay = 5f;
        yield return new WaitForSeconds(delay);

        switch (delayType)
        {
            case delayTypes.endRoundToStartRound:
                resetRoundInfo(); 
                moveInRoundInterface(roundIndex);
                changePhase(roundPhases.input);
                break;
            case delayTypes.resultToInput:
                changePhase(roundPhases.input);
                break;
            case delayTypes.startToInput:
                break;
        }
    }
}

