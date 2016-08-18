using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Amazon;
using Amazon.CognitoIdentity;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Assets.autoCompete.players;
using System.Delegate;

public class acDBHelper : MonoBehaviour
{
    public static acDBHelper instance = null;
    delegate bool myCallBack(string l_id, string r_id);


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

    public bool checkIfPlayerExistsByID(string id)
    {
        myCallBack callBackFunc;
        callBackFunc = compareIDNumbers;

        entity_players tPlayer = loadPlayerFromDynamoViaID(id); //THE PROBLEM WITH THIS IS THAT THIS IS A NON-SYNCHRONOUS LOAD. It'll return an empty type in the middle.
        Debug.Log("Submitted ID: " + id + " :: " + tPlayer.playerID);

        if (tPlayer.playerID == id)
        {
            Debug.Log("PLAYER ALREADY KNOWN; NOT SAVING NEW RECORD; MOVING TO UPDATE");
            return true;
        }
        else
        {
            Debug.Log("PLAYER NOT KNOWN; MOVING TO SAVE RECORD");
            savePlayerToDynamo(id);
            return false;
        }

    }

    public bool compareIDNumbers(string l_id, string r_id)
    {
        return true;
    }

    public void savePlayerToDynamo(string id)
    {
        entity_players newPlayer = new entity_players();
        newPlayer.playerID = id;
        newPlayer.playerName = "DEBUGTEST" + Random.Range(0,999);
        newPlayer.playerAuthSource = "DEBUG";
        
        Context.SaveAsync(newPlayer, (result) =>
        {
            if (result.Exception == null)
                Debug.Log("PLAYER SAVED");
            else
                Debug.LogError("ERROR SAVING PLAYER OBJECT:" + result.Exception.Message);
        });

    }

    public entity_players loadPlayerFromDynamoViaID(string id)
    {
        entity_players tempPlayer = new entity_players();

        Debug.Log("TRYING TO LOAD PLAYER FROM DYNAMO VIA ID: " + id);
        Context.LoadAsync<entity_players>(id,
        (AmazonDynamoDBResult<entity_players> result) =>
        {
            if (result.Exception != null)
            {
                Debug.Log("PLAYER ASYNC LOAD ERROR: " + result.Exception.Message);
                return;
            }
            //MAYBE TRY AND INVOKE AFTER TH ASYNC LOAD????
            tempPlayer = result.Result;
            Debug.Log("LOADED PLAYER " + tempPlayer.playerName + " FROM ID " + tempPlayer.autoCompeteUsableID);
            
        }, null);

        return tempPlayer;
    }

    void saveGameToDyanmo(string p1ID, string p2ID)
    {

    }

    void loadGameFromDynamo()
    {

    }

}
