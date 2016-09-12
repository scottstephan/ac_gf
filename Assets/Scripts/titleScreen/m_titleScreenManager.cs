using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class m_titleScreenManager : MonoBehaviour {

    public static m_titleScreenManager instance = null;

    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void startGameFromTitle(bool isMP)
    {
        Debug.Log("Button clicked; starting game from title");
        if (!isMP)
        {
            appManager.createGameObject(appManager.devicePlayer.playerID, "none", appManager.devicePlayer.playerName, "none", false);
            m_phaseManager.instance.changePhase(m_phaseManager.phases.categorySelectSP);
        }
        else
        {
            Debug.Log("---STARTING MP PROCESS---");
            m_phaseManager.instance.changePhase(m_phaseManager.phases.MPLobby);
        } 
    }

}
