using UnityEngine;
using System.Collections;

public class u_debugManager : MonoBehaviour {
    public bool useJSONDebug = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (useJSONDebug)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Debug.Log("+++READING CATEGORIES+++");
                u_acJsonUtility.instance.discoverCategories(false);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("+++PARSING JSON+++");
                u_acJsonUtility.instance.readJson();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("GETTNING QDBINFO");
                u_acJsonUtility.instance.returnCurQDBObject();
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                Debug.Log("CHECKIN CATEGORIES DIRECTORY");
                u_acJsonUtility.instance.getAllCategoryRawJson();
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                Debug.Log("CREATING TEST CAT INFO OBJECT");
                u_acJsonUtility.instance.createCatInfoFile();
            }
            else if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("SHOWIN CAT INFO FROM LOCAL DIR");
                u_acJsonUtility.instance.discoverAllCategoryUnlockInfo();
            }
        }
	}
}
