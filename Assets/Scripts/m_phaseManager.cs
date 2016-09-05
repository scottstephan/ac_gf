using UnityEngine;
using System.Collections;

public class m_phaseManager : MonoBehaviour {
    public static m_phaseManager instance = null;

    public enum phases
    {
        loadScreen,
        titleScreen,
        MPLobby,
        categorySelect,
        mainRoundSP,
        mainRoundMP,
        scoreComp
    }

    public phases thisPhase = phases.loadScreen;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    // Use this for initialization
    void Start () {
       
	}

    public void changePhase(phases nextPhase)
    {
        switch (nextPhase)
        {
            case phases.loadScreen:
                break;
            case phases.titleScreen:
                m_phaseManager.instance.transitionToTitleScreen();
                break;
            case phases.MPLobby:
                m_phaseManager.instance.transitionToMPLobby();
                break;
            case phases.categorySelect:
                m_phaseManager.instance.transitionToCatSelect();
                break;
            case phases.mainRoundSP:
                break;
            case phases.mainRoundMP:
                break;
            case phases.scoreComp:
                break;
        }
    }

    private void transitionToLoadScreen()
    {

    }

    private void transitionToTitleScreen()
    {
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.loadingToMenu);
        m_loadPanelManager.instance.deactivateLoadPanel();
    }

    private void transitionToMPLobby()
    {
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.menuToMPLobby);
        m_MPLobby_Matchmake.instance.init_MPLobby();
    }

    private void transitionToCatSelect()
    { ///Need to know active panel- Menu to cat or MP to cat?

    }

    private void transitionToSP()
    {

    }

    private void transitionToMP()
    {

    }

    private void transitionToScoreComp()
    {

    }


}
