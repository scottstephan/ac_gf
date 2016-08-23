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

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setUpButton()
    {
        ButtonText.text = appManager.curLiveGame.player1_name + " vs. " + appManager.curLiveGame.player2_name;
    }

    void onButtonClick()
    {
        //If info is loaded etc....
    }

    public void loadGameEntity(string gameID)
    {
        Debug.Log("***LOAD GAME ENTITY FOR " + gameID + "***");
        entity_games tG = new entity_games();
        tG.gameID = gameID; 
        DBWorker.Instance.Load<entity_games>(tG, gameLoadComplete);
    }

    void gameLoadComplete(entity_games response, GameObject obj, string nextMethod, Exception e = null)
    {
        if(e != null)
        {
            DBTools.PrintException("gameLoadComplete", e);
        }
        Debug.Log("***GAME ENTITY LOAD COMPLETE***");
        appManager.curLiveGame = response;
        setUpButton();
    }
}
