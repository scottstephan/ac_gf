using UnityEngine;
using System.Collections;

public class m_phaseManager : MonoBehaviour {
    public static m_phaseManager instance = null;

    public enum phases
    {
        loadScreen,
        titleScreen,
        MPLobby,
        categorySelectSP,
        categorySelectMP,
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
            case phases.categorySelectSP:
                m_phaseManager.instance.transitionToCatSelectSP();
                break;
            case phases.categorySelectMP:
                m_phaseManager.instance.transitionToCatSelectMP();
                break;
            case phases.mainRoundSP:
                break;
            case phases.mainRoundMP:
                m_phaseManager.instance.transitionToMP();
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

    private void transitionToCatSelectMP()
    { ///Need to know active panel- Menu to cat or MP to cat?
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.MPLobbyToCatSelect);
    }

    private void transitionToCatSelectSP()
    {

    }


    private void transitionToSP()
    {
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.catSelectToMainRound);

    }

    private void transitionToMP()
    {
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.catSelectToMainRound);
        gameManager.instance.StartCoroutine("InitGame");
    }

    private void transitionToScoreComp()
    {

    }


}
