using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.autoCompete.players;
using Assets.autoCompete.games;

public class m_MPLobby_Matchmake : MonoBehaviour {
    public Text txt_curPlayerReadout;
    public GameObject opponentButton;
    public GameObject opponentListParentGrid;
    public GameObject masterCanvas;

    public Vector2 listStartPos;
    public float listYPadding;
    int listIndex;

    List<entity_players> allPlayers = new List<entity_players>();
	// Use this for initialization
	void Start () {
        txt_curPlayerReadout.text = "You: " + appManager.devicePlayer.playerName;
        getAndListAllPlayers();
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void getAndListAllPlayers() {
        //Get a scan of all player sin DB
        acDBHelper.instance.loadAllPlayers((bool playerLoaded, List<entity_players> allPlayers) =>
        {
            foreach (entity_players ep in allPlayers)
            {
                Debug.Log(ep.playerName);
                GameObject tempButton = Instantiate(opponentButton);
                tempButton.transform.SetParent(masterCanvas.transform);
                tempButton.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                Vector3 buttonPos = new Vector3(listStartPos.x, listStartPos.y - (listYPadding * listIndex), 0);
                tempButton.GetComponent<RectTransform>().anchoredPosition = buttonPos;

           //     Debug.Log("Setting " + ep.playerName + " to " + buttonPos);

                ui_opponentButtonManager tempBtnManager = tempButton.GetComponent<ui_opponentButtonManager>();
                tempBtnManager.opEntity = ep;

                tempBtnManager.setUpButton();
                listIndex++;
            }
        });

       
        //Create a button for each

    }
}
