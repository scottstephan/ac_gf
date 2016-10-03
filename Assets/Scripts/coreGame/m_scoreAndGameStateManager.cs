using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class m_scoreAndGameStateManager : MonoBehaviour {
    public static m_scoreAndGameStateManager instance = null;

    public Text scoreText;
    string scorePrefix = "SCORE: ";
    int scoreValue;

    public Text roundStatusText;
    public Text numberOfMissesText;
    string numMissesPrefix = "Num. Missed: ";

    public string roundStartText = "Round Start!";
    public string roundEndWithInputText = "Round End!";
    public string roundEndWithTimeoutText = "Time over.";
    public string roundEndWithPlayerHit = "You guessed right";
    public string roundEndWithPlayerMiss = "You guessed wrong";
    public string roundEndWithPlayerHitMax = "Guessed all 10! You win!";
    public string roundEndWithPlayerHitTwice = "Already guessed that answer!";
    public string gameEndText = "Game over";
    public string gameStartText = "Ready?";

    public Image[] missLightBulbs;
    public Sprite brokenBulbSprite;

    public InputField playerInputField;
    void Start () {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void setInputFieldAccessibility(bool state)
    {
        if(!playerInputField.interactable)
            playerInputField.interactable = state;
    }

    public void updateScoreText(string scoreValueAsString)
    {
        //scoreText.text = scorePrefix + scoreValueAsString;
        scoreText.text = scoreValueAsString;
    }

    public void updateNumberMissedText(int numMissed)
    {
        //numberOfMissesText.text = numMissesPrefix + numMissed.ToString();
        numberOfMissesText.text = "";
    }

    public void resetGameUIState()
    {
        updateScoreText("0");
        updateNumberMissedText(0);
    }

    public void breakBulb(int bulbIndex)
    {
        int index = bulbIndex - 1; //i.e., miss 1 is 0, 2 is 1 etc...
        //Play sfx
        //Play anim
        //Swap sprite
        missLightBulbs[index].GetComponent<Animation>().Play();
        //Continue  
    }

    public void bulbCallback()
    {
        //At end of bulb anim
    }
}
