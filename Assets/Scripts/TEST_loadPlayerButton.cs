using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using Amazon.DynamoDBv2.DataModel;
using DDBHelper;
using Assets.autoCompete.players;

public class TEST_loadPlayerButton : MonoBehaviour {
    public Text idStatus;
	// Use this for initialization
	void Start () {
	    idStatus.text = m_prefsDataManager.getPlayerIDPref();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onLoadClick()
    {
        entity_players tP = new entity_players();
        tP.playerID = m_prefsDataManager.getPlayerIDPref();
        
        DBWorker.Instance.Load(tP, OnPlayerLoaded);
    }

    static void OnPlayerLoaded(entity_players response, GameObject obj, string nextMethod, Exception e = null)
    {
        Debug.Log("***PLAYER UPDATED FROM REGISTER BUTTON***");
        Debug.Log("NewPlayerName:" + response.playerName);
        if (e == null)
        {
            appManager.devicePlayer = response;
            Debug.Log("LOADED THIS PLAYER: " + appManager.devicePlayer.playerName);
        }
        else
        {
            Debug.Log("***LOAD PLAYER ENCOUNTERED A PROBLEM***");
            DBTools.PrintException("DBExample Save", e);
        }
    }
}
