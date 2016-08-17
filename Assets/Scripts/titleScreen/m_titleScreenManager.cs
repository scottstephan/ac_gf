using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class m_titleScreenManager : MonoBehaviour {

	public void startGameFromTitle(bool isMP)
    {
        Debug.Log("Button clicked; starting game from title");
        if (!isMP) SceneManager.LoadScene(appManager.sceneNames.title.ToString());
        else SceneManager.LoadScene(appManager.sceneNames.multiPlayerLobby.ToString());  
    }
}
