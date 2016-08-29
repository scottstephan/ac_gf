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

    public InputField playerInputField;
    void Start () {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void setInputFieldAccessibility(bool state)
    {
        playerInputField.interactable = state;
    }

    public void updateScoreText(string scoreValueAsString)
    {
        scoreText.text = scorePrefix + scoreValueAsString;
    }

    public void updateNumberMissedText(int numMissed)
    {
        numberOfMissesText.text = numMissesPrefix + numMissed.ToString();
    }
}
