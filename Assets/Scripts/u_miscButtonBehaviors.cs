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
        hideSettings
    }

    public buttonBehaviors myButtonBehavior;

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
        }
    }
}
