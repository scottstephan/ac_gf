using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class m_roundManager : MonoBehaviour {
    public Text questionText;
    public Transform parentPanel;
    public List<obj_Answer> gridAnswerObjects = new List<obj_Answer>();
    public List<string> qAnswers = new List<string>();
    public u_acJsonUtility.acQ questionSet;
    public int hitOrMissAnswerIndex = 0;

    public EasyTween toLeft;
    public EasyTween toCenter;

    public enum validationRoundEndResult
    {
        playerHit,
        playerMiss,
        playerHitDouble,
        playerTimeOut
    }

    public void initRound()
    {
        if (parentPanel == null) parentPanel = GameObject.Find("4_MainRound").GetComponent<RectTransform>();
        qAnswers = questionSet.turnAnswerStructToList();
        setPos();
        setAnswerGrid();
        setQuestionText(questionSet.questionDisplayText);
        //Fill answer grid

    }

    public void setPos()
    {
        gameObject.GetComponent<RectTransform>().SetParent(parentPanel);
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector3(-1600,0,0);
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

    }

    public void setQuestion(u_acJsonUtility.acQ tQ)
    {
        questionSet = tQ;
    }

	public void setAnswerGrid()
    {
        for(int i = 0; i < gridAnswerObjects.Count; ++i)
        {
            gridAnswerObjects[i].setAnswerInfo(qAnswers[i],i); //Presumes they're in order! Why weren't they before?
            gridAnswerObjects[i].initAnswer();
        }
    }

    public void setQuestionText(string qT)
    {
        questionText.text = qT.ToLower();
    }
	
	public void checkAnswerAgainstInput(string input)
    {

    }

    public void moveRoundIn()
    {
        toCenter.OpenCloseObjectAnimation();
    }

    public void moveRoundOut()
    {
        toLeft.OpenCloseObjectAnimation(); 
    }

    public void startValidationPhase()
    {
        //Get Input
        string playerInput = m_gameManager.instance.playerInput.text;
        playerInput = playerInput.ToLower();
        Debug.Log("Player input is: " + playerInput);
        //Check it against answers
        validationRoundEndResult result = returnValidationResult(playerInput);
        //Act on the result 
        actOnValidationResult(result);
        //Will likely want a delay here. 
        m_gameManager.instance.StartCoroutine("delayAndCall", m_gameManager.delayTypes.resultToInput);        
    }

    validationRoundEndResult returnValidationResult(string sanitizedInput)
    {
        for(int i = 0; i < gridAnswerObjects.Count; ++i)
        {
            if(gridAnswerObjects[i].answerText == sanitizedInput)
            {
                if (gridAnswerObjects[i].thisAnswerState == obj_Answer.E_answerState.hidden)
                {
                    Debug.Log(sanitizedInput + "is the same as answer " + gridAnswerObjects[i].answerText + "and the answer is hidden. TRUE.");
                    hitOrMissAnswerIndex = i;
                    return validationRoundEndResult.playerHit;
                }
                else
                {
                    Debug.Log(sanitizedInput + "is the same as answer " + gridAnswerObjects[i].answerText + "and the answer is revealed. FALSE");
                    hitOrMissAnswerIndex = i;
                    return validationRoundEndResult.playerHitDouble;
                }
            }
        }

        Debug.Log(sanitizedInput + " has no match in grid. FALSE");
        hitOrMissAnswerIndex = -1;
        return validationRoundEndResult.playerMiss;   
    }

    public void actOnValidationResult(validationRoundEndResult result)
    {
        switch (result)
        {
            case (validationRoundEndResult.playerHit):
                gridAnswerObjects[hitOrMissAnswerIndex].revealAnswer();
                m_gameManager.instance.incrementScore(gridAnswerObjects[hitOrMissAnswerIndex].scoreValue);
                m_gameManager.instance.playerInputText.color = Color.green;
                break;
            case (validationRoundEndResult.playerHitDouble):
                m_gameManager.instance.incrementMisses();
                m_gameManager.instance.animateLightbulb();
                m_gameManager.instance.playerInputText.color = Color.red;

                break;
            case (validationRoundEndResult.playerMiss):
                m_gameManager.instance.incrementMisses();
                m_gameManager.instance.animateLightbulb();
                m_gameManager.instance.playerInputText.color = Color.red;
                break;
            case (validationRoundEndResult.playerTimeOut): //This does NOT pass through the startValidationPhase() function. Make sure nothing critical is set there. 
                m_gameManager.instance.incrementMisses();
                m_gameManager.instance.animateLightbulb();
                m_gameManager.instance.StartCoroutine("delayAndCall", m_gameManager.delayTypes.endRoundToStartRound);

                break;
        }
    }

    public void revealAllAnswers()
    {
        for(int i = 0; i < gridAnswerObjects.Count; i++)
        {
            if(gridAnswerObjects[i].thisAnswerState == obj_Answer.E_answerState.hidden)
            {
                // gridAnswerObjects[i].revealAnswer();
                gridAnswerObjects[i].answerTextField.fontStyle = FontStyle.Normal;
                gridAnswerObjects[i].thisAnswerMask.SetActive(false);
            }
        }
    }
}
