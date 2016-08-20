using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.autoCompete.players;
using UnityEngine.SceneManagement;

public class ui_opponentButtonManager : MonoBehaviour {
    public Text opButtonText;
    string opID;
    public entity_players opEntity;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setUpButton()
    {
        opButtonText.text = opEntity.playerName;
    }

   public void onButtonCLick()
    {
        //Create game obj n appManager
        appManager.createGameObject();
        //Assign local playe to p1 slot and do general init
        appManager.curLiveGame.gameID = appManager.generateUniqueGameID(); //need an id schema!
        appManager.curLiveGame.player1_id = appManager.currentPlayerID; //prim key for player DB
        appManager.curLiveGame.player2_id = null;
        appManager.curLiveGame.p1_score = 0;
        appManager.curLiveGame.p2_score = 0;
        appManager.curLiveGame.p1_Fin = false;
        appManager.curLiveGame.p2_Fin = false;
        appManager.curLiveGame.isMPGame = true;
        appManager.saveCurGame();
        //Load cat scree
        appManager.loadScene(appManager.sceneNames.categorySelect);
      
    }


}
