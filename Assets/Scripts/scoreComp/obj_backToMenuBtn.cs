using UnityEngine;
using System.Collections;

public class obj_backToMenuBtn : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onButtonClick()
    {
        appManager.flushReferences();
        m_phaseManager.instance.changePhase(m_phaseManager.phases.titleScreen);
       // appManager.loadScene(appManager.sceneNames.title);
    }
}
