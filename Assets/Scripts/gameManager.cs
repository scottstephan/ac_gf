using UnityEngine;
using System.Collections;
using System.Collections.Generic;      

public class gameManager : MonoBehaviour
{

    public static gameManager instance = null;
    public static List<string> roundAnswerStrings = new List<string>(); //WILL BE ANSWERS class objects
    public static List<obj_Answer> roundAnswers = new List<obj_Answer>();
    public static int numAnswers; //Will probably always be 10

    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    void InitGame()
    {
        //Load in JSON Data
        numAnswers = 10;
        addDebugAnswers();
        //Declare and initalize players
        //Declare and initalize Answers
        createAnswerObjects();
        //Call UI manager to layout board
        //Gather up answers
    }

    void createAnswerObjects()
    {
        for(int i = 0; i < roundAnswerStrings.Count; i++)
        {
            obj_Answer tempAnswer = new obj_Answer();
            tempAnswer.answerText = roundAnswerStrings[i];
            tempAnswer.id = i; //+ playerID
            tempAnswer.initAnswer();

            roundAnswers.Add(tempAnswer);
        }
    }

    void checkPlayerInputAgainstAnswers(string playerAnswer)
    {

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