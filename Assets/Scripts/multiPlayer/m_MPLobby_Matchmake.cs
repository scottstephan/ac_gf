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

    public GameObject opponentButton;
    public GameObject opponentListParentGrid;
    public GameObject playerListPanel;
    public GameObject friendListHeader;

    public GameObject gamesInitiatedButton;

    public GameObject gameChallengedButton;

    public GameObject fullGameListParentGrid;
    public GameObject fullGameListPanel;
    public GameObject playerGameListHeader;
    public GameObject challengeAFriendButton;

    int listIndex;

    static List<appManager.playerGameID> p1Initiated = new List<appManager.playerGameID>();
    static List<appManager.playerGameID> p1Challenged = new List<appManager.playerGameID>();
    static List<appManager.playerGameID> pgID = new List<appManager.playerGameID>();
    static List<entity_games> allP1GameEntities = new List<entity_games>();

    public event DDBScanResponseDelegate OnScanComplete;
    public event DDBQueryHashKeyOnlyDelegate<appManager.playerGameID> OnP1GameFetchComplete;

    public bool userFriendListPopulated = false;
    List<object> users = new List<object>();

    int numGameLoadsReportedBack;

    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void init_MPLobby()
    {
        OnScanComplete += OnPlayerScanComplete;
        clearLists();
        m_fbStatusManager.instance.loadFriendsInstalledList(playerFriendsPopulated);
        GameObject tH = Instantiate(this.playerGameListHeader);
        tH.transform.SetParent(fullGameListParentGrid.transform, false);
        tH.transform.SetAsFirstSibling();

        appManager.instance.startLoadWheel(fullGameListParentGrid.transform); //WIll need to reset on backups!

        getAllP1Games();
    }

    IEnumerator delayAndStartWheel()
    {
        yield return new WaitForSeconds(.25f);
        appManager.instance.startLoadWheel(fullGameListParentGrid.transform);
    }

    void playerFriendsPopulated(List<object> userFriendsWithAppInstalled)
    {
        Debug.Log("---ME/FRIENDS?INSTALLED POPULATED---");
        userFriendListPopulated = true;
        users = userFriendsWithAppInstalled;
        populateFriendsSubpanel();
    }

    void populateFriendsSubpanel()
    {
        List<entity_players> friendsWithApp = new List<entity_players>();

        for (int i = 0; i < users.Count; i++)
        {
            entity_players tP = new entity_players();
            string fName = Convert.ToString(((Dictionary<string, object>)users[i])["name"]);
            string fId = Convert.ToString(((Dictionary<string, object>)users[i])["id"]);

            tP.playerName = fName;
            tP.searchName = fName.ToLower();
            tP.playerID = fId;

            friendsWithApp.Add(tP);
            Debug.Log(fName + " :: " + fId);
        }

        createPlayerListInUI(friendsWithApp);
    }

    void getAndListAllPlayers() {
        Debug.Log("***SCANNING FOR ALL PLAYERS***");
        DBWorker.Instance.ScanTable("players", OnPlayerScanComplete);  
    }


    void createPlayerListInUI(List<entity_players> allPlayers)
    {
        GameObject fLH = Instantiate(friendListHeader);
        fLH.transform.SetParent(opponentListParentGrid.transform, false);

        for (int i = 0; i < allPlayers.Count; i++)
        {
            GameObject tButton = Instantiate(opponentButton);

            ui_opponentButtonManager tManager = tButton.GetComponent<ui_opponentButtonManager>();
            tManager.opEntity = allPlayers[i];
            tManager.setUpButton();
            tButton.transform.SetParent(opponentListParentGrid.transform, false);
        }
    }

    void getAllP1Games()
    {

        Debug.Log("***QUERYING ALL GAMES INVOLVING P1***: " + appManager.FB_ID);
        List<string> attToReturn = new List<string>();
        attToReturn.Add("playerID");
        attToReturn.Add("gameID");
        attToReturn.Add("role");

        DBWorker.Instance.QueryHashKeyObject<appManager.playerGameID>(appManager.FB_ID, attToReturn, allP1GameQueryComplete,true);
    }

    public void allP1GameQueryComplete(List<appManager.playerGameID> response, Exception e = null)
    {
        Debug.Log("ALL P1 GAMES LOADED");
        pgID.Clear();
        p1Initiated.Clear();
        p1Challenged.Clear();
        allP1GameEntities.Clear();

        for (int i = 0; i < response.Count; i++)
        {
            appManager.playerGameID tPGID = new appManager.playerGameID();
            tPGID.playerID = response[i].playerID;
            tPGID.gameID = response[i].gameID;
            tPGID.role = response[i].role;

            ///Debug.Log(tPGID.playerID + " is a " + tPGID.role + " in " + tPGID.gameID);

            pgID.Add(tPGID); //These are ALL of the games this user is currently involved in
/*
            if (tPGID.role == appManager.playerRoles.intiated.ToString())
                p1Initiated.Add(tPGID);
            else if (tPGID.role == appManager.playerRoles.challenged.ToString())
                p1Challenged.Add(tPGID);*/
        }

        //    m_MPLobby_Matchmake.instance.listAllGamesP1IsInitiated();
        //    m_MPLobby_Matchmake.instance.listAllGamesP1IsChallengedIn();
        processAllP1Games();
    }

    void processAllP1Games()
    {
        Debug.Log("---LOADING ALL GAMES P1:---" + pgID.Count);
        //Maybe load the games here and then send the data to the buttons???
        for(int i = 0; i < pgID.Count; ++i)
        {
            loadGameEntity(pgID[i].gameID);
        }

        if(pgID.Count == 0)
        {
            Debug.Log("Player has no games, creating challenge button");
            listP1NoGames();
        }
       
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
        if (e != null)
        {
            DBTools.PrintException("gameLoadComplete", e);
        }
        Debug.Log("***GAME ENTITY LOAD COMPLETE***");
        allP1GameEntities.Add(response);

        if(allP1GameEntities.Count == pgID.Count)
        {
            Debug.Log("ALL P1 GAME ENTITIES LOADED");
            listAllP1Games();
        }
    }

    void listAllP1Games()
    {//SHould say DEVICEPLAYER, not P1
        appManager.instance.stopLoadWHeel();
        allP1GameEntities.Sort((x, y) =>Convert.ToDateTime(x.lastDateTimeEdit).CompareTo(Convert.ToDateTime(y.lastDateTimeEdit)));
        allP1GameEntities.Reverse();

        GameObject cFB = Instantiate(challengeAFriendButton);
        cFB.transform.SetParent(fullGameListParentGrid.transform, false);

        for (int i = 0; i < allP1GameEntities.Count; i++)
        {
            GameObject tButton = Instantiate(gamesInitiatedButton);

            ui_existingGameButton tManager = tButton.GetComponent<ui_existingGameButton>();
            
            tManager.gameID = allP1GameEntities[i].gameID;
            tManager.thisGame = allP1GameEntities[i];
            tManager.determineGameState();
        }
    }

    void listP1NoGames()
    {
        appManager.instance.stopLoadWHeel();
        GameObject cFB = Instantiate(challengeAFriendButton);
        cFB.transform.SetParent(fullGameListParentGrid.transform, false);
    }

    void listAllGamesP1IsInitiated()
    {
        appManager.instance.stopLoadWHeel();

        GameObject cFB = Instantiate(challengeAFriendButton);
        cFB.transform.SetParent(fullGameListParentGrid.transform, false);

        Debug.Log("---LISTING ALL GAMES P1 INITIATED:---" + p1Initiated.Count);
        for (int i = 0; i < p1Initiated.Count; i++)
        {
            GameObject tButton = Instantiate(gamesInitiatedButton);           

            ui_existingGameButton tManager = tButton.GetComponent<ui_existingGameButton>();
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
            GameObject tButton = Instantiate(gamesInitiatedButton);

            ui_existingGameButton tManager = tButton.GetComponent<ui_existingGameButton>();
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
            if (child.name == "loadingCircle")
            {
                continue;
            }
            Destroy(child.gameObject);
        }
        //Nuke 'em!
    }

/// <summary>
/// ---YE OLDEN --\\\
/// </summary>

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

}
