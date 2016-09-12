using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Assets.autoCompete.games;
using UnityEngine.SceneManagement;
using DDBHelper;

public class ui_existingGameButton : MonoBehaviour {
    public Text ButtonText;
    public Button uiButtonManager;
    public string gameID; //This comes from m_MP_Lobby which gets it from the devicePlayer's entry in player_games

    public appManager.playerRoles devicePlayerRole;
    public entity_games thisGame;
    public GameObject parentCanvasTransform;

    public appManager.E_lobbyGameStatus thisGameStatus;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void setColorBlock()
    {
        ColorBlock tCB = uiButtonManager.colors;
        tCB.normalColor = m_colorPaletteManager.instance.buttonColorPalette.returnRandomColor();
        uiButtonManager.colors = tCB;
    }

    public void setUpButton()
    {
        setColorBlock();
        string btext = "";
        if (thisGameStatus == appManager.E_lobbyGameStatus.init_viewScore)
            btext = "Waiting for " + thisGame.player2_name + " to finish! Tap to see your score";
        else if (thisGameStatus == appManager.E_lobbyGameStatus.init_viewFinal)
           btext = thisGame.player1_name + " vs. " + thisGame.player2_name +" ! View the results!";
        else if (thisGameStatus == appManager.E_lobbyGameStatus.challenged_playGame)
            btext = thisGame.player1_name + " vs. " + thisGame.player2_name +"! Play now!";
        gameObject.transform.SetParent(m_MPLobby_Matchmake.instance.fullGameListParentGrid.transform);

        ButtonText.text = btext;
    }

    public void onButtonClick()
    {
        //If info is loaded etc....
        appManager.setCurGame(thisGame, devicePlayerRole);

        //IF role is init AND has finished AND NOT has seen, go to scoreComp....
        if (thisGameStatus == appManager.E_lobbyGameStatus.init_viewScore)
            m_phaseManager.instance.changePhase(m_phaseManager.phases.scoreComp);
        //IF role is init annd P2 has finished, view final score
        else if (thisGameStatus == appManager.E_lobbyGameStatus.init_viewFinal)
            m_phaseManager.instance.changePhase(m_phaseManager.phases.scoreComp);
        //Else if role is challenger AND
        else if (thisGameStatus == appManager.E_lobbyGameStatus.challenged_playGame)
            m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundMP);

    }

    public void loadGameEntity(string gameID)
    {
        Debug.Log("***LOAD GAME ENTITY FOR " + gameID + "***");
        entity_games tG = new entity_games();
        tG.gameID = gameID;
        tG.gameState = appManager.E_storedGameStates.unstarted.ToString(); //In this case, all games from the lobby are MP games
        DBWorker.Instance.Load<entity_games>(tG, gameLoadComplete);
    }

    void gameLoadComplete(entity_games response, GameObject obj, string nextMethod, Exception e = null)
    {
        if(e != null)
        {
            DBTools.PrintException("gameLoadComplete", e);
        }
        Debug.Log("***GAME ENTITY LOAD COMPLETE***");
        thisGame = response;

        determineGameState();
    }

    void determineGameState()
    {
        // Some sort logic:
        //If my role is init && p2 is not finished, I can view score, but am waiting
        //If my role is init && p2 IS finished, I need to view final comp and mark me as done
        if (devicePlayerRole == appManager.playerRoles.intiated)
        {
            thisGameStatus = thisGame.p2_Fin ? appManager.E_lobbyGameStatus.init_viewFinal : appManager.E_lobbyGameStatus.init_viewScore;

        }
        //If my role is challenged && p2 is not finished, I need to play
        
        else if (devicePlayerRole == appManager.playerRoles.challenged)
        {
            if (!thisGame.p2_Fin) thisGameStatus = appManager.E_lobbyGameStatus.challenged_playGame;
        }

        appManager.curGameStatus = thisGameStatus;
        
        setUpButton();
    }
}
