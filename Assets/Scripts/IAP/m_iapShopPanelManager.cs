using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class m_iapShopPanelManager : MonoBehaviour {
    public static m_iapShopPanelManager instance = null;

    public EasyTween toMid;
    public EasyTween toTop;
    public GameObject catIAPButton;
    public RectTransform parentCatIAPList;
    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    public void initIAPShop()
    {
        listAllLockedCats();
    }

    public void listAllLockedCats()
    {
        List<u_acJsonUtility.categoryUnlockInfo> catUnlockStatus = new List<u_acJsonUtility.categoryUnlockInfo>();
        catUnlockStatus = u_acJsonUtility.instance.discoverAllCategoryUnlockInfo();
        for(int i = 0; i < catUnlockStatus.Count; ++i)
        {
            if(catUnlockStatus[i].unlockStatus == "locked")
            {
                Debug.Log(catUnlockStatus[i].categoryName + " is " + catUnlockStatus[i].unlockStatus);
                GameObject tButton = Instantiate(catIAPButton);
                tButton.transform.SetParent(parentCatIAPList);
                tButton.GetComponentInChildren<Text>().text = catUnlockStatus[i].categoryName + "\n" + "Buy now, 99 cents!";
            }
        }
    }
}
