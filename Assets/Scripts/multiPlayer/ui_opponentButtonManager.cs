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
        if (opEntity.playerID == appManager.currentPlayerID) Destroy(gameObject);
        opButtonText.text = opEntity.playerName;
    }

   public void onButtonCLick() 
    {
        Debug.Log("---GAME BUTTON CLICKED---");
        //Create game obj n appManager
        appManager.createGameObject(appManager.currentPlayerID,opEntity.playerID,appManager.devicePlayer.playerName,opEntity.playerName,true);
        appManager.curGameStatus = appManager.E_lobbyGameStatus.init_playGame;
        m_phaseManager.instance.changePhase(m_phaseManager.phases.categorySelectMP);
    }


}
