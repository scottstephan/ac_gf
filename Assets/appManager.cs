using UnityEngine;
using System.Collections;
using Assets.autoCompete.players;

public class appManager : MonoBehaviour {
    public static appManager instance = null;
    public static entity_players devicePlayer;

    public enum sceneNames
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
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static string generateUniqueID()
    {
        string uniqueDeviceID = SystemInfo.deviceUniqueIdentifier;
        return uniqueDeviceID;
    }
}
