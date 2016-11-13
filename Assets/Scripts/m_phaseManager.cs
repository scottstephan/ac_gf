using UnityEngine;
using System.Collections;
/// <summary>
/// This class handles high-level logic for swapping between panels
/// i.e.: flushing refs, setting important variables etc. It does NOT handle look/eel- That's m_panelManager
/// </summary>
public class m_phaseManager : MonoBehaviour {
    public static m_phaseManager instance = null;

    public enum phases
    {
        loadScreen,
        titleScreen,
        MPLobby,
        categorySelectSP,
        categorySelectMP,
        tutorial,
        mainRoundSP,
        mainRoundMP,
        scoreComp,
    }

    public phases thisPhase = phases.loadScreen;
    public phases previousPhase;

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
        previousPhase = thisPhase;
        switch (nextPhase)
        {
            case phases.loadScreen:
                thisPhase = phases.loadScreen;
                break;
            case phases.titleScreen:
                thisPhase = phases.titleScreen;
                m_phaseManager.instance.transitionToTitleScreen();
                break;
            case phases.MPLobby:
                thisPhase = phases.MPLobby;
                m_phaseManager.instance.transitionToMPLobby();
                break;
            case phases.categorySelectSP:
                thisPhase = phases.categorySelectSP;
                m_phaseManager.instance.transitionToCatSelectSP();
                break;
            case phases.categorySelectMP:
                thisPhase = phases.categorySelectMP;
                m_phaseManager.instance.transitionToCatSelectMP();
                break;
            case phases.tutorial:
              //  thisPhase = phases.tutorial; //Don't set the phase- The MP relies on knowing whether the last relevant panel phase as category or MP lobby. No sense in fucking with that.
                m_phaseManager.instance.transitionToTutorial();
                break;
            case phases.mainRoundSP:
                thisPhase = phases.mainRoundSP;
                m_phaseManager.instance.transitionTutorialToSP();
                break;
            case phases.mainRoundMP:
                thisPhase = phases.mainRoundMP;
                m_phaseManager.instance.transitionTutorialToMP();
                break;
            case phases.scoreComp:
                thisPhase = phases.scoreComp;
                m_phaseManager.instance.transitionToScoreComp();
                break;
        }
    }

    private void transitionToLoadScreen()
    {

    }

    private void transitionToTitleScreen()
    {
        if (previousPhase == phases.loadScreen)
        {
            m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.loadingToMenu);
            m_loadPanelManager.instance.deactivateLoadPanel();
        }
        else if (previousPhase == phases.scoreComp)
        {
            m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.scoreCompToMainMenu);
        }
    }

    private void transitionToMPLobby()
    {
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.menuToMPLobby);
        m_MPLobby_Matchmake.instance.init_MPLobby();
    }

    private void transitionToCatSelectMP()
    { 
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.MPLobbyToCatSelect);
        m_categorySelectionManager.instance.initCategoryPhase();
    }

    private void transitionToCatSelectSP()
    {
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.menuToCatSelect);
        m_categorySelectionManager.instance.initCategoryPhase();
    }

    private void transitionToTutorial()
    {
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.catSelectToTutorial);

    }

    private void transitionTutorialToSP()
    {
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.TutorialToMainRound);
        m_gameManager.instance.init(false);
    }

    private void transitionTutorialToMP()
    {
        if (previousPhase == phases.categorySelectMP)
        {
            m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.catSelectToMainRound);
        }
        else if (previousPhase == phases.MPLobby)
        {
            m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.MPLobbyToMainRound);
        }

        m_gameManager.instance.init(true);
    }

    private void transitionToSP()
    {
        
            m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.catSelectToMainRound);
            m_gameManager.instance.init(false);
        
    }

    private void transitionToMP()
    {
        if (previousPhase == phases.categorySelectMP)
        {
            m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.catSelectToMainRound);
        }
        else if (previousPhase == phases.MPLobby)
        {
            m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.MPLobbyToMainRound);
        }

        m_gameManager.instance.init(true);

    }

    private void transitionToScoreComp()
    {
        if (previousPhase == phases.MPLobby)
        {
            m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.MPLobbyToScoreComp);//This loops back to menu- Maybe move it back to MPLObby?
        }
        else if (previousPhase == phases.mainRoundMP || previousPhase == phases.mainRoundSP)
        {
            m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.mainRoundToScoreComp);
        }

        m_scoreCompManager.instance.initSC();
    }


}
