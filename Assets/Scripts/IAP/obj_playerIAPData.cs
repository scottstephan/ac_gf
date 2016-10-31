using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class obj_playerIAPData : MonoBehaviour {
    public class iapData
    {
       public int totalCreditsBought = 0;
       public int unspentCredits = 0;
       public bool adsActive = true; 
    }

    static string iapLocalSavePath;
    static string iapLocalFullSavePath;
    static iapData thisPlayerIAPData = new iapData();

    public static void initPlayerIAPData()
    {
        iapLocalSavePath = Application.persistentDataPath + "/iapPlayerData/";
        string fileName = "playerIAPDATA.json";
        iapLocalFullSavePath = iapLocalSavePath + fileName;

        checkIAPLocalSetup();
    }

	public static void checkIAPLocalSetup()
    {
        Debug.Log("Checking for IAP data: " + iapLocalFullSavePath);

        if (!Directory.Exists(iapLocalSavePath))
            Directory.CreateDirectory(iapLocalSavePath);
        if (!File.Exists(iapLocalFullSavePath))
            saveIAPLocalData();
        
        loadIAPLocalData(); //In either case, load the data after.
    }

    public static void loadIAPLocalData()
    {
        string jsonToLoad = File.ReadAllText(iapLocalFullSavePath);
        thisPlayerIAPData = JsonUtility.FromJson<iapData>(jsonToLoad);
    }

    public static void saveIAPLocalData()
    {
        string jsonToSave = JsonUtility.ToJson(thisPlayerIAPData);
        File.WriteAllText(iapLocalFullSavePath, jsonToSave);
    }

    public static void removeCredit()
    {
        thisPlayerIAPData.unspentCredits--;

        saveIAPLocalData();
    }

    public static void addCredit()
    {
        thisPlayerIAPData.totalCreditsBought++;
        thisPlayerIAPData.unspentCredits++;

        saveIAPLocalData();
    }

    public static void removeAds()
    {
        thisPlayerIAPData.adsActive = false;

        saveIAPLocalData();
    }
}
