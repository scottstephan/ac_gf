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
            buttonText.text = "Main Menu";
        }
        else
        {
            buttonText.text = "Game Lobby";
        }
    }

    public void onButtonClick()
    {
        if (appManager.curGameStatus != appManager.E_lobbyGameStatus.init_viewScore && !appManager.curLiveGame.isMPGame)
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
