using UnityEngine;
using System.Collections;

public class playAgainstFriendButtonManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onButtonClick()
    {
        m_panelManager.instance.animateUIPanelByPhase(m_panelManager.uiPanelTransitions.playerInputToCenter);
        m_loadPanelManager.instance.activateLoadPanel();
    }
}
