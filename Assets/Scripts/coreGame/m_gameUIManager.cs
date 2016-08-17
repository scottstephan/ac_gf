using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class m_gameUIManager : MonoBehaviour {

    public static m_gameUIManager instance = null;
    public static float answerCellSize;
    public static float canvasWidth;
    public static float canvasHeight;

    public static float operableGridSpace_h;
    public static float operableGridSpace_w;

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

    void Start()
    {
        canvasHeight = gameManager.masterUICanvas.GetComponent<RectTransform>().rect.height;
        canvasWidth = gameManager.masterUICanvas.GetComponent<RectTransform>().rect.width;

        operableGridSpace_h = canvasHeight * .6f;
        operableGridSpace_w = canvasWidth * .5f;
    }

    public static void layoutAnswersDynamic()
    {
        Debug.Log("H:" + canvasHeight);
        Debug.Log("W:"  + canvasWidth);
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
