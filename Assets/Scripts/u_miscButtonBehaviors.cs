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
        hideDebug,
        restorePurchases,
        backToTitle,
        showHighScore
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
                if (m_phaseManager.instance.thisPhase == m_phaseManager.phases.titleScreen)
                    m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.menuToIAPShop);
                else if (m_phaseManager.instance.thisPhase == m_phaseManager.phases.categorySelectSP || m_phaseManager.instance.thisPhase == m_phaseManager.phases.categorySelectMP)
                    m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.categoryToIAPShop);
                break;
            case buttonBehaviors.hideShop:
                if (m_phaseManager.instance.thisPhase == m_phaseManager.phases.titleScreen)
                    m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.IAPToMenu);
                else if (m_phaseManager.instance.thisPhase == m_phaseManager.phases.categorySelectSP || m_phaseManager.instance.thisPhase == m_phaseManager.phases.categorySelectMP)
                    m_panelManager.instance.animatePanelsByPhase(m_panelManager.phaseTransitions.IAPToCatSelect);
                m_iapShopPanelManager.instance.destroyLockedCatList();
                break;
            case buttonBehaviors.showFriends:
                m_phaseManager.instance.changePhase(m_phaseManager.phases.friendPanel);
                break;
            case buttonBehaviors.hideFriends:
                m_panelManager.instance.animateUIPanelByPhase(m_panelManager.uiPanelTransitions.playerInputToTop);
                m_panelManager.instance.mpLobby.toMiddle.animationParts.ObjectState = UITween.AnimationParts.State.CLOSE;
                m_panelManager.instance.mpLobby.toMiddle.OpenCloseObjectAnimation();

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
                Debug.Log("TRYING TO BUY CATEGORY CREDITS");
                if (catName != null)
                    evaluateIAPPath();
                else
                    Debug.Log("PURCHASE FAILURE: NO CATEGORY ATTACHED TO BUTTON");
                break;
            case buttonBehaviors.buyAdRemoval:
                Debug.Log("TRYING TO BUY AD REMOVAL");
                appManager.iapManager.BuyProductID(u_iapManager.androidIAPID.ac_gp_noads.ToString(), null);
                break;
            case buttonBehaviors.showDebug:
                m_panelManager.instance.anim_debugToCenter();
                break;
            case buttonBehaviors.hideDebug:
                m_panelManager.instance.anim_debugToTop();
                break;
            case buttonBehaviors.restorePurchases:
                u_iapManager.restorePurchases_iOS();
                break;
            case buttonBehaviors.backToTitle:
                break;
            case buttonBehaviors.showHighScore:
                m_phaseManager.instance.changePhase(m_phaseManager.phases.toHighScore);   
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
