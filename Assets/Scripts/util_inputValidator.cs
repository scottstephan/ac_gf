using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class util_inputValidator : MonoBehaviour
{
    public InputField playerInputField;
    
    public void Start()
    {
        playerInputField.onEndEdit.AddListener(delegate { acceptPlayerInput(); });
    }

    public void acceptPlayerInput()
    {
        string playerInput = playerInputField.text;
        Debug.Log("Player input: " + playerInput);
        playerInputField.text = "";
        //Validate & process
        //Pass to GM for logic
        gameManager.checkPlayerInputAgainstAnswers(playerInput);
    }
}
