using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class highScorePanelManager : MonoBehaviour {
    public static highScorePanelManager instance = null;
    public GameObject listParent;
    public GameObject highScoreObject;
    public GameObject hsPanel;
    List<u_acJsonUtility.categoryHighScore> allCatHS = new List<u_acJsonUtility.categoryHighScore>();

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            createHighScoreList();
        }
    }

	public void createHighScoreList()
    {
        allCatHS = u_acJsonUtility.instance.returnAllCategoryHighScoreObjects();
        for(int i =0; i < allCatHS.Count; ++i)
        {
            GameObject tHS = Instantiate(highScoreObject);
            tHS.transform.SetParent(listParent.transform);

            tHS.GetComponent<obj_highScoreList>().setupReadout(allCatHS[i].categoryDisplayName, allCatHS[i].categoryHighscore);

        }
    }

    public void showHSPanel()
    {
        hsPanel.transform.position = new Vector3(0, 0, 0);
    }
}
