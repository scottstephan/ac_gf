using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Amazon.DynamoDBv2.DataModel;
using DDBHelper;
using Assets.autoCompete.players;
using Assets.autoCompete.games;
using Amazon.DynamoDBv2.Model;

public class m_MPLobby_Matchmake : MonoBehaviour {
    public static m_MPLobby_Matchmake instance = null;

    public Text txt_curPlayerReadout;
    public GameObject opponentButton;
    public GameObject opponentListParentGrid;
    public GameObject playerListPanel;

    public Vector2 listStartPos;
    public float listYPadding;
    int listIndex;

    List<entity_players> allPlayers = new List<entity_players>();
    public event DDBScanResponseDelegate OnScanComplete;
    public event DDBQueryHashKeyOnlyDelegate<appManager.playerGameID> OnP1GameFetchComplete;

    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    void Start () {
        txt_curPlayerReadout.text = "You: " + appManager.devicePlayer.playerName;
        OnScanComplete += OnPlayerScanComplete;
       // OnP1GameFetchComplete += allP1GameQueryComplete;

        getAndListAllPlayers();
        getAllP1Games();
	}

    void getAndListAllPlayers() {
        Debug.Log("***SCANNING FOR ALL PLAYERS***");
        DBWorker.Instance.ScanTable("players", OnPlayerScanComplete);  
    }

    static void OnPlayerScanComplete(List<Dictionary<string, AttributeValue>> response, GameObject obj, string nextMethod, Exception e = null)
    {
        Debug.Log("***LISTING ALL PLAYERS FROM SCAN***");
        List<entity_players> allPlayers = new List<entity_players>();

        foreach (Dictionary<string, AttributeValue> d in response) //foreach is bugged- Swap it ot when we change this
        {
            entity_players tP = new entity_players();
            tP.playerName = d["playerName"].S;
            tP.playerID = d["playerID"].S;

            allPlayers.Add(tP);
        }

        m_MPLobby_Matchmake.instance.createPlayerListInUI(allPlayers);
    }

    void createPlayerListInUI(List<entity_players> allPlayers)
    {
        for (int i = 0; i < allPlayers.Count; i++)
        {
            GameObject tButton = Instantiate(opponentButton);
            tButton.transform.SetParent(opponentListParentGrid.transform);

            ui_opponentButtonManager tManager = tButton.GetComponent<ui_opponentButtonManager>();
            tManager.opEntity = allPlayers[i];
            tManager.setUpButton();
        }
    }

    void getAllP1Games()
    {
        Debug.Log("***QUERING ALL GAMES INVOLVING P1***");
        List<string> attToReturn = new List<string>();
        attToReturn.Add("playerID");
        attToReturn.Add("gameID");
        attToReturn.Add("role");

        DBWorker.Instance.QueryHashKeyObject<appManager.playerGameID>(appManager.devicePlayer.playerID, attToReturn, allP1GameQueryComplete,true);
    }

    static void allP1GameQueryComplete(List<appManager.playerGameID> response, Exception e = null)
    {
        Debug.Log("ALL P1 GAMES LOADED");
        List<appManager.playerGameID> pgID = new List<appManager.playerGameID>();
        for (int i = 0; i < response.Count; i++)
        {
            appManager.playerGameID tPGID = new appManager.playerGameID();
            tPGID.playerID = response[i].playerID;
            tPGID.gameID = response[i].gameID;
            tPGID.role = response[i].role;

            Debug.Log(tPGID.playerID + " is a " + tPGID.role + " in " + tPGID.gameID);

            pgID.Add(tPGID); //These ALL of the games this user is currently involved in
        }
    }

    /*
    static void allP1GameQueryComplete(List<Dictionary<string, AttributeValue>> response, GameObject obj, string nextMethod, Exception e = null)
    {
        List<appManager.playerGameID> pgID = new List<appManager.playerGameID>();
        for(int i = 0; i < response.Count; i++)
        {
            appManager.playerGameID tPGID = new appManager.playerGameID();
            tPGID.playerID = response[i]["playerID"].S;
            tPGID.gameID = response[i]["playerID"].S;
            tPGID.role = response[i]["role"].S;

            Debug.Log(tPGID.playerID + " is a " + tPGID.role + " in " + tPGID.gameID);

            pgID.Add(tPGID); //These ALL of the games this user is currently involved in
        }
    }*/

    void getAllGamesP1IsInitiated()
    {
        
    }

    void getAllGamesP1IsChallengedIn()
    {

    }
}
