using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class m_categorySelectionManager : MonoBehaviour {
    public static m_categorySelectionManager instance = null;
    public GameObject categoryButton;
    public GameObject shopButton;
    public GameObject parentCategoryListGrid;
    public GameObject categoryListHeader;

    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void initCategoryPhase()
    {
        clearCategoryLayout();
        createCategoryLayout();
    }

    void clearCategoryLayout()
    {
        foreach (Transform t in parentCategoryListGrid.transform)
            Destroy(t.gameObject);
        
    }

    void createCategoryLayout()
    {
        List<string> catNames = new List<string>();
        List<u_acJsonUtility.categoryUnlockInfo> catUnlockStatus = new List<u_acJsonUtility.categoryUnlockInfo>();
        catNames.Clear();
        catUnlockStatus.Clear();

        catNames = u_acJsonUtility.instance.discoverCategories(false);
        catUnlockStatus = u_acJsonUtility.instance.discoverAllCategoryUnlockInfo();

        GameObject catHead = Instantiate(categoryListHeader);
        catHead.transform.SetParent(parentCategoryListGrid.transform, false);
        //0- The outcome of this process is that catUnlockStatus wll retain all of the UNLOCKED categories. 

        for (int i = 0; i < catNames.Count; ++i)
        {
            for (int j = 0; j < catUnlockStatus.Count; ++j)
            {
                Debug.Log("Trying Match: " + catUnlockStatus[j].categoryName + " :: " + catNames[i]);
                if(catUnlockStatus[j].categoryName == catNames[i])
                {
                    if(catUnlockStatus[j].unlockStatus == "unlocked")
                    {
                        Debug.Log(catNames[i] + " IS UNLOCKED");
                        break;
                    }
                    else if(catUnlockStatus[j].unlockStatus == "locked")
                    {
                        Debug.Log(catNames[i] + " IS LOCKED");
                        catUnlockStatus.Remove(catUnlockStatus[j]);
                        break;
                    }
                    else
                    {
                        Debug.Log("UNKNOWN CATEGORY STATUS: " + catNames[i]);
                    }
                }
            }
        }
        //Order the list
        Debug.Log("Going to sort list");
        catUnlockStatus.Sort((x, y) => x.categoryID.CompareTo(y.categoryID));
        Debug.Log("Done sorting list");
        //1 - Instantiate only the unlocked stuff 
        for (int i = 0; i < catUnlockStatus.Count; ++i)
        {
            GameObject tButton = Instantiate(categoryButton);
            tButton.transform.SetParent(parentCategoryListGrid.transform, false);

            categorySelectionButtonManager tManager = tButton.GetComponent<categorySelectionButtonManager>();
            tManager.categoryName = catUnlockStatus[i].categoryName;
            tManager.categoryId = catUnlockStatus[i].categoryID;
            tManager.setUpButton(catUnlockStatus[i].categoryName);
        }

        GameObject tSB = Instantiate(shopButton);
        tSB.transform.SetParent(parentCategoryListGrid.transform, false);
    }
    /// <summary>
    /// Loads and sets the 'current question' attribute for the game
    /// </summary>
    /// <param name="catName"></param>
    public void loadCategoryQuestion(string catName)
    {
        u_acJsonUtility.acQ tQ = new u_acJsonUtility.acQ();
        tQ = u_acJsonUtility.instance.loadRandomQuestionData(catName);
        appManager.currentQuestion = tQ; //COuld skip the above hullabaloo.
    }

    public void catImageWebLoadCallback()
    {

    }

}
