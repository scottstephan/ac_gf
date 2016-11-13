using UnityEngine;
using System.Collections;

public class u_miscButtonBehaviors : MonoBehaviour {
    public enum buttonBehaviors{
        showShop,
        hideShop,
        showFriends,
        hideFriends,
        showAd,
        leaveTutorialPanel,
        showSettings,
        hideSettings,
        buyCategoryCredits,
        buyAdRemoval,
        showDebug,
        hideDebug
    }

    public buttonBehaviors myButtonBehavior;
    [System.NonSerialized]
    public string catName;

	public void OnClick()
    {
        switch (myButtonBehavior)
        {
            case buttonBehaviors.showShop:
                m_iapShopPanelManager.instance.refreshIAPStore();
                m_iapShopPanelManager.instance.toMid.OpenCloseObjectAnimation();
                m_loadPanelManager.instance.activateLoadPanel();
                break;
            case buttonBehaviors.hideShop:
                m_iapShopPanelManager.instance.toTop.OpenCloseObjectAnimation();
                m_loadPanelManager.instance.deactivateLoadPanel();
                m_iapShopPanelManager.instance.destroyLockedCatList();
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
            case buttonBehaviors.leaveTutorialPanel:
                if (appManager.curLiveGame.isMPGame)
                    m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundMP);
                else
                    m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundSP);
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
            case buttonBehaviors.showDebug:
                m_panelManager.instance.anim_debugToCenter();
                break;
            case buttonBehaviors.hideDebug:
                m_panelManager.instance.anim_debugToTop();
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
            m_iapShopPanelManager.instance.refreshIAPStore();
            m_categorySelectionManager.instance.initCategoryPhase(); //TO-DO: These refreshes should be in one core f(x) and not here AND in IAPManager

        }
        else
            appManager.iapManager.BuyProductID(u_iapManager.androidIAPID.ac_gp_categorycredit.ToString(), catName);
    }
}
