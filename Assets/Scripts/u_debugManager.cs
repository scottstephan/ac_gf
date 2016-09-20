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
                u_acJsonUtility.instance.discoverCategories();
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                Debug.Log("+++PARSIN JSON+++");
                u_acJsonUtility.instance.readJson();
            }
        }
	}
}
