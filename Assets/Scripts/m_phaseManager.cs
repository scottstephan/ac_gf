﻿using UnityEngine;
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
        toSettings,
        fromSettings,
        friendPanel,
        toHighScore,
        highScoreToMenu,
        mainRoundToMenu,
        scoreCompToMPLobby,
        catSelectToFriendPanel
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
            case phases.MPLobby:;
                Debug.Log("Phase is now MP lobby!");
                thisPhase = phases.MPLobby;
                m_phaseManager.instance.transitionToMPLobby();
                break;
            case phases.friendPanel:
                thisPhase = phases.friendPanel;
                m_phaseManager.instance.transitionToFriendPanel();
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
            case phases.toHighScore:
                thisPhase = phases.toHighScore;
                m_phaseManager.instance.transitionToHighScore();
                break;
            case phases.highScoreToMenu:
                thisPhase = phases.titleScreen;
                m_phaseManager.instance.transitionToMenuFromHS();
                break;
            case phases.mainRoundToMenu:
                thisPhase = phases.titleScreen;
                m_phaseManager.instance.transitionToMenuFromMainRound();
                break;
            case phases.scoreCompToMPLobby:
                thisPhase = phases.MPLobby;
                m_phaseManager.instance.transitionToMPLObbyFromScoreComp();
                break;
            case phases.catSelectToFriendPanel:
                thisPhase = phases.friendPanel;
                m_phaseManager.instance.transitionToFriendPanelFromCatSelect();
                break;
        }
    }

    public void backUpPhase(phases backToPhase)
    {
        previousPhase = thisPhase;
        thisPhase = backToPhase;

        if(backToPhase == phases.MPLobby)
        {
            if(previousPhase == phases.categorySelectMP)
            {
                m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.mp_CatSelectToLobby);
            }
            else if(previousPhase == phases.friendPanel)
            {
                m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.mp_FriendListToTitle);
            }
        }
        else if(backToPhase == phases.titleScreen)
        {
            if(previousPhase == phases.categorySelectSP)
            {
                m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.sp_catSelectToTitle);
            }
            else if(previousPhase == phases.MPLobby)
            {
                m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.mp_LobbyToTitle);
            }
        }
    }

    public void transitionToMPLObbyFromScoreComp()
    {
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.scoreCompToMPLobby);
        m_MPLobby_Matchmake.instance.init_MPLobby();

    }

    public void transitionToFriendPanelFromCatSelect()
    {
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.mp_CatSelectToFriendList);

    }

    private void transitionToMenuFromMainRound()
    {
        m_gameManager.instance.pauseGame();
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.mainRoundToMenu);
        //Stop game
    }

    private void transitionToMenuFromHS()
    {
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.highScoreToTitle);
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

    private void transitionToFriendPanel()
    {
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.mpLobbyToFriendPanel);
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
        {//You're starting a game
            m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.TutorialToMainRound);
            m_gameManager.instance.init(false);
        }
        else if (previousPhase == phases.MPLobby)
        {//You're finishing a game
            m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.MPLobbyToMainRound);
            m_gameManager.instance.init(true);
        }

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

    private void transitionToHighScore()
    {
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.titleToHighScore);
        highScorePanelManager.instance.createHighScoreList();
    }

    private void transitionToSettings()
    {
        switch (previousPhase)
        {
            case (phases.titleScreen):
                break;
            case (phases.MPLobby):
                break;
            case (phases.categorySelectSP):
                break;
            case (phases.categorySelectMP):
                break;
        }    
    }

    private void transitionFromSettings()
    {

    }


}
