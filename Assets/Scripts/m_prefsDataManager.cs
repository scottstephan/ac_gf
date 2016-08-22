using UnityEngine;
using System.Collections;

public  static class m_prefsDataManager  {
    
    public enum playerPrefVariables
    {
        playerID
    }

	public static void setPlayerIDPref(string id)
    {
        PlayerPrefs.SetString("playerID", id);
    }

    public static string getPlayerIDPref()
    {
        string retId = PlayerPrefs.GetString(playerPrefVariables.playerID.ToString());
        if (retId == null || retId == "")
        {
            Debug.Log("CAN'T FIND LOCAL PLAYER ID");
            return null;
        }
        else
        {
            Debug.Log("PLAYER KNOWN LOCALLY; RETURNING ID");
            return retId;
        }
    }

    public static bool confirmCurrentUserIsStoredUser(string id)
    {
        string storedID = PlayerPrefs.GetString(playerPrefVariables.playerID.ToString());

        if (storedID == appManager.generateUniquePlayerID())
        {
            Debug.Log("STORED PLAYER MATCHES DEVICE PLAYER");
            return true;
        }
        else
        {
            Debug.Log("STORED PLAYED DOES NOT MATCH DEVICE PLAYER! SETTING ID TO NEW DEVICE");
            return false;
        }

        return false;
    }
}
