using UnityEngine;
using System.Collections;

public  static class m_prefsDataManager  {
    
    public enum playerPrefVariables
    {
        playerID,
        playerSearchName
    }

	public static void setPlayerIDPref(string id)
    {
        PlayerPrefs.SetString("playerID", id);
    }

    public static void setPlayerSearchName(string searchName)
    {
        PlayerPrefs.SetString(playerPrefVariables.playerSearchName.ToString(), searchName);
    }

    public static string getPlayerSearchName()
    {
        string sName = PlayerPrefs.GetString(playerPrefVariables.playerSearchName.ToString());
        if(sName == null || sName == "")
        {
            Debug.Log("NO STORED SEARCH NAME");
            return "none"; //Dynamo requires a non-empty value for keys
        }
        else
        {
            Debug.Log("---SEARCHNAME IS STORED LOCALLY: " + sName);
            return sName;
        }
    }

    public static string getPlayerIDPref()
    {
        string retId = PlayerPrefs.GetString(playerPrefVariables.playerID.ToString());
        if (retId == null || retId == "")
        {
            Debug.Log("---CAN'T FIND LOCAL PLAYER ID---");
            return null;
        }
        else
        {
            Debug.Log("---PLAYER KNOWN LOCALLY; RETURNING ID---");
            return retId;
        }
    }

    public static bool confirmCurrentUserIsStoredUser(string id)
    {
        string storedID = PlayerPrefs.GetString(playerPrefVariables.playerID.ToString());

        if (storedID == appManager.generateUniquePlayerID())
        {
            Debug.Log("---STORED PLAYER MATCHES DEVICE PLAYER---");
            return true;
        }
        else
        {
            Debug.Log("---STORED PLAYED DOES NOT MATCH DEVICE PLAYER! SETTING ID TO NEW DEVICE---");
            return false;
        }

        return false;
    }
}
