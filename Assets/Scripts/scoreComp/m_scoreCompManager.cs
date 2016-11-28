﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.autoCompete.games;

public class m_scoreCompManager : MonoBehaviour {
    public static m_scoreCompManager instance = null;
    private int cScore;

    public Text resultText;
    public Text yourText;
    public Text theirText;

    string scoreColor = "#4470E6FF";
    string categoryColor = "#08AF22FF";

    public GameObject listParent;
    public GameObject[] listItemsToInstantiate;

    public enum mpGameState
    {
       waitingForP2ToFinishAndViewResult, //Most common- P1 is done, but waiting for P2 to play
       waitingForP2ToViewResult, //In the case of an abandon from P2
       waitingForP1ToViewResult, //P2 played, waiting for P1 to see the result
       allPlayersHaveSeenResult,
       singlePlayerGame
    }

    public mpGameState thisGameState;
  //  public Text highScoreStatus; //Not using this. 

    // Use this for initialization
    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void initSC()
    {
        determineGameState();
        updateScoreColors();
        updatePlayerScore(thisGameState);
        createScoreScreenList();
        if (appManager.curGameStatus != appManager.E_lobbyGameStatus.init_viewScore && thisGameState != mpGameState.singlePlayerGame)
            determineGameAction();
        else
            m_loadPanelManager.instance.deactivateLoadPanel();
    }

    public void createScoreScreenList()
    {
        for(int i = 0; i < listItemsToInstantiate.Length; i++)
        {
            listItemsToInstantiate[i].transform.SetParent(listParent.transform);
        }
    }

    public void updateScoreColors()
    {
       categoryColor = u_acJsonUtility.instance.loadCategoryData(appManager.curLiveGame.categoryText).categorColorValue;
    }

    void determineGameState()
    {
        if (!appManager.curLiveGame.isMPGame)
        {
            thisGameState = mpGameState.singlePlayerGame;
            return;
        }

        if(appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated)
        {//I am P1
            if (appManager.curLiveGame.p2_Fin)
            {
                appManager.curLiveGame.p1HasViewedResult = true; //Because we both have scores in
                if (appManager.curLiveGame.p2HasViewedResult)
                {
                    thisGameState = mpGameState.allPlayersHaveSeenResult;
                }
                else
                {
                    thisGameState = mpGameState.waitingForP2ToViewResult;
                }
            }
            else
            {
                thisGameState = mpGameState.waitingForP2ToFinishAndViewResult;
            }
        }
        else if(appManager.devicePlayerRoleInCurGame == appManager.playerRoles.challenged)
        {
            appManager.curLiveGame.p2HasViewedResult = true; //Because we both have scores in

            if (appManager.curLiveGame.p1HasViewedResult)
            {//I am P2 and I abandoned and now I am seeing the final
                appManager.curLiveGame.p2HasViewedResult = true;
                thisGameState = mpGameState.allPlayersHaveSeenResult;
            }
            else
            {
                thisGameState = mpGameState.waitingForP1ToViewResult;
            }
        }

        Debug.Log("Gamestate: " + thisGameState.ToString());
    }

    void updatePlayerScore(mpGameState state)
    {
        string scorePrepend = "got \n <color=" + scoreColor + "><size=135><b>";
        string scoreAppend = "</b></size></color> \n points!";
        string waitingPrepend = "Waiting for <color=" + categoryColor + ">";
        string waitingAppend = "</color> to finish";

        switch (state)
        {
            case mpGameState.allPlayersHaveSeenResult://I am P1, viewing the final result and ending the game
                yourText.text = "You" + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;
                theirText.text = appManager.curLiveGame.player2_name + scorePrepend + appManager.curLiveGame.p2_score + scoreAppend;

                if (appManager.curLiveGame.p1_score > appManager.curLiveGame.p2_score)
                {
                    resultText.text = "YOU WON!";
                }
                else
                {
                    resultText.text = appManager.curLiveGame.player2_name + " WON!";
                }
                break;
            case mpGameState.waitingForP1ToViewResult: //I must be P2, Viewing + Waiting for P1 to view and end
                yourText.text = "You" + scorePrepend + appManager.curLiveGame.p2_score + scoreAppend;
                theirText.text = appManager.curLiveGame.player1_name + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;

                if (appManager.curLiveGame.p2_score > appManager.curLiveGame.p1_score)
                {
                    resultText.text = "YOU WON!";
                }
                else
                {
                    resultText.text = appManager.curLiveGame.player1_name + " WON!";
                }
                break;
            case mpGameState.waitingForP2ToViewResult: //I must be P1, Viewing + Waiting for P2 to view because they abandoned
                break;
            case mpGameState.waitingForP2ToFinishAndViewResult: //I must be P1, Viewing + Waiting for P2 to Play
                yourText.text = "You " + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;
                cScore = appManager.curLiveGame.p1_score;
                theirText.text = waitingPrepend + appManager.curLiveGame.player2_name + waitingAppend;
                resultText.text = "";
                break;
            case mpGameState.singlePlayerGame:
                resultText.text = "GAME OVER";
                cScore = appManager.curLiveGame.p1_score;
                yourText.text = "You " + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;
                theirText.text = " ";
                compHSValue(cScore);
                break;
        }
    }

