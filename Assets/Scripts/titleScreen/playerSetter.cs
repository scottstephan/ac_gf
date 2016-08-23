using UnityEngine;
using System.Collections;

public class playerSetter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    if(appManager.currentPlayerID == null)
        {
            //First look in GameDataManager...
            string id = m_prefsDataManager.getPlayerIDPref();
            Debug.Log("Returned ID: " + id);
            //Then confirm the device
            if (m_prefsDataManager.confirmCurrentUserIsStoredUser(id))
                appManager.currentPlayerID = id;
            else
            {
                appManager.currentPlayerID = appManager.generateUniquePlayerID();
                m_prefsDataManager.setPlayerIDPref(appManager.currentPlayerID);
            }
            //Check AWS connect.

            //If we're good, check to see if this is a known user in our DB
            
            //If we're not, disable MP and set the player object to local refs

            //If we're AWS good && have a known user, load their record in for local use in entity_player

            //If so
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void findPlayerID()
    {

    }
}
