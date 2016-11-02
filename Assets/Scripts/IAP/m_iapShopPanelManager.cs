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
        for(int i = 0; i < curLockedCats.Count; i++)
        {
            Destroy(curLockedCats[i]);
        }
        curLockedCats.Clear();
        listAllLockedCats();
    }

    public void listAllLockedCats()
    {
        curLockedCats.Clear();

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
                tButton.GetComponentInChildren<u_miscButtonBehaviors>().catName = catUnlockStatus[i].categoryName;
                curLockedCats.Add(tButton);
            }
        }
    }
}
