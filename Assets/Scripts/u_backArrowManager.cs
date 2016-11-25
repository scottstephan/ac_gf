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
        mainRoundToMenu
    }

    public backArrowDirection myDirection;
    public GameObject masterCanvas;

    public void Start()
    {
        masterCanvas = GameObject.Find("MASTERCANVAS");
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
            }
        
    }

     void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameObject.transform.parent.position.x == masterCanvas.transform.position.x) //i.e., hey, we're active panel
                OnClick(); //For Android back button
        }
    }
}
