using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class m_scoreCompManager : MonoBehaviour {
    public Text p1Score;
    public Text p2Score;
    public Text p1Name;
    public Text p2Name;
	// Use this for initialization
	void Start () {
        updatePlayerView();
        updatePlayerScores();
        appManager.updateGameRecord_Manual();
	}

    void updatePlayerView() //DON'T EVER UPDATE UNTIL BOTH PLAYERS ARE DONE. 
    { //Possible issue here where two players may intersect.
     /*   if(appManager.devicePlayerRoleInCurGame == appManager.playerRoles.intiated)
        {
            appManager.curLiveGame.p1HasViewedResult = false;
        }
        else if (appManager.devicePlayerRoleInCurGame == appManager.playerRoles.challenged)
        {
            appManager.curLiveGame.p2HasViewedResult = true; //As its V. LIKELY P1 will have seen
        } */
    }

    void updatePlayerScores()
    {
        p1Score.text = appManager.curLiveGame.p1_score.ToString();
        p2Score.text = appManager.curLiveGame.p2_score.ToString();
        string namePrefx = "";
        string nameSuffix = " score:";
        p1Name.text = namePrefx + appManager.curLiveGame.player1_name + nameSuffix;
        p2Name.text = namePrefx + appManager.curLiveGame.player2_name + nameSuffix;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
