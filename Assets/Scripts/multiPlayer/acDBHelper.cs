using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Assets.autoCompete.players;
using Assets.autoCompete.games;

public class acDBHelper : MonoBehaviour
{
    public static acDBHelper instance = null;


    public enum E_gameTables
    {
        players,
        games_live
    }

    public E_gameTables curTableTarget = E_gameTables.players;

    public string cognitoIdentityPoolID;
    //public string tableName;

    private CognitoAWSCredentials credentials;

    private IAmazonDynamoDB _client;
    private DynamoDBContext _context;

    //    private List<entity_comic> comicList = new List<entity_comic>();
    private int curComicIndex;

    private DynamoDBContext Context
    {
        get
        {
            if (_context == null)
                _context = new DynamoDBContext(_client);
            return _context;
        }
    }

    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        createCredentials(true);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
          //  savePlayerToDynamo(); //Need save v. update logic
        }
    }

    void createCredentials(bool testCredentials)
    {
        credentials = new CognitoAWSCredentials(cognitoIdentityPoolID, RegionEndpoint.USEast1);

        credentials.GetIdentityIdAsync(delegate (AmazonCognitoIdentityResult<string> result)
        {
            if (result.Exception != null)
            {
                Debug.LogError("Exception in getting creds: " + result.Exception.Message);
            }

            var ddbClient = new AmazonDynamoDBClient(credentials, RegionEndpoint.USEast1);
            _client = ddbClient;

            if (testCredentials)
            {
                Debug.Log("RETRIEVING INIT TABLE INFO");

                var request = new DescribeTableRequest
                {
                    TableName = @curTableTarget.ToString()
                };

                ddbClient.DescribeTableAsync(request, (ddbresult) =>
                {
                    if (result.Exception != null)
                    {
                        Debug.Log(result.Exception.Message);
                        return;
                    }

                    var response = ddbresult.Response;
                    TableDescription description = response.Table;
                    Debug.Log("TABLE INFO:: Name: " + description.TableName + "\n" + "Num Items: " + description.ItemCount);
                }, null);
            }
        });
    }

    public void checkIfPlayerExistsByID(string id, string playerName)
    {

        loadPlayerFromDynamoViaID(id, (bool playerLoaded, entity_players tPlayer) =>
        {

            Debug.Log("Submitted ID: " + id + " :: " + tPlayer.playerID);

            if (tPlayer.playerID == id)
            {
                Debug.Log("PLAYER ALREADY KNOWN; NOT SAVING NEW RECORD; MOVING TO UPDATE");
                savePlayerToDynamo(id, playerName);
            }
            else
            {
                Debug.Log("PLAYER NOT KNOWN; MOVING TO SAVE RECORD");
                savePlayerToDynamo(id, playerName);
            }
        });
    }

    public void savePlayerToDynamo(string id, string playerName) //Should just pass in the EP from AppManager
    {
        entity_players newPlayer = new entity_players();
        newPlayer.playerID = id;
        newPlayer.playerName = playerName;
        newPlayer.playerAuthSource = "DEBUG";
        
        Context.SaveAsync(newPlayer, (result) =>
        {
            if (result.Exception == null)
                Debug.Log("PLAYER SAVED");
            else
                Debug.LogError("ERROR SAVING PLAYER OBJECT:" + result.Exception.Message);
        });

        appManager.devicePlayer = newPlayer;
    }

    public void loadPlayerFromDynamoViaID(string id, Action <bool, entity_players> completionBlock)
    {

        Debug.Log("TRYING TO LOAD PLAYER FROM DYNAMO VIA ID: " + id);
        Context.LoadAsync<entity_players>(id,
        (AmazonDynamoDBResult<entity_players> result) =>
        {
            if (result.Exception != null)
            {
                Debug.Log("PLAYER ASYNC LOAD ERROR: " + result.Exception.Message);
                if (completionBlock != null)
                    completionBlock(false, null);

                return;
            }
            //MAYBE TRY AND INVOKE AFTER TH ASYNC LOAD????
            Debug.Log("LOADED PLAYER " + result.Result.playerName + " FROM ID " + result.Result.playerID);
            
            if (completionBlock != null)
                completionBlock(true, result.Result);

        }, null);
    }

    public void loadAllPlayers(Action<bool, List<entity_players>> completionBlock)
    {
        Debug.Log("***LOADING ALL PLAYERS***");
        List<entity_players> allPlayers = new List<entity_players>();
        var search = Context.ScanAsync<entity_players>(new ScanCondition("playerName", ScanOperator.IsNotNull));
        search.GetRemainingAsync(result =>
        {
            if(result.Exception != null)
            {
                Debug.Log("ALL PLAYER SCAN NOT COMPLETED: " + result.Exception.Message);
                if (completionBlock != null)
                    completionBlock(false, null);
                return;
            }

            allPlayers = result.Result;
            if (completionBlock != null)
                completionBlock(true, allPlayers);
        }, null);        
    }

    //******Games*******\

    public void saveGameToDyanmo(entity_games cGame)
    {
       
        Context.SaveAsync(cGame, (result) =>
        {
            if (result.Exception == null)
                Debug.Log("Game SAVED");
            else
                Debug.LogError("ERROR SAVING Game OBJECT:" + result.Exception.Message);
        });
    }

    void loadGameFromDynamo()
    {

    }

}
