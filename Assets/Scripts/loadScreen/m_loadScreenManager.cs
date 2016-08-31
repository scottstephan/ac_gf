using UnityEngine;
using System;
using System.Collections;
using Assets.autoCompete.players;
using Amazon.DynamoDBv2.DataModel;
using DDBHelper;
using UnityEngine.UI;

public class m_loadScreenManager : MonoBehaviour {
    public static m_loadScreenManager instance = null;
    public Text playerLoadStatusText;
    public GameObject registerButton;
    public GameObject registerNameInput;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        registerButton.SetActive(false);
        registerNameInput.SetActive(false);
    }
	// Use this for initialization
	void Start () {
        checkIfPlayerStoredLocal();
        checkIfPlayerStoredAWS();         //Now check AWS- Do we know this user?

    }

    void checkIfPlayerStoredLocal()
    {
        string id = m_prefsDataManager.getPlayerIDPref();
        Debug.Log("Returned ID: " + id);
        //Then confirm the device
        if (m_prefsDataManager.confirmCurrentUserIsStoredUser(id))
        {
            appManager.currentPlayerID = id;
            playerLoadStatusText.text = "Player is known on this machine. Moving to AWS check";
        }
        else //create a unique ID for this device
        {
            appManager.currentPlayerID = appManager.generateUniquePlayerID();
            m_prefsDataManager.setPlayerIDPref(appManager.currentPlayerID);
            playerLoadStatusText.text = "Player unknown local; Creatind ID and moving to AWS";
        }

    }

    void checkIfPlayerStoredAWS()
    {
        entity_players tP = new entity_players();
        tP.playerID = m_prefsDataManager.getPlayerIDPref();

        DBWorker.Instance.Load(tP, OnPlayerLoaded);
    }

    static void OnPlayerLoaded(entity_players response, GameObject obj, string nextMethod, Exception e = null)
    {
        Debug.Log("***PLAYER LOADED***");

        if(response == null)
        {         //If user not found, pop dialog, cap info and save
            m_loadScreenManager.instance.playerLoadStatusText.text = "PLAYER NOT FOUND IN AWS; REVEALING DIALOG";
            m_loadScreenManager.instance.createPlayerRegisterDialog();
            m_loadPanelManager.instance.deactivateLoadPanel();
            return;
        }

        if (e == null)
        {
            appManager.devicePlayer = response;
            Debug.Log("***LOADED THIS PLAYER: " + appManager.devicePlayer.playerName + "***");
            m_loadScreenManager.instance.playerLoadStatusText.text = "Player loaded from AWS! Going to menu!";
            m_loadScreenManager.instance.loadToMenu();
            m_loadPanelManager.instance.deactivateLoadPanel();

        }
        else
        {
            Debug.Log("***LOAD PLAYER ENCOUNTERED A PROBLEM***");
            DBTools.PrintException("DBExample Save", e);
        }

    }

    void createPlayerRegisterDialog()
    {
        registerNameInput.SetActive(true);
        registerButton.SetActive(true);
    }

    public void loadToMenu()
    {
        appManager.loadScene(appManager.sceneNames.title);
    }

}
