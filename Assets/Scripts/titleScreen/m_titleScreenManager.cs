using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class m_titleScreenManager : MonoBehaviour {

	public void startGameFromTitle()
    {
        Debug.Log("Button clicked; starting game from title");
        SceneManager.LoadScene(1);  
    }
}
