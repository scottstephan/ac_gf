using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.autoCompete.players;

public class ui_opponentButtonManager : MonoBehaviour {
    public Text opButtonText;
    string opID;
    public entity_players opEntity;
    public RawImage playerPic;
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
        LoadPlayerPic(false);
    }

   public void onButtonCLick() 
    {
        Debug.Log("---GAME BUTTON CLICKED---");
        //Create game obj n appManager
        appManager.createGameObject(appManager.devicePlayer.playerID,opEntity.playerID,appManager.devicePlayer.playerName,opEntity.playerName,true);
        appManager.curGameStatus = appManager.E_lobbyGameStatus.init_playGame;
        m_phaseManager.instance.changePhase(m_phaseManager.phases.categorySelectMP);
    }

    public void LoadPlayerPic(bool needToSave = false)
    {
        string getUserPicString =   opEntity.playerID + "?fields=picture.height(100)";
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
        
            playerPic.texture = tex;
        
        //  playerDp.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }


}
