using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Assets.autoCompete.games;
using UnityEngine.SceneManagement;
using DDBHelper;

public class ui_existingGameButton : MonoBehaviour {
    public Text ButtonText;
    public string gameID; //This comes from m_MP_Lobby which gets it from the devicePlayer's entry in player_games

    public appManager.playerRoles devicePlayerRole;
    public entity_games thisGame;
    public GameObject parentCanvasTransform;

    public enum lobbyGameStatus
    {
        init_viewScore,
        init_playGame,
        init_viewFinal,
        challenged_playGame,
        challenged_waitForInit
    }

    public lobbyGameStatus thisGameStatus;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setUpButton()
    {
        gameObject.transform.SetParent(parentCanvasTransform.transform);
        ButtonText.text = thisGame.player1_name + " vs. " + thisGame.player2_name;
    }

    public void onButtonClick()
    {
        //If info is loaded etc....
        appManager.setCurGame(thisGame, devicePlayerRole);

        //IF role is init AND has finished AND NOT has seen, go to scoreComp....
        //Else if role is challenger AND
        appManager.loadScene(appManager.sceneNames.mainRound);
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
        //if my role is initiated and p1 is not fin, I need to play that game - RARE (On App Quit, maybe mark score as 0??)
        //If my role is initiated p1 is done and p2 is not done,  I need to wait, but I can view score (Assign game to appManager)
        //If my roll is initaed and we're both done and p1View is not true, I need to view score, p1View becomes true, game goes to games_dead
        if (devicePlayerRole == appManager.playerRoles.intiated)
        {
            if(thisGame.p1_Fin && thisGame.p2_Fin && !thisGame.p1HasViewedResult && thisGame.p2HasViewedResult) {
                thisGameStatus = lobbyGameStatus.init_viewFinal;
            }
            else if(thisGame.p1_Fin && !thisGame.p2_Fin)
            {
                thisGameStatus = lobbyGameStatus.init_viewScore;
            }
            else if(!thisGame.p1_Fin && thisGame.p2_Fin)
            {
                thisGameStatus = lobbyGameStatus.init_playGame;
            }
        }
        //If my role is challenged and p2 is not finished, I need to play
        //If my role is challenged and p1 is not finished, I need to wait
        //If my role is challenged and p1 is finished and p2 is finished, I can vew score, p1View becomes true, game goes to games_dead
        else if (devicePlayerRole == appManager.playerRoles.challenged)
        {
            if(thisGame.p1_Fin && !thisGame.p2_Fin)
            {
                thisGameStatus = lobbyGameStatus.challenged_playGame;
            }
            else if(thisGame.p2_Fin && !thisGame.p1_Fin)
            {
                thisGameStatus = lobbyGameStatus.challenged_waitForInit;
            }
        }
        
       

        setUpButton();
    }
}
