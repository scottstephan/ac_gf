using UnityEngine;
using System.Collections;

public class TEST_registerPlayerButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onRegisterButtonClick()
    {
        bool isNewPlayer = acDBHelper.instance.checkIfPlayerExistsByID(m_prefsDataManager.getPlayerIDPref());
    }
}
