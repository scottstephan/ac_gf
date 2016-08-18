using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TEST_loadPlayerButton : MonoBehaviour {
    public Text idStatus;
	// Use this for initialization
	void Start () {
	    idStatus.text = m_prefsDataManager.getPlayerIDPref();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onLoadClick()
    {
        acDBHelper.instance.loadPlayerFromDynamoViaID(m_prefsDataManager.getPlayerIDPref());
    }
}
