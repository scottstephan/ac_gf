using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class highScorePanelManager : MonoBehaviour {
    public static highScorePanelManager instance = null;
    public GameObject listParent;
    public GameObject highScoreObject;
    public GameObject hsPanel;
    public GameObject hsHeader;
    List<u_acJsonUtility.categoryHighScore> allCatHS = new List<u_acJsonUtility.categoryHighScore>();

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void refreshList()
    {
        foreach (Transform t in listParent.transform)
            Destroy(t.gameObject);
    }

	public void createHighScoreList()
    {
        refreshList();

        allCatHS = u_acJsonUtility.instance.returnAllCategoryHighScoreObjects();
        List<u_acJsonUtility.categoryUnlockInfo> catUnlockInfo = u_acJsonUtility.instance.discoverAllCategoryUnlockInfo();
        List<u_acJsonUtility.categoryHighScore> unlockedCatHS = new List<u_acJsonUtility.categoryHighScore>();

        //Clean the HS list so that we only get unlocked categories
        for(int i = 0; i < allCatHS.Count; ++i)
        {
            for(int j = 0; j < catUnlockInfo.Count; ++j)
            {
                if(catUnlockInfo[j].categoryName == allCatHS[i].categoryName)
                {
                    if(catUnlockInfo[j].unlockStatus == "unlocked")
                    {
                        unlockedCatHS.Add(allCatHS[i]);
                        break;
                    }
                    else if(catUnlockInfo[j].unlockStatus == "locked")
                    {
                        break;
                    }
                }
            }
        }
        //Sort the list
        unlockedCatHS.Sort((x, y) => x.categoryID.CompareTo(y.categoryID));

        GameObject hsHeaderObj = Instantiate(hsHeader);
        hsHeaderObj.transform.SetParent(listParent.transform);

        for (int i =0; i < unlockedCatHS.Count; ++i)
       {
           GameObject tHS = Instantiate(highScoreObject);
           tHS.transform.SetParent(listParent.transform);

           tHS.GetComponent<obj_highScoreList>().setupReadout(unlockedCatHS[i].categoryDisplayName, unlockedCatHS[i].categoryHighscore, unlockedCatHS[i].categoryColor);

       }
    }

    public void showHSPanel()
    {
        hsPanel.transform.position = new Vector3(0, 0, 0);
    }
}
