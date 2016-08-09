using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class obj_Answer : MonoBehaviour {

    public string answerText;
    public int id;

    public enum E_answerState {
        hidden,
        revealed
    }

    public E_answerState thisAnswerState = E_answerState.hidden;


    public void initAnswer()
    {
        //Grab UI components and set them
        gameObject.GetComponent<Text>().text = answerText;
    }

    public void setAnswerText(string answerText)
    {

    }

    public void setAnswerState(E_answerState answerState)
    {

    }

    public void revealAnswer()
    {
        //setAnswerState
        //PlayAnim
    }

    public void setAnswerPosition(Vector3 pos)
    {
        gameObject.GetComponent<RectTransform>().position = pos;
    }
}
