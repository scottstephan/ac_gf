using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Amazon.DynamoDBv2.DataModel;
using DDBHelper;
using Assets.autoCompete.players;
using Assets.autoCompete.nameIDPair;
using Amazon.DynamoDBv2.Model;

public class playBy_FriendName : MonoBehaviour {
    public InputField playerNameInput;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void onButtonClick()
    {
        //See if you can find opponent in DB
        loadOpposingPlayer(playerNameInput.text);
        //IF so, grab their info and make a game
        //Proceed as normalis
    }

    public void loadOpposingPlayer(string name)
    {
        List<string> attToReturn = new List<string>();
        attToReturn.Add("id");

        DBWorker.Instance.QueryHashKeyObject<nameIdPair>(name, attToReturn, loadedNameIDCombo, true);
    }

    static void loadedNameIDCombo(List<nameIdPair> response, Exception e = null)
    {
        if (e == null)
        {
            Debug.Log("***LOADE NAMEID DATA COMBO***");
        }
        else
        {
            DBTools.PrintException("loadedNameIDCombo", e);
        }
    }
}
