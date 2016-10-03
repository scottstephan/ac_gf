using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.autoCompete.games;

public class m_scoreCompManager : MonoBehaviour {
    public static m_scoreCompManager instance = null;

    public Text p1Score;
    public Text p2Score;
    public Text p1Name;
    public Text p2Name;
    // Use this for initialization
    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void initSC()
    {
        updatePlayerView();
        updatePlayerScores();
        if (appManager.curGameStatus != appManager.E_lobbyGameStatus.init_viewScore)
            determineGameAction();
        else
            m_loadPanelManager.instance.deactivateLoadPanel();
    }

    void updatePlayerView() 
    { 
        if(appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated)
        {
            appManager.curLiveGame.p1HasViewedResult = false;
        }
        else if (appManager.devicePlayerRoleInCurGame == appManager.playerRoles.challenged)
        {
            appManager.curLiveGame.p2HasViewedResult = true;
        } 
    }

    void showFinalScores()
    {
        //Lerp in You Got... dialog
        //Lerp in They got... dialog
        //Loser swings and falls off
        //Win/Lose message appears
        //Exit button appears
    }

    void updatePlayerScores()
    {
        if (appManager.curLiveGame.isMPGame)
        { //Maybe see if p2 has finished yet???
            if (appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated)
            {
                p1Name.text = "Your Score: ";
                p2Name.text = appManager.curLiveGame.player2_name + "'s Score: ";
            }
            else
            {
                p1Name.text = appManager.curLiveGame.player1_name + "'s Score: ";
                p2Name.text = "Your Score: ";
            }
            p2Score.text = appManager.curLiveGame.p2_score.ToString();
        }
        else
        {
            p1Name.text = "Your Score: ";
            p2Name.text = "";
            p2Score.text = "";
        }

        p1Score.text = appManager.curLiveGame.p1_score.ToString(); //No matter what...
    }

    void determineGameAction()
    {
        if (appManager.curLiveGame.p1_Fin && appManager.curLiveGame.p2_Fin)
        {
            entity_gamesDead deadGame = new entity_gamesDead();
            deadGame.initGameDead(appManager.curLiveGame);
            appManager.saveDeadGame(deadGame);
            appManager.deleteCurGame();
        }
        else if(appManager.curLiveGame.p1_Fin && !appManager.curLiveGame.p2_Fin)
        {
            appManager.saveCurGame();
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
