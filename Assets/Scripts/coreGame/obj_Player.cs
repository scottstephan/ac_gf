using UnityEngine;
using System.Collections;

public class obj_Player : MonoBehaviour {
    public static obj_Player instance;

    public string playerName;
    public int numMisses;
    public int numHits;
    public int totalScore;

    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void setName(string PName)
    {
        playerName = PName;
    }

    public void resetPlayer()
    {
        playerName = "";
        numMisses = 0;
        numHits = 0;
        totalScore = 0;
    }

    public void playerMissed()
    {
        Debug.Log("Player missed");
        numMisses++;
        m_scoreAndGameStateManager.instance.updateNumberMissedText(numMisses);

        if (numMisses >= gameManager.maxPlayerMisses)
        {
            Debug.Log("PLAYER HAS EXCEEDED MAX MISSES; ENDING GAME");
            gameManager.instance.endGame(true);
        }
    }

    public void playerHit(int score)
    {
        numHits++;
        totalScore += score;
        m_scoreAndGameStateManager.instance.updateScoreText(totalScore.ToString());

        if(numHits == 10)
        {
            gameManager.instance.endGame(false);
        }
    }

}
