using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using Facebook.Unity;
using Assets.autoCompete.games;
using UnityEngine.SceneManagement;
using DDBHelper;

public class ui_existingGameButton : MonoBehaviour {
    public Text statusText;
    public Text opponentText;
    public Text categoryText;
    public RawImage opponentImage;

    public Button uiButtonManager;
    public string gameID; //This comes from m_MP_Lobby which gets it from the devicePlayer's entry in player_games

    public appManager.playerRoles devicePlayerRole;
    public entity_games thisGame;
    public GameObject parentCanvasTransform;

    public appManager.E_lobbyGameStatus thisGameStatus;
    string opponentName;
    string opponentID;

    public Color yourTurnColor;
    public Color theirTurnColor;
    public Color viewScoreColor;
    bool continueExecute = true;
    public void setUpButton()
    {
        setColorBlock();
        setPlayerNames();
        setCategoryName();
        LoadPlayerPic(opponentID, false);
        gameObject.transform.SetParent(m_MPLobby_Matchmake.instance.fullGameListParentGrid.transform, false);
    }

    void setColorBlock()
    {
        ColorBlock tCB = uiButtonManager.colors;
        if (thisGameStatus == appManager.E_lobbyGameStatus.init_viewScore)
            tCB.normalColor = viewScoreColor;
        else if (thisGameStatus == appManager.E_lobbyGameStatus.init_viewFinal)
            tCB.normalColor = theirTurnColor;
        else if (thisGameStatus == appManager.E_lobbyGameStatus.challenged_playGame)
            tCB.normalColor = yourTurnColor;

        uiButtonManager.colors = tCB;
    }

    void setCategoryName()
    {
       categoryText.text = u_acJsonUtility.UppercaseFirst(thisGame.categoryText) + " (<i>" + setStatusText() + "</i>)";
    }

    void setPlayerNames()
    {
        opponentText.text = opponentName;
    }

    string setStatusText() {

        string status = "";

        if (thisGameStatus == appManager.E_lobbyGameStatus.init_viewScore)
            // status = "Waiting for" + thisGame.player2_name+ "";
            status = "Their turn";
        else if (thisGameStatus == appManager.E_lobbyGameStatus.init_viewFinal)
            status = "See who won";
        else if (thisGameStatus == appManager.E_lobbyGameStatus.challenged_playGame)
            status = "Your turn";

        return status;
    }

    public void onButtonClick()
    {
        //If info is loaded etc....
        appManager.setCurGame(thisGame, devicePlayerRole);
     //   appManager.loadGameQuestions();
        appManager.setCurGameQuestionDetails(thisGame.categoryID, thisGame.categoryText, thisGame.questionID, thisGame.questionText);
        //IF role is init AND has finished AND NOT has seen, go to scoreComp....
        if (thisGameStatus == appManager.E_lobbyGameStatus.init_viewScore)
            m_phaseManager.instance.changePhase(m_phaseManager.phases.scoreComp);
        //IF role is init annd P2 has finished, view final score
        else if (thisGameStatus == appManager.E_lobbyGameStatus.init_viewFinal)
            m_phaseManager.instance.changePhase(m_phaseManager.phases.scoreComp);
        //Else if role is challenger AND
        else if (thisGameStatus == appManager.E_lobbyGameStatus.challenged_playGame)
            m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundMP);

    }

    public void loadGameEntity(string gameID)
    {
        Debug.Log("***LOAD GAME ENTITY FOR " + gameID + "***");
        entity_games tG = new entity_games();
        tG.gameID = gameID;
        tG.gameState = appManager.E_storedGameStates.unstarted.ToString(); //In this case, all games from the lobby are MP games
        DBWorker.Instance.Load<entity_games>(tG, gameLoadComplete);
    }

    void gameLoadComplete(entity_games response, GameObject obj, string nextMethod, Exception e = null)
    {
        if(e != null)
        {
            DBTools.PrintException("gameLoadComplete", e);
        }
        Debug.Log("***GAME ENTITY LOAD COMPLETE***");
        thisGame = response;

        determineGameState();
    }

    public void determineGameState()
    {
        //Determine player role
        if (appManager.FB_ID == thisGame.player1_id)
            appManager.devicePlayerRoleInCurGame = appManager.playerRoles.intiated;
        else if (appManager.FB_ID == thisGame.player2_id)
            appManager.devicePlayerRoleInCurGame = appManager.playerRoles.challenged;

        devicePlayerRole = appManager.devicePlayerRoleInCurGame;
        // Some sort logic:
        //If my role is init && p2 is not finished, I can view score, but am waiting
        //If my role is init && p2 IS finished, I need to view final comp and mark me as done
        if (devicePlayerRole == appManager.playerRoles.intiated)
        {
            thisGameStatus = thisGame.p2_Fin ? appManager.E_lobbyGameStatus.init_viewFinal : appManager.E_lobbyGameStatus.init_viewScore;
            opponentName = thisGame.player2_name;
            opponentID = thisGame.player2_id;
        }
        //If my role is challenged && p2 is not finished, I need to play
        
        else if (devicePlayerRole == appManager.playerRoles.challenged)
        {
            if (!thisGame.p2_Fin)
                thisGameStatus = appManager.E_lobbyGameStatus.challenged_playGame;
            else if (thisGame.p2_Fin && !thisGame.p2HasViewedResult)
                thisGameStatus = appManager.E_lobbyGameStatus.init_viewFinal; //Abandoned
            else if (thisGame.p2_Fin && thisGame.p2HasViewedResult)
            {
                Destroy(gameObject); //No need for this to hang around!
                Destroy(this);
                continueExecute = false;
            } 
            opponentName = thisGame.player1_name;
            opponentID = thisGame.player1_id;
        }

        appManager.curGameStatus = thisGameStatus;
        if(continueExecute)
            setUpButton();
    }

    public void LoadPlayerPic(string oppID, bool needToSave = false)
    {
        string getUserPicString = oppID + "?fields=picture.height(150)";
        FB.API(getUserPicString, HttpMethod.GET,
            delegate (IGraphResult result)
            {
                if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
                {
                    IDictionary picData = result.ResultDictionary["picture"] as IDictionary;
                    IDictionary data = picData["data"] as IDictionary;
                    string picURL = data["url"] as string;
                    StartCoroutine(GetProfilePicRoutine(picURL, needToSave));

                }
            });
    }


    private IEnumerator GetProfilePicRoutine(string url, bool needToSave = false)
    {
        WWW www = new WWW(url);
        yield return www;
        LoadOrSavePicture(www.texture, needToSave);
    }

    void LoadOrSavePicture(Texture2D tex, bool needToSave)
    {
        opponentImage.texture = tex;
    }
}
