﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class categorySelectionButtonManager : MonoBehaviour
{
    public string categoryName;
    public string categoryId;
    public Color categoryColor;
    public Texture2D categoryImage;
    public Text catButtonText;
    public u_acJsonUtility.acCat thisCat;
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
            //m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundSP);
            m_phaseManager.instance.changePhase(m_phaseManager.phases.tutorial);
        }
        else
        {//IAP path
            unlockCategory(); //FOR TESTING ONLY
        }
    }

    public void setUpButton()
    {
        thisCat = u_acJsonUtility.instance.loadCategoryData(categoryName);
        gameObject.GetComponent<Button>().colors = setColorBlock();

        string dispCatName = categoryName;
        dispCatName = char.ToUpper(dispCatName[0]) + dispCatName.Substring(1);
        catButtonText.text = dispCatName;
    }

    private ColorBlock setColorBlock()
    {
        ColorBlock tCB = new ColorBlock();
        
        if (ColorUtility.TryParseHtmlString(thisCat.categorColorValue, out categoryColor))
        {
            tCB.normalColor = categoryColor;
        }
        else
        {
            tCB.normalColor = Color.black;
        }

        tCB.highlightedColor = tCB.normalColor;
        tCB.pressedColor = tCB.normalColor;
        tCB.colorMultiplier = 1;
        return tCB;
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
