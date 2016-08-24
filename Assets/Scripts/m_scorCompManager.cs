using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.autoCompete.games;

public class m_scorCompManager : MonoBehaviour {
    public Text p1ScoreText;
    public Text p2ScoreText;

	// Use this for initialization
	void Start () {
        p1ScoreText.text = appManager.curLiveGame.p1_score.ToString(); //Make sure we're saving/updating local game in gameManager
        if (appManager.curLiveGame.isMPGame)
        {
            p2ScoreText.text = appManager.curLiveGame.p2_score.ToString();
        }
        else
        {
            p2ScoreText.gameObject.SetActive(false);
        }
    }

    void setScores()
    {

    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
