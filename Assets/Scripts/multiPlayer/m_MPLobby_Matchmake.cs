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

    public GameObject gamesInitiatedButton;
    public GameObject gameInitListParentGrid;
    public GameObject gamesInitListPanel;

    public GameObject gameChallengedButton;
    public GameObject gameChallengedParentGrid;
    public GameObject gameChallengedPanel;

    
    public GameObject fullGameListParentGrid;
    public GameObject fullGameListPanel;

    public Vector2 listStartPos;
    public float listYPadding;
    int listIndex;

    static List<appManager.playerGameID> p1Initiated = new List<appManager.playerGameID>();
    static List<appManager.playerGameID> p1Challenged = new List<appManager.playerGameID>();

    public event DDBScanResponseDelegate OnScanComplete;
    public event DDBQueryHashKeyOnlyDelegate<appManager.playerGameID> OnP1GameFetchComplete;

    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    void Start () {
      
	}

    public void init_MPLobby()
    {
        m_loadPanelManager.instance.activateLoadPanel();
        Invoke("removeLoadPanel", 3f);
        OnScanComplete += OnPlayerScanComplete;
        // OnP1GameFetchComplete += allP1GameQueryComplete;
        clearLists();
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
        allPlayers.Clear();

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
        Debug.Log("***QUERYING ALL GAMES INVOLVING P1***");
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
        pgID.Clear();
        p1Initiated.Clear();
        p1Challenged.Clear();
        for (int i = 0; i < response.Count; i++)
        {
            appManager.playerGameID tPGID = new appManager.playerGameID();
            tPGID.playerID = response[i].playerID;
            tPGID.gameID = response[i].gameID;
            tPGID.role = response[i].role;

            ///Debug.Log(tPGID.playerID + " is a " + tPGID.role + " in " + tPGID.gameID);

            pgID.Add(tPGID); //These ALL of the games this user is currently involved in

            if (tPGID.role == appManager.playerRoles.intiated.ToString())
                p1Initiated.Add(tPGID);
            else if (tPGID.role == appManager.playerRoles.challenged.ToString())
                p1Challenged.Add(tPGID);
        }

        m_MPLobby_Matchmake.instance.listAllGamesP1IsInitiated();
        m_MPLobby_Matchmake.instance.listAllGamesP1IsChallengedIn();
    }

    void listAllGamesP1IsInitiated()
    {
        Debug.Log("---LISTING ALL GAMES P1 INITIATED:---" + p1Initiated.Count);
        for (int i = 0; i < p1Initiated.Count; i++)
        {
            //Debug.Log("LISTING A GAME P1 INITIATED");
            GameObject tButton = Instantiate(gamesInitiatedButton);
           // tButton.transform.SetParent(gameInitListParentGrid.transform);
           

            ui_existingGameButton tManager = tButton.GetComponent<ui_existingGameButton>();
            tManager.parentCanvasTransform = gameInitListParentGrid;
            tManager.devicePlayerRole = appManager.playerRoles.intiated;
            tManager.gameID = p1Initiated[i].gameID;
            tManager.loadGameEntity(tManager.gameID);
        }
    }

    void listAllGamesP1IsChallengedIn()
    {
        Debug.Log("---LISTING ALL GAMES P1 CHALLENGED:--- " + p1Challenged.Count);
        for (int i = 0; i < p1Challenged.Count; i++)
        {
            //Debug.Log("LISTING A GAME P1 CHALLENGED");
            GameObject tButton = Instantiate(gamesInitiatedButton);
           // tButton.transform.SetParent(gameChallengedParentGrid.transform);

            ui_existingGameButton tManager = tButton.GetComponent<ui_existingGameButton>();
            tManager.parentCanvasTransform = gameChallengedParentGrid;
            tManager.devicePlayerRole = appManager.playerRoles.challenged;
            tManager.gameID = p1Challenged[i].gameID; 
            tManager.loadGameEntity(tManager.gameID);
           
        }
    }

    void removeLoadPanel()
    {
        //Dumb implementation for now. We'll need full event checkin at the end.
        m_loadPanelManager.instance.deactivateLoadPanel();
    }

    public void clearLists()
    {
        //Get children of player & game lists
        foreach(Transform child in opponentListParentGrid.transform)
        {
            Destroy(child.gameObject);
        }

        foreach(Transform child in fullGameListParentGrid.transform)
        {
            Destroy(child.gameObject);
        }
        //Nuke 'em!
    }
}
