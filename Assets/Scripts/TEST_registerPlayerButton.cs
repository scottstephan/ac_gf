using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using Amazon.DynamoDBv2.DataModel;
using DDBHelper;
using Assets.autoCompete.players;

public class TEST_registerPlayerButton : MonoBehaviour {
    public InputField playerNameInput;
    public event DDBLoadObjResponseDelegate<entity_players> OnLoaded;
    public event DDBCompletionResponseDelegate OnSaved;

    // Use this for initialization
    void Start()
    {
        OnLoaded += OnPlayerLoaded;
        OnSaved += OnPlayerSaved;
    }

    public void onRegisterButtonClick()
    {
        //If there's a playerPref for ID, use that to put the player in appManager by loading
        //If there's ni playerPref for ID, create a new player and save that
       entity_players tP = new entity_players();
       tP.playerName = playerNameInput.text;
       tP.playerAuthSource = appManager.authSources.debug.ToString();

        if (m_prefsDataManager.getPlayerIDPref() != null) {
            //Update an existing player
            tP.playerID = m_prefsDataManager.getPlayerIDPref();
          //  DBWorker.Instance.UpdateItem("players",); //This is confusing For now, use the psuedo-update save method

        }
        else
        {
            //Create and save a new player
            tP.playerID = appManager.generateUniquePlayerID();
        }
        DBWorker.Instance.Save(tP, OnPlayerSaved);

    }

    static void OnPlayerLoaded(entity_players response, GameObject obj, string nextMethod, Exception e = null)
    {
        Debug.Log("***PLAYER UPDATED FROM REGISTER BUTTON***");
        Debug.Log("NewPlayerName:" + response.playerName);
    }

    static void OnPlayerSaved(bool success, GameObject obj, string nextMethod, Exception e = null)
    {
        if(e == null)
         Debug.Log("***NEW PLAYER SAVED/UPDATED FROM REGISTER BUTTON***");
        else
            DBTools.PrintException("DBExample Save", e);
    }

    
}
