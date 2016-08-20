using UnityEngine;
using System.Collections;
using Assets.autoCompete.players;
using Assets.autoCompete.games;
using UnityEngine.SceneManagement;

public class appManager : MonoBehaviour {
    public static appManager instance = null;
    public static entity_players devicePlayer;

    public static entity_games curLiveGame; 

    public  enum sceneNames
    {
        title,
        categorySelect,
        singlePlayer,
        multiPlayerLobby,
        multiplayerCategorySelect
    }

    public static string currentPlayerID;

    
    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    } 

    
    void Start () {
       /* if (devicePlayer == null) {
            acDBHelper.instance.loadPlayerFromDynamoViaID(m_prefsDataManager.getPlayerIDPref(), (bool playerLoaded, entity_players tPlayer) =>{
                devicePlayer = tPlayer;
            });
        }*/
	}

    public static string generateUniqueID()
    {
        string uniqueDeviceID = SystemInfo.deviceUniqueIdentifier;
        return uniqueDeviceID;
    }

    public static string generateUniqueGameID()
    {
        return devicePlayer.playerID + Random.Range(0, 10000); //The odds are in my faor, but still. Use datetime!
    }

    public static void createGameObject()
    {
        curLiveGame = new entity_games();
    }

    public static void loadScene(sceneNames sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad.ToString());
    }

    public static void saveCurGame()
    {
        if(curLiveGame == null)
        {
            Debug.Log("NO GAME SET IN APP MANAGER; NOT SAVING GAME");
            return;
        }

        acDBHelper.instance.saveGameToDyanmo(curLiveGame);
    }
}
