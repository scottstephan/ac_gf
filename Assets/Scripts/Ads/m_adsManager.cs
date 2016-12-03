using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class m_adsManager : MonoBehaviour {
    static public m_adsManager instance = null;
    public bool useAds = true;
    // Use this for initialization

    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void ShowAd()
    {
        if(obj_playerIAPData.getAdStatus() == false)
        {
            return;
        }

        if (Advertisement.IsReady() && useAds)
        {
            Advertisement.Show();
        }
    }

    public void checkAdStatus()
    {
        //load json iap, flag ads on/off
    }
}
