﻿using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class obj_Answer : MonoBehaviour {

    public GameObject thisAnswerMask;
    public Text answerTextField;
    public Text answerScoreField;
    public string answerText;
    public string answerText_sanitized;
    public int id;
    public int answerScore;
    public int scoreValue;
    Color answerRevealColor;
    public float animRevealLength = 1.5f;
    public Image rowBG;

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
        scoreValue = (10 - id) * 1000;
        rowBG.color = thisAnswerMask.GetComponent<Image>().color; 
        answerRevealColor = m_colorPaletteManager.instance.buttonColorPalette.palette[2];
        setTextStyle();
    }

    public void setAnswerInfo(string aT,string aT_s, int iT)
    {
        answerText = aT;
        answerText_sanitized = aT_s;
        id = iT;
    }

    public void setTextStyle()
    {
        int fontSize = 40;

        answerTextField.text = answerText.ToLower();

        answerScoreField.text = scoreValue.ToString("N0");

        answerTextField.fontSize = fontSize;
        answerScoreField.fontSize = fontSize;

        answerScoreField.color = m_colorPaletteManager.instance.buttonColorPalette.palette[2];

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

        answerTextField.fontStyle = FontStyle.Bold;
        answerScoreField.fontStyle = FontStyle.Bold;

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
