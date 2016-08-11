using UnityEngine;
using System.Collections;

public class obj_Player : MonoBehaviour {
    public string playerName;
    public int numMisses;
    public int numHits;
    public int totalScore;

    public void setName(string PName)
    {
        playerName = PName;
    }

    public void playerMissed()
    {
        numMisses++;
        if(numMisses >= gameManager.maxPlayerMisses)
        {
            Debug.Log("PLAYER HAS EXCEEDED MAX MISSES; ENDING GAME");
            gameManager.endGame();
        }
    }

    public void playerHit(int score)
    {
        numHits++;
        totalScore += score;
    }

}
