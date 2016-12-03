using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class obj_backToMenuBtn : MonoBehaviour {

    public Text buttonText;

	// Use this for initialization
	void Start () {
	
	}

    public void setText()
    {
        if(appManager.curGameStatus != appManager.E_lobbyGameStatus.init_viewScore)
        {
            buttonText.text = "Back to Menu";
        }
        else
        {
            buttonText.text = "Back to Games List";
        }
    }

    public void onButtonClick()
    {
        if (appManager.curGameStatus != appManager.E_lobbyGameStatus.init_viewScore)
        {
            m_phaseManager.instance.changePhase(m_phaseManager.phases.titleScreen);
        }
        else
        {
            m_phaseManager.instance.changePhase(m_phaseManager.phases.scoreCompToMPLobby);
        }
        appManager.flushReferences();
    }
}
