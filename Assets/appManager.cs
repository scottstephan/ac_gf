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
using UnityEngine.SceneManagement;

public class appManager : MonoBehaviour {
    public static appManager instance = null;
    public static entity_players devicePlayer;
    public static playerRoles devicePlayerRoleInCurGame;

    public static entity_games curLiveGame; 

    public enum playerRoles
    {
        intiated,
        challenged
    }

    public  enum sceneNames
    {
        title,
        categorySelect,
        mainRound,
        multiPlayerLobby,
        multiplayerCategorySelect
    }

    public enum authSources
    {
        debug,
        facebook,
        google,
        autocompete
    }

    public enum tableNames
    {
        players,
        games_active
    }

    public static string currentPlayerID;

    [DynamoDBTable("gamesByPlayer")]
    public class playerGameID
    {

        [DynamoDBHashKey]
        public string playerID;
        [DynamoDBRangeKey]
        public string gameID;
        [DynamoDBProperty]
        public string role;
    }
    
    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    } 

    
    void Start () {
       
	}

    public static string generateUniquePlayerID()
    {
        string uniqueDeviceID = SystemInfo.deviceUniqueIdentifier;
        return uniqueDeviceID;
    }

    public static string generateUniqueGameID()
    {
        return devicePlayer.playerID + UnityEngine.Random.Range(0, 10000); //The odds are in my faor, but still. Use datetime!
    }

    public static void createGameObject(string p1ID, string p2ID, string p1Name, string p2Name, bool isMPGame)
    {
        curLiveGame = new entity_games();
        curLiveGame.initGame(appManager.generateUniqueGameID(), p1ID, p2ID, false, false, 0, 0, isMPGame, p1Name,p2Name);
    }

    public static void loadScene(sceneNames sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad.ToString());
    }

    public static void saveCurGame()
    {
        if(curLiveGame == null)
        {
            Debug.Log("NO GAME SET IN APP MANAGER; NOT SAVING GAME");
            return;
        }
        Debug.Log("***SAVING CUR GAME TO DB***");
        DBWorker.Instance.Save(curLiveGame, GameSavedToDB);

        createPlayerGameRelationship(curLiveGame.player1_id, curLiveGame.player2_id, curLiveGame.gameID);
    }

    static void GameSavedToDB(bool success, GameObject obj, string nextMethod, Exception e = null)
    {
        if(e != null)
        {
            DBTools.PrintException("DBExample Save",e);
        }
        else
        {
            Debug.Log("***GAME SUCCESSFULLY SAVED TO DB***");
        }
    }

    public static void loadGameEntity(string gameID)
    {
        Debug.Log("***LOAD GAME ENTITY FOR " + gameID +"***");
        entity_games tG = new entity_games();
        tG.gameID = gameID;
        DBWorker.Instance.Load<entity_games>(tG, gameLoadComplete);   
    }

    static void gameLoadComplete(entity_games response, GameObject obj, string nextMethod, Exception e = null)
    {
        Debug.Log("***GAME ENTITY LOAD COMPLETE***");
        appManager.curLiveGame = response;
    }

    static void createPlayerGameRelationship(string p1ID, string p2ID, string gameID)
    {
        playerGameID p1 = new playerGameID();
        p1.playerID = p1ID;
        p1.gameID = gameID;
        p1.role = appManager.playerRoles.intiated.ToString();

        playerGameID p2 = new playerGameID();
        p2.playerID = p2ID;
        p2.gameID = gameID;

        p2.role = appManager.playerRoles.challenged.ToString();

        DBWorker.Instance.Save(p1, OnPlayerGameIDSaved);
        DBWorker.Instance.Save(p2, OnPlayerGameIDSaved);
    }
    
    static void OnPlayerGameIDSaved(bool success, GameObject obj, string nextMethod, Exception e = null)
    {
        if (e == null)
            Debug.Log("***PLAYERGAMEID SAVED***");
        else
            DBTools.PrintException("DBExample Save", e);
    }

    public static void setCurGame(entity_games cGame, playerRoles p1Role)
    {
        curLiveGame = cGame;
        devicePlayerRoleInCurGame = p1Role;
    }
}
