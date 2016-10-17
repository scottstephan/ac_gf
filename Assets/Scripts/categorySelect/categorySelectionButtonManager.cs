using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class categorySelectionButtonManager : MonoBehaviour
{
    public string categoryName;
    public string categoryId;
    public Text catButtonText;
    bool isLocked = false;

    public void categorySelected()
    {
        if (appManager.curLiveGame.isMPGame)
            m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundMP);
        else
            m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundSP);
    }

    public void buttonClicked()
    {
        if (!isLocked) { 
            m_gameManager.instance.setCurrentSelectCategory(categoryName);
            m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundSP);
        }
        else
        {//IAP path
            unlockCategory(); //FOR TESTING ONLY
            
        }
    }

    public void setUpButton()
    {
        ColorBlock bColor = new ColorBlock();
        bColor.colorMultiplier = 1;
        bColor.normalColor = Color.green;
        bColor.highlightedColor = Color.blue;
        gameObject.GetComponent<Button>().colors = bColor;

        string dispCatName = categoryName;
        dispCatName = char.ToUpper(dispCatName[0]) + dispCatName.Substring(1);
        catButtonText.text = dispCatName;
    }

    public void lockButton()
    {
        isLocked = true;
        ColorBlock bColor = new ColorBlock();
        bColor.colorMultiplier = 1;
        bColor.normalColor = Color.red;
        bColor.highlightedColor = Color.blue;

        gameObject.GetComponent<Button>().colors = bColor;
        string dispCatName = categoryName+ ":: LOCKED";
        dispCatName = char.ToUpper(dispCatName[0]) + dispCatName.Substring(1);
        catButtonText.text = dispCatName;
    }

    public void unlockCategory()
    {
        u_acJsonUtility.instance.findAndUnlockCategory(categoryName);
        setUpButton();
    }
}
