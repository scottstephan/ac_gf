using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.autoCompete.games;

public class m_scoreCompManager : MonoBehaviour {
    public static m_scoreCompManager instance = null;
    private int cScore;

    public Text resultText;
    public Text yourText;
    public Text theirText;

    string googleBlueHex = "#4470E6FF";
    string googleGreenHex = "#08AF22FF";

    public enum mpGameState
    {
        onlyP1Complete,
        P1AndP2Complete
    }
  //  public Text highScoreStatus; //Not using this. 

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
        string scorePrepend = "got \n <color=" + googleBlueHex + "><size=135><b>";
        string scoreAppend = "</b></size></color> \n points!";
        string waitingPrepend = "Waiting for <color=" + googleGreenHex + ">";
        string waitingAppend = "</color> to finish";

        //NEED BETTER CASING
            //if p1 is done and P2 is not done: no result text, yes to score text
            //if p1 and p2 are done, result text and score text
        if (appManager.curLiveGame.isMPGame)
        { 
           if(appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated && appManager.curLiveGame.p1_Fin && !appManager.curLiveGame.p2_Fin)
            {//I am P1 and I am done, but am waiting on P2
                //Set Score
                //Do not set results
                yourText.text = "You" + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;
                cScore = appManager.curLiveGame.p1_score;
                theirText.text = waitingPrepend + appManager.curLiveGame.player2_name + waitingAppend;
                resultText.text = "";
            }
           else if(appManager.devicePlayerRoleInCurGame == appManager.playerRoles.challenged && appManager.curLiveGame.p2_Fin)
            {//I just finished
             //Set score
                yourText.text = "You" + scorePrepend + appManager.curLiveGame.p2_score + scoreAppend;
                theirText.text = appManager.curLiveGame.player1_name + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;

                //Set results
                if (appManager.curLiveGame.p2_score > appManager.curLiveGame.p1_score)
                {
                    resultText.text = "YOU WON!";
                }
                else
                {
                    resultText.text = appManager.curLiveGame.player1_name + " WON!";
                }

            }
           else if(appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated && appManager.curLiveGame.p1_Fin && appManager.curLiveGame.p2_Fin)
            {//P2 has seen results, but I am p1 and ahve not. Now both players have seen the results, so delete the game.
                yourText.text = "You" + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;
                theirText.text = appManager.curLiveGame.player2_name + scorePrepend + appManager.curLiveGame.p2_score + scoreAppend;

                //Set results
                if (appManager.curLiveGame.p1_score > appManager.curLiveGame.p2_score)
                {
                    resultText.text = "YOU WON!";
                }
                else
                {
                    resultText.text = appManager.curLiveGame.player2_name + " WON!";
                }
            }

           if(appManager.curLiveGame.p1_score == appManager.curLiveGame.p2_score)
            {
                resultText.text = "TIE GAME!";
            }
        }
        else //NOT an MP Game
        {            //Set YOUR text, but not theirs
            resultText.text = "GAME OVER!";
            cScore = appManager.curLiveGame.p1_score;
            yourText.text = "You " + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;
            theirText.text = " ";
            compHSValue(cScore);
        }
    }

    public void setResultText()
    {

    }

    public void setScoreText()
    {

    }

    void compHSValue(int curScore)
    {
        int savedHS = 0;
        Debug.Log("Getting highscore for: " + appManager.curLiveGame.categoryText);
        savedHS = u_acJsonUtility.instance.getCatHighscore(appManager.curLiveGame.categoryText);
        Debug.Log("Saved v Cur: " + savedHS + " vs. " + curScore);

        if(curScore > savedHS)
        {
            theirText.text = "You set a new high \n score in <color=" + googleGreenHex + ">" + u_acJsonUtility.UppercaseFirst(appManager.curLiveGame.categoryText) + "</color> \n <b><color=" + googleBlueHex + "><size=135>" + curScore +"</size></color></b> \n points!";
            u_acJsonUtility.instance.updateHighScore(appManager.curLiveGame.categoryText, curScore);
        }
        else
        {
            theirText.text = "Your best score in \n <color=" + googleGreenHex + ">" + u_acJsonUtility.UppercaseFirst(appManager.curLiveGame.categoryText) + "</color> is \n <color=" + googleBlueHex + "><size=135><b>" + savedHS + "</b></size></color> \n points!";
        }
    }

    void determineGameAction()
    {
        if (appManager.curLiveGame.p1_Fin && appManager.curLiveGame.p2_Fin)
        {
            Debug.Log("GAME COMPLETE; DELETING FROM DB");
            //Setup an archived 'deadgame' entry
            entity_gamesDead deadGame = new entity_gamesDead();
            deadGame.initGameDead(appManager.curLiveGame);
            //Need to work out which player is what role here and then delete from table
            appManager.playerRoles p1Role = appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated ? appManager.playerRoles.intiated : appManager.playerRoles.challenged;
            appManager.playerRoles p2Role = p1Role == appManager.playerRoles.intiated ? appManager.playerRoles.challenged : appManager.playerRoles.intiated;
            //Delete the game from games live and then delete the two entries from player_game
            appManager.saveDeadGame(deadGame);
            appManager.deleteCurGame();
            appManager.deletePlayerGameEntry(appManager.curLiveGame.player1_id, appManager.curLiveGame.gameID, p1Role);
            appManager.deletePlayerGameEntry(appManager.curLiveGame.player2_id, appManager.curLiveGame.gameID, p2Role);
        }
        else if(appManager.curLiveGame.p1_Fin && !appManager.curLiveGame.p2_Fin)
        {
            appManager.saveCurGame();
        }
    }
	
}

/* if (appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated && appManager.curLiveGame.p1_Fin && appManager.curLiveGame.p2_Fin)
            { //YOU are P1- P2 may or may not be done. 
                

                
            }
            else if (appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated && appManager.curLiveGame.p1_Fin)
            {
                resultText.text = "";
            }
            else if(appManager.devicePlayerRoleInCurGame == appManager.playerRoles.challenged)
            { //YOU are P2 - In this case, P1 HAS to have finished
                if (appManager.curLiveGame.p1_score < appManager.curLiveGame.p2_score)
                {
                    resultText.text = "YOU WON!";
                }
                else
                {
                    resultText.text = appManager.curLiveGame.player2_name + " WON!";
                }
                cScore = appManager.curLiveGame.p2_score;
                yourText.text = "You " + scorePrepend + appManager.curLiveGame.p2_score + scoreAppend;
                theirText.text = appManager.curLiveGame.player1_name + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;
            }*/
