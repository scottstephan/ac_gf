using UnityEngine;
using System.Collections;
using UnityEngine.UI;
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
       acDBHelper.instance.loadPlayerFromDynamoViaID(m_prefsDataManager.getPlayerIDPref(), (bool playerLoaded, entity_players tPlayer) =>
       {
           if (tPlayer != null)
           {
               Debug.Log("VIA LOAD BUTTON: LOADED PLAYER: " + tPlayer.playerName);
               idStatus.text = tPlayer.playerID + " :: " + tPlayer.playerName;
               appManager.devicePlayer = tPlayer;
           }
           else
               Debug.Log("VIA LOAD BUTTON: PLAYER NOT FOUND IN DB");
       });

       
    }
}
