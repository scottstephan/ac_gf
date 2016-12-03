using UnityEngine;
using System.Collections;

public class u_backArrowManager : MonoBehaviour {
    public enum backArrowDirection
    {
        sp_catSelectToTitle,
        mp_lobbyToTitle,
        mp_friendPanelToLobby,
        mp_catSelectToLobby,
        highScoreToMenu,
        mainRoundToMenu,
        skip_mainRoundToScore,
        tutorialToCatSelect,
        scoreViewToMPLobby
    }

    public backArrowDirection myDirection;
    public GameObject masterCanvas;

    public void Start()
    {
        masterCanvas = GameObject.Find("MASTERCANVAS");
        if (myDirection == backArrowDirection.scoreViewToMPLobby && !appManager.curLiveGame.isMPGame)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }

    public void OnClick()
    {
        
            switch (myDirection)
            {
                case (backArrowDirection.sp_catSelectToTitle):
                    if (!appManager.curLiveGame.isMPGame)
                        m_phaseManager.instance.backUpPhase(m_phaseManager.phases.titleScreen);
                    else
                        m_phaseManager.instance.backUpPhase(m_phaseManager.phases.MPLobby);
                    break;
                case (backArrowDirection.mp_lobbyToTitle):
                    m_phaseManager.instance.backUpPhase(m_phaseManager.phases.titleScreen);
                    break;
                case (backArrowDirection.mp_friendPanelToLobby):
                    m_phaseManager.instance.backUpPhase(m_phaseManager.phases.MPLobby);
                    break;
                case (backArrowDirection.mp_catSelectToLobby):
                    //I need to know if this is MP or SP!
                    m_phaseManager.instance.backUpPhase(m_phaseManager.phases.MPLobby);
                    break;
                case (backArrowDirection.highScoreToMenu):
                    m_phaseManager.instance.changePhase(m_phaseManager.phases.highScoreToMenu);
                    break;
                case (backArrowDirection.mainRoundToMenu):
                    m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundToMenu);
                    break;
                case (backArrowDirection.skip_mainRoundToScore):
                    m_gameManager.instance.quitGame();
                    break;
                case (backArrowDirection.tutorialToCatSelect):
                if (!appManager.curLiveGame.isMPGame)
                    m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.tutorialToCatselect);
                else
                    m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.tutorialToMPLobby);
                     break;
                case (backArrowDirection.scoreViewToMPLobby):
                    if (m_phaseManager.instance.previousPhase == m_phaseManager.phases.MPLobby)
                    {
                        gameObject.SetActive(false);
                        m_phaseManager.instance.changePhase(m_phaseManager.phases.scoreCompToMPLobby);
                    }
                    
                    break;
         }
        
    }

     void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(m_phaseManager.instance.thisPhase == m_phaseManager.phases.scoreComp)
            {
                m_phaseManager.instance.changePhase(m_phaseManager.phases.titleScreen);
            }

            if (gameObject.transform.parent.position.x == masterCanvas.transform.position.x) //i.e., hey, we're active panel
                OnClick(); //For Android back button

        }
    }
}
