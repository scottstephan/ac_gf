using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TEST_registerPlayerButton : MonoBehaviour {
    public InputField playerNameInput;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onRegisterButtonClick()
    {
        acDBHelper.instance.checkIfPlayerExistsByID(m_prefsDataManager.getPlayerIDPref(), playerNameInput.text);
    }
}
