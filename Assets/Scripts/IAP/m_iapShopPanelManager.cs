﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

public class m_iapShopPanelManager : MonoBehaviour {
    public static m_iapShopPanelManager instance = null;

    public EasyTween toMid;
    public EasyTween toTop;
    public GameObject catIAPButton;
    public GameObject noAdsIAP;
    public RectTransform parentCatIAPList;
    public List<GameObject> curLockedCats = new List<GameObject>();
    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void initIAPShop()
    {
        listAllLockedCats();
    }

    public void refreshIAPStore()
    {
        destroyLockedCatList();
        curLockedCats.Clear();
        listAllLockedCats();
    }

    public void destroyLockedCatList()
    {
        foreach (Transform t in parentCatIAPList.transform)
            Destroy(t.gameObject);
    }

    public void listAllLockedCats()
    {
        curLockedCats.Clear();

        GameObject btn_noAds = Instantiate(noAdsIAP);
        btn_noAds.transform.SetParent(parentCatIAPList);

        List<u_acJsonUtility.categoryUnlockInfo> catUnlockStatus = new List<u_acJsonUtility.categoryUnlockInfo>();
        catUnlockStatus = u_acJsonUtility.instance.discoverAllCategoryUnlockInfo();
        for(int i = 0; i < catUnlockStatus.Count; ++i)
        {
            if(catUnlockStatus[i].unlockStatus == "locked")
            {
                Debug.Log(catUnlockStatus[i].categoryName + " is " + catUnlockStatus[i].unlockStatus);
                GameObject tButton = Instantiate(catIAPButton);
                tButton.transform.SetParent(parentCatIAPList);
                tButton.GetComponentInChildren<Text>().text = catUnlockStatus[i].categoryDisplayName + "\n" + "$.99";
                tButton.GetComponent<obj_IAPCatButtonManager>().setUpButton(catUnlockStatus[i]);
                curLockedCats.Add(tButton);
            }
        }
    }

   
}
