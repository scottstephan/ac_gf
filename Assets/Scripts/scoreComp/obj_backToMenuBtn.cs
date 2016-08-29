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

        appManager.loadScene(appManager.sceneNames.title);
    }
}
