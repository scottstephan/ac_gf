using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class obj_Answer : MonoBehaviour {

    public GameObject thisAnswerMask;
    public Text answerTextField;
    public Text answerScoreField;
    public string answerText;
    public int id;
    public int answerScore;
    public int scoreValue;
    public Color answerRevealColor = Color.blue;
    public float animRevealLength = 1.5f;

    public enum E_answerState {
        hidden,
        revealed
    }

    public E_answerState thisAnswerState = E_answerState.hidden;


    public void initAnswer()
    {
        hideAnswer();
        RectTransform maskTransform = thisAnswerMask.GetComponent<RectTransform>();
        maskTransform.localScale = new Vector3(1,1,1);
        scoreValue = (id + 1) * 10;

        answerTextField.text = answerText.ToLower();
        answerScoreField.text = scoreValue.ToString();
    }

    public void setAnswerInfo(string aT, int iT)
    {
        answerText = aT;
        id = iT;
    }

    public void setAnswerState(E_answerState answerState)
    {

    }

    public void revealAnswer()
    {
        // thisAnswerMask.SetActive(false);
        thisAnswerMask.GetComponent<Image>().color = answerRevealColor;
        StartCoroutine("lerpMaskScale");
        thisAnswerState = E_answerState.revealed;
    }

    public void hideAnswer()
    {
        RectTransform tempAnswerMaskRect = thisAnswerMask.GetComponent<RectTransform>();
        RectTransform tempAnswerRect = gameObject.GetComponent<RectTransform>();
        tempAnswerMaskRect.sizeDelta = new Vector2(tempAnswerRect.rect.width, tempAnswerRect.rect.height);

        thisAnswerMask.SetActive(true);
    }

    public void setAnswerPosition(Vector3 pos)
    {
        gameObject.GetComponent<RectTransform>().position = pos;
    }

    IEnumerator lerpMaskScale()
    {
        float i = 0f;
        float rate = 1 / animRevealLength;
        RectTransform maskTransform = thisAnswerMask.GetComponent<RectTransform>();
        while(i < 1)
        {
            i += Time.deltaTime * rate;
            float maskScale = Mathf.Lerp(800,0,i);
            maskTransform.sizeDelta = new Vector2(maskScale, 75);

            yield return null;
        }
        
    }
}
