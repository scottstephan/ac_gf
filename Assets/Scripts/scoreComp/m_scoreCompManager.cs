using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.autoCompete.games;

public class m_scoreCompManager : MonoBehaviour {
    public static m_scoreCompManager instance = null;

    public Text yourText;
    public Text theirText;

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

    void updatePlayerScores()
    {
        if (appManager.curLiveGame.isMPGame)
        { //Maybe see if p2 has finished yet???
            if (appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated)
            { //YOU are P1- P2 may or may not be done. 
                yourText.text = "You got <color=blue>" + appManager.curLiveGame.p1_score + "</color> points!";
                if (appManager.curLiveGame.p2_Fin)
                    theirText.text = appManager.curLiveGame.player2_name + " got <color=blue>" + appManager.curLiveGame.p2_score + "</color> points!";
                else
                    theirText.text = "Waiting for <color=blue>" + appManager.curLiveGame.player2_name + "</color> to finish!";
            }
            else if(appManager.devicePlayerRoleInCurGame == appManager.playerRoles.challenged)
            { //YOU are P2 - In this case, P1 HAS to have finished
                yourText.text = "You got <color=blue>" + appManager.curLiveGame.p2_score + "</color> points!";
                theirText.text = appManager.curLiveGame.player1_name + " got <color=blue>" + appManager.curLiveGame.p1_score + "</color> points!";
            }
        }
        else
        {            //Set YOUR text, but not theirs
            yourText.text = "You got <color=blue>" + appManager.curLiveGame.p1_score + "</color> points!";
            theirText.text = " ";
        }
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
	
}
