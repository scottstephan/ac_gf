using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class m_categorySelectionManager : MonoBehaviour {
    public static m_categorySelectionManager instance = null;
    public GameObject categoryButton;
    public GameObject shopButton;
    public GameObject parentCategoryListGrid;

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

        catNames = u_acJsonUtility.instance.discoverCategories();
        catUnlockStatus = u_acJsonUtility.instance.discoverAllCategoryUnlockInfo();
        for(int i = 0; i < catNames.Count; ++i)
        {//TO-DO: Don't even instantiate the buton unless it's unlocked!
            GameObject tButton = Instantiate(categoryButton);
            tButton.transform.SetParent(parentCategoryListGrid.transform);

            categorySelectionButtonManager tManager = tButton.GetComponent<categorySelectionButtonManager>();
            tManager.categoryName = catNames[i];
            tManager.categoryId = "NOTUSINGFORNOW";

            for(int j = 0; j < catUnlockStatus.Count; ++j)
            {
                Debug.Log("Trying Match: " + catUnlockStatus[j] + " :: " + catNames[i]);
                if(catUnlockStatus[j].categoryName == catNames[i])
                {
                    if(catUnlockStatus[j].unlockStatus == "unlocked")
                    {
                        tManager.setUpButton();
                        catUnlockStatus.Remove(catUnlockStatus[j]);
                        break;
                    }
                    else if(catUnlockStatus[j].unlockStatus == "locked")
                    { //Commented out the bottom- Don't list it! 
                        Destroy(tButton);
                     /*   tManager.lockButton();
                        catUnlockStatus.Remove(catUnlockStatus[j]); */
                        break;
                    }
                    else
                    {
                        Debug.Log("UNKNOWN CATEGORY STATUS: " + catNames[i]);
                    }
                }
                else
                {
                    Debug.Log("CATEGORY STATUS DID NOT FIND MATCH WITH DIRECTORYLIST");
                }
            }
        }

        GameObject tSB = Instantiate(shopButton);
        tSB.transform.SetParent(parentCategoryListGrid.transform);
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
}
