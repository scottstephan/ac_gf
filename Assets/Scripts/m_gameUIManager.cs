using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class m_gameUIManager : MonoBehaviour {

    public static m_gameUIManager instance = null;

    public enum E_uiManagerStatus
    {
        uiLayoutIncomplete,
        uiLayoutComplete
    }
 
    public E_uiManagerStatus currentUIManagerStatus = E_uiManagerStatus.uiLayoutIncomplete;

    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public static void layoutAnswers()
    {
        Debug.Log("H:" + gameManager.masterUICanvas.GetComponent<RectTransform>().rect.height);
        Debug.Log("W:"  + gameManager.masterUICanvas.GetComponent<RectTransform>().rect.width);
        //get answer list from GM
        for(int i = 0; i < gameManager.roundAnswers.Count; i++)
        {
            GameObject tA;
            tA = gameManager.roundAnswers[i];
            //TO-DO: do resolution maths
            //iterate over answers, position them
            tA.transform.SetParent(gameManager.answerLayoutGrid.transform);
            //tA.GetComponent<obj_Answer>().setAnswerPosition(new Vector3(200, 50 * i, 0));
        }
    }

    public void setInputFieldPosition()
    {
        
    }
}