    void compHSValue(int curScore)
    {
        int savedHS = 0;
        Debug.Log("Getting highscore for: " + appManager.curLiveGame.categoryText);
        savedHS = u_acJsonUtility.instance.getCatHighscore(appManager.curLiveGame.categoryText);
        Debug.Log("Saved v Cur: " + savedHS + " vs. " + curScore);

        if(curScore > savedHS)
        {
            theirText.text = "You set a new high \n score in <color=" + categoryColor + ">" + u_acJsonUtility.UppercaseFirst(appManager.curLiveGame.categoryText) + "</color> \n <b><color=" + scoreColor + "><size=135>" + curScore +"</size></color></b> \n points!";
            u_acJsonUtility.instance.updateHighScore(appManager.curLiveGame.categoryText, curScore);
        }
        else
        {
            theirText.text = "Your best score in \n <color=" + categoryColor + ">" + u_acJsonUtility.UppercaseFirst(appManager.curLiveGame.categoryText) + "</color> is \n <color=" + scoreColor + "><size=135><b>" + savedHS + "</b></size></color> \n points!";
        }
    }

    void determineGameAction()
    {
        if (thisGameState == mpGameState.allPlayersHaveSeenResult)
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
        else
        {//No need to save if we just viewed the score. 
            if (appManager.curGameStatus != appManager.E_lobbyGameStatus.init_viewScore)
                appManager.saveCurGame();
        }
    }

    ///DEPRECATED
    /*    void updatePlayerView()
    {
        if (appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated)
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
        string scorePrepend = "got \n <color=" + scoreColor + "><size=135><b>";
        string scoreAppend = "</b></size></color> \n points!";
        string waitingPrepend = "Waiting for <color=" + categoryColor + ">";
        string waitingAppend = "</color> to finish";

        if (appManager.curLiveGame.isMPGame)
        { 
           if(appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated && appManager.curLiveGame.p1_Fin && !appManager.curLiveGame.p1HasViewedResult && !appManager.curLiveGame.p2_Fin && !appManager.curLiveGame.p2HasViewedResult)
            {//I am P1 and I am done, but am waiting on P2
                yourText.text = "You " + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;
                cScore = appManager.curLiveGame.p1_score;
                theirText.text = waitingPrepend + appManager.curLiveGame.player2_name + waitingAppend;
                resultText.text = "";
            }
           else if(appManager.devicePlayerRoleInCurGame == appManager.playerRoles.challenged && appManager.curLiveGame.p2_Fin && appManager.curLiveGame.p2HasViewedResult)
            {//I am P2 and just finished. I know the result, but P1 doesn't. 
                yourText.text = "You" + scorePrepend + appManager.curLiveGame.p2_score + scoreAppend;
                theirText.text = appManager.curLiveGame.player1_name + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;

                if (appManager.curLiveGame.p2_score > appManager.curLiveGame.p1_score)
                {
                    resultText.text = "YOU WON!";
                }
                else
                {
                    resultText.text = appManager.curLiveGame.player1_name + " WON!";
                }

            }
           else if(appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated && appManager.curLiveGame.p1_Fin && appManager.curLiveGame.p1HasViewedResult && appManager.curLiveGame.p2_Fin && appManager.curLiveGame.p2HasViewedResult)
            {//P2 has seen results, but I am P1 and have not. Now we both have, so delete 
                yourText.text = "You" + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;
                theirText.text = appManager.curLiveGame.player2_name + scorePrepend + appManager.curLiveGame.p2_score + scoreAppend;

                if (appManager.curLiveGame.p1_score > appManager.curLiveGame.p2_score)
                {
                    resultText.text = "YOU WON!";
                }
                else
                {
                    resultText.text = appManager.curLiveGame.player2_name + " WON!";
                }
            }
            else if (appManager.devicePlayerRoleInCurGame == appManager.playerRoles.challenged && appManager.curLiveGame.p1_Fin && appManager.curLiveGame.p1HasViewedResult && appManager.curLiveGame.p2_Fin && appManager.curLiveGame.p2HasViewedResult)
            {
                //P1 has seen results, but I am P2 and have not. Now we both have, so delete - Most likely result of ditching out on a game
                yourText.text = appManager.curLiveGame.player2_name + scorePrepend + appManager.curLiveGame.p2_score + scoreAppend;
                theirText.text = "You" + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;

                if (appManager.curLiveGame.p2_score > appManager.curLiveGame.p1_score)
                {
                    resultText.text = "YOU WON!";
                }
                else
                {
                    resultText.text = appManager.curLiveGame.player1_name + " WON!";
                }
            }
           /*
            if (appManager.curLiveGame.p1_score == appManager.curLiveGame.p2_score)
            {
                resultText.text = "TIE GAME!";
            }
}
        else //NOT an MP Game
        {//Set YOUR text, but not theirs
            resultText.text = "GAME OVER";
            cScore = appManager.curLiveGame.p1_score;
            yourText.text = "You " + scorePrepend + appManager.curLiveGame.p1_score + scoreAppend;
            theirText.text = " ";
            compHSValue(cScore);
        }
    }*/
	
}


