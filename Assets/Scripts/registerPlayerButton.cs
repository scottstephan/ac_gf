﻿using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using Amazon.DynamoDBv2.DataModel;
using DDBHelper;
using Assets.autoCompete.players;
using Assets.autoCompete.nameIDPair;

public class registerPlayerButton : MonoBehaviour {
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
       tP.searchName = playerNameInput.text.ToLower();
       tP.playerAuthSource = appManager.authSources.debug.ToString();
       tP.playerID = appManager.currentPlayerID;
       tP.autoCompeteUsableID = tP.playerID;
        appManager.devicePlayer = tP; //Not super thrilled about assigning this pre-load. 
        m_loadPanelManager.instance.activateLoadPanel();

        m_prefsDataManager.setPlayerSearchName(tP.searchName);

        nameIdPair tIDP = new nameIdPair();
        tIDP.name = tP.searchName;
        tIDP.id = tP.playerID;

        DBWorker.Instance.Save(tP, OnPlayerSaved);
        DBWorker.Instance.Save(tIDP, OntIDPSaved);
    }

    static void OnPlayerLoaded(entity_players response, GameObject obj, string nextMethod, Exception e = null)
    {
        Debug.Log("***PLAYER UPDATED FROM REGISTER BUTTON***");
        Debug.Log("NewPlayerName:" + response.playerName);
    }

    static void OntIDPSaved(bool success, GameObject obj, string nextMethod, Exception e = null)
    {
        if(e == null)
        {
            Debug.Log("***SAVED ID/NAME PAIR IN DB***");
        }
    }

        static void OnPlayerSaved(bool success, GameObject obj, string nextMethod, Exception e = null)
    {
        if (e == null)
        {
            Debug.Log("***NEW PLAYER SAVED/UPDATED FROM REGISTER BUTTON***");
            m_loadPanelManager.instance.deactivateLoadPanel();
            m_phaseManager.instance.changePhase(m_phaseManager.phases.titleScreen);
        }
        else
            DBTools.PrintException("DBExample Save", e);
    }

    
}
