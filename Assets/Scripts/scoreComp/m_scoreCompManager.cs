using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.autoCompete.games;

public class m_scoreCompManager : MonoBehaviour {
    public static m_scoreCompManager instance = null;
    private int cScore;

    public Text yourText;
    public Text theirText;
    public Text highScoreStatus;

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
        string scorePrepend = "got <color=blue><size=135>";
        string scoreAppend = "</size></color> points!";
        string waitingPrepend = "Waiting for <color=green>";
        string waitingAppend = "</color> to finish";

        if (appManager.curLiveGame.isMPGame)
        { 
            if (appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated)
            { //YOU are P1- P2 may or may not be done. 
                yourText.text = "You" + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;
                cScore = appManager.curLiveGame.p1_score;
                if (appManager.curLiveGame.p2_Fin)
                    theirText.text = appManager.curLiveGame.player2_name + scorePrepend + appManager.curLiveGame.p2_score + scoreAppend;
                else
                    theirText.text = waitingPrepend + appManager.curLiveGame.player2_name + waitingAppend;
            }
            else if(appManager.devicePlayerRoleInCurGame == appManager.playerRoles.challenged)
            { //YOU are P2 - In this case, P1 HAS to have finished
                cScore = appManager.curLiveGame.p2_score;
                yourText.text = "You " + scorePrepend + appManager.curLiveGame.p2_score + scoreAppend;
                theirText.text = appManager.curLiveGame.player1_name + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;
            }
        }
        else
        {            //Set YOUR text, but not theirs
            cScore = appManager.curLiveGame.p1_score;
            yourText.text = "You " + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;
            theirText.text = " ";
        }

        compHSValue(cScore);
    }

    void compHSValue(int curScore)
    {
        int savedHS = 0;
        Debug.Log("Getting highscore for: " + appManager.curLiveGame.categoryText);
        savedHS = u_acJsonUtility.instance.getCatHighscore(appManager.curLiveGame.categoryText);
        Debug.Log("Saved v Cur: " + savedHS + " vs. " + curScore);

        if(curScore > savedHS)
        {
            highScoreStatus.text = "A new high score in <color=green>" + appManager.curLiveGame.categoryText + "</color>!: <color=blue><size=135>" + curScore +"</size></color>";
            u_acJsonUtility.instance.updateHighScore(appManager.curLiveGame.categoryText, curScore);
        }
        else
        {
            highScoreStatus.text = "Your best score in <color=green>" + appManager.curLiveGame.categoryText + "</color> is  <color=blue><size=135>" + savedHS + "</size></color>";
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
