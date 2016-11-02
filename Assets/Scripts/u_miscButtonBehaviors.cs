using UnityEngine;
using System.Collections;

public class u_miscButtonBehaviors : MonoBehaviour {
    public enum buttonBehaviors{
        showShop,
        hideShop,
        showFriends,
        hideFriends,
        showAd,
        showSettings,
        hideSettings,
        buyCategoryCredits,
        buyAdRemoval
    }

    public buttonBehaviors myButtonBehavior;
    [System.NonSerialized]
    public string catName;

	public void OnClick()
    {
        switch (myButtonBehavior)
        {
            case buttonBehaviors.showShop:
                m_iapShopPanelManager.instance.toMid.OpenCloseObjectAnimation();
                m_iapShopPanelManager.instance.initIAPShop();
                m_loadPanelManager.instance.activateLoadPanel();
                break;
            case buttonBehaviors.hideShop:
                m_iapShopPanelManager.instance.toTop.OpenCloseObjectAnimation();
                m_loadPanelManager.instance.deactivateLoadPanel();
                break;
            case buttonBehaviors.showFriends:
                m_panelManager.instance.animateUIPanelByPhase(m_panelManager.uiPanelTransitions.playerInputToCenter);
                m_loadPanelManager.instance.activateLoadPanel();
                break;
            case buttonBehaviors.hideFriends:
                m_panelManager.instance.animateUIPanelByPhase(m_panelManager.uiPanelTransitions.playerInputToTop);
                m_loadPanelManager.instance.deactivateLoadPanel();
                break;
            case buttonBehaviors.showAd:
                m_adsManager.instance.ShowAd();
                break;
            case buttonBehaviors.showSettings:
                break;
            case buttonBehaviors.hideSettings:
                break;
            case buttonBehaviors.buyCategoryCredits:
                Debug.Log("TRYING TO BUY CAT CREDITS");
                if (catName != null)
                    evaluateIAPPath();
                else
                    Debug.Log("PURCHASE FAILURE: NO CATEGORY ATTACHED TO BUTTON");
                break;
            case buttonBehaviors.buyAdRemoval:
                Debug.Log("TRYING TO BUY AD REMOVAL");
                appManager.iapManager.BuyProductID(u_iapManager.IAPTypes.noAds.ToString(),null);
                break;
        }
    }

    public void evaluateIAPPath()
    {
        if (obj_playerIAPData.getCreditBalance() > 0)
        {
            Debug.Log("PLAYER HAS CREDITS; PROCEEDING TO UNLOCK");
            obj_playerIAPData.removeCredit();
            u_acJsonUtility.instance.findAndUnlockCategory(catName);
        }
        else
            appManager.iapManager.BuyProductID(u_iapManager.IAPTypes.categoryCredit.ToString(), catName);
    }
}
