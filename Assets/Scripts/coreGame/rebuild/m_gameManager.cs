using UnityEngine;
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
    public Text playerInputText;
    public Text playerScoreText;
    public Text roundNumberText;
    public Text roundStatusText;

    public m_roundAdvanceButton advanceButton;

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

    public void init(bool hasQSet)
    {
        reset();
        if (!hasQSet)
            loadRandomQuestions();
        else
            loadExistingQuestionSet();
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
        roundNumberText.text = "ROUND 1" + "/" + numRounds;
        roundStatusText.text = "";

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
        m_gameManager.instance.playerInputText.color = Color.black;

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
            advanceButton.myButtonRole = m_roundAdvanceButton.buttonRole.advanceToNextRound;
            playerInput.text = "";
            advanceButton.setTextByRole();
            advanceButton.toMid.animationParts.ObjectState = UITween.AnimationParts.State.CLOSE; //To prevent snapping
            advanceButton.toMid.OpenCloseObjectAnimation();
           // StartCoroutine("delayAndCall", delayTypes.endRoundToStartRound);
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
            appManager.curLiveGame.p1_score = int.Parse(playerScoreText.text);

        }
        advanceButton.myButtonRole = m_roundAdvanceButton.buttonRole.endGame;
        advanceButton.setTextByRole();
        playerInput.text = "";
        
        advanceButton.toMid.OpenCloseObjectAnimation();
       
    }

    public void startInputPhase()
    {
        timer.resetTimer();
        timer.startTimer();
        playerInput.text = "";
        playerInput.interactable = true;
        playerInput.ActivateInputField(); //yield focus
        TouchScreenKeyboard.hideInput = true;
    }

    public void endInputPhase()
    { 
        timer.stopTimer();
        //     playerInput.DeactivateInputField();
        playerInput.interactable = false; //Stops input. However, on DeactivateInputField() it flags the OnEndEdit event leading to double input. Could do a flag.
        roundObjects[roundIndex].GetComponent<m_roundManager>().startValidationPhase();
    }

    public void loadRandomQuestions()
    {
        string fullQString = "";
        for(int i = 0; i < numRounds; i++)
        {
            u_acJsonUtility.acQ tQ = new u_acJsonUtility.acQ();
            tQ = u_acJsonUtility.instance.loadRandomQuestionData(currentSelectedCategory);
            questionSet.Add(tQ);
            fullQString += tQ.questionID; //Capturing for now. Used in save/load on MP games
        }

        if (appManager.curLiveGame != null)
        {
            appManager.curLiveGame.questionID = fullQString;
            appManager.curLiveGame.categoryText = currentSelectedCategory;
        }

    }

    public void loadExistingQuestionSet()
    {
        List<string> qIds = appManager.instance.loadQuestionIDs();

        for(int i = 0; i < qIds.Count; i++)
        {
            u_acJsonUtility.acQ tQ = new u_acJsonUtility.acQ();
            tQ = u_acJsonUtility.instance.loadSpecificQuestionData(qIds[i], appManager.curLiveGame.categoryText);
            questionSet.Add(tQ);
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
        //If answer > 9999 , crash??
        // playerScoreText.text = tS.ToString("N0");
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
        int counterIndex = roundIndex + 1;
        if (counterIndex > 3) counterIndex = 3;
    }

    public void updateRoundCounter()
    {
        int counterIndex = roundIndex + 1;
        roundNumberText.text = "ROUND " + counterIndex + "/" + numRounds;

    }

    public void setCurrentSelectCategory(string catName)
    {
        currentSelectedCategory = catName;
    }

    public void timerOver()
    {
        roundObjects[roundIndex].GetComponent<m_roundManager>().startValidationPhase();
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

