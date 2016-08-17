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

public class autoCompeteDBHelper : MonoBehaviour
{

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
            savePlayerToDynamo();
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

    void savePlayerToDynamo()
    {
        entity_players newPlayer = new entity_players();
        newPlayer.playerID = Random.Range(0, 100).ToString();
        newPlayer.playerName = "TestJones";
        newPlayer.playerAuthSource = "testing";

        Context.SaveAsync(newPlayer, (result) =>
        {
            if (result.Exception == null)
                Debug.Log("PLAYER SAVED");
            else
                Debug.LogError("ERROR SAVING PLAYER OBJECT:" + result.Exception.Message);
        });

    }

    void loadPlayerFromDynamoViaID(string id)
    {

    }

    void saveGameToDyanmo(string p1ID, string p2ID)
    {

    }

    void loadGameFromDynamo()
    {

    }

}
