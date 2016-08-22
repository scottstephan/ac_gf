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
        appManager.createGameObject(appManager.currentPlayerID,opEntity.playerID,appManager.devicePlayer.playerName,opEntity.playerName,true);
        appManager.saveCurGame();
        //Load cat scree
        appManager.loadScene(appManager.sceneNames.categorySelect);
      
    }


}
