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
    public bool waitingForAppInit = true;
    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        registerButton.SetActive(false);
        registerNameInput.SetActive(false);
    }
	// Use this for initialization
	void Start () {
        appManager.instance.checkFBLoginStatus();
    }

    public void appInitComplete()
    {
        if (waitingForAppInit)
        {
            waitingForAppInit = false;
            m_phaseManager.instance.changePhase(m_phaseManager.phases.titleScreen);
        }
    }

    void checkIfPlayerStoredLocal()
    {
        string id = m_prefsDataManager.getPlayerIDPref();
        Debug.Log("---ID IN DATA PREFS---: " + id);
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
        }

    }

    void checkIfPlayerStoredAWS()
    {
        entity_players tP = new entity_players();
        tP.playerID = m_prefsDataManager.getPlayerIDPref();
        tP.searchName = m_prefsDataManager.getPlayerSearchName();

        DBWorker.Instance.Load(tP, OnPlayerLoaded);
    }

    static void OnPlayerLoaded(entity_players response, GameObject obj, string nextMethod, Exception e = null)
    {
        Debug.Log("***PLAYER LOADED***");

        if(response == null)
        {         //If user not found, pop dialog, cap info and save
            Debug.Log ("PLAYER NOT FOUND IN AWS; REVEALING DIALOG");
            m_loadScreenManager.instance.createPlayerRegisterDialog();
            m_loadPanelManager.instance.deactivateLoadPanel();
            return;
        }

        if (e == null)
        {
            appManager.devicePlayer = response;
            Debug.Log("***LOADED THIS PLAYER: " + appManager.devicePlayer.playerName + "***");
            m_phaseManager.instance.changePhase(m_phaseManager.phases.titleScreen);
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

    public void moveInMenuPanel()
    {
        m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.loadingToMenu);
    }
}
