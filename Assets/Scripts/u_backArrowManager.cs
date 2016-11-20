using UnityEngine;
using System.Collections;

public class u_backArrowManager : MonoBehaviour {
    public enum backArrowDirection
    {
        sp_catSelectToTitle,
        mp_lobbyToTitle,
        mp_friendPanelToLobby,
        mp_catSelectToLobby
    }

    public backArrowDirection myDirection;

    public void OnClick()
    {
        switch (myDirection)
        {
            case (backArrowDirection.sp_catSelectToTitle):
                if(!appManager.curLiveGame.isMPGame)
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
        }
    }

     void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClick(); //For Android back button
        }
    }
}
