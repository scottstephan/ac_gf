using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class obj_Answer : MonoBehaviour {

    public GameObject thisAnswerMask;
    public string answerText;
    public int id;

    public enum E_answerState {
        hidden,
        revealed
    }

    public E_answerState thisAnswerState = E_answerState.hidden;


    public void initAnswer()
    {
        gameObject.GetComponent<Text>().text = answerText;
        hideAnswer();
        //SET A FONT
        //SET A NAME
    }

    public void setAnswerText(string answerText)
    {

    }

    public void setAnswerState(E_answerState answerState)
    {

    }

    public void revealAnswer()
    {
        thisAnswerMask.SetActive(false);
        //setAnswerState
        //PlayAnim
    }

    public void hideAnswer()
    {
        RectTransform tempAnswerMaskRect = thisAnswerMask.GetComponent<RectTransform>();
        RectTransform tempAnswerRect = gameObject.GetComponent<RectTransform>();
        tempAnswerMaskRect.sizeDelta = new Vector2(tempAnswerRect.rect.width, tempAnswerRect.rect.height);
    }

    public void setAnswerPosition(Vector3 pos)
    {
        gameObject.GetComponent<RectTransform>().position = pos;
    }
}
