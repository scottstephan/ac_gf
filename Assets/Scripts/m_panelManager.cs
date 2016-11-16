using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// This class ONLY handles animations. Any additional logic or state concerns are handled in m_phaseManager
/// </summary>
public class m_panelManager : MonoBehaviour {
    public static m_panelManager instance = null;

    [System.Serializable]
    public struct panelAnimations
    {
       public enum panelPos
        {
            left,
            right,
            center
        }

       public panelPos thisPanelPos;
       public EasyTween toLeft;
       public EasyTween toRight;
       public EasyTween toMiddle;
    }

    [System.Serializable]
    public struct uiPanelAnimations
    {
        public enum panelPos
        {
            up,
            center,
            bottom
        }

        public panelPos thisPanelPos;
        public EasyTween toCenter;
        public EasyTween toTop;
        public EasyTween toBottom;

    }

    public class animationSetToPlay{
        public List<EasyTween> animsToPlayInOrder = new List<EasyTween>();
    }

    public enum phaseTransitions
    {
        loadingToMenu,
        menuToMPLobby,
        menuToCatSelect,
        MPLobbyToCatSelect,
        MPLobbyToScoreComp,
        MPLobbyToMainRound,
        catSelectToTutorial,
        TutorialToMainRound,
        catSelectToMainRound,
        mainRoundToScoreComp,
        scoreCompToMainMenu,
        scoreCompToMPLobby,
        categoryToIAPShop,
        menuToIAPShop,
        IAPToMenu,
        IAPToCatSelect
    }

    public enum uiPanelTransitions{
        playerInputToCenter,
        playerInputToTop
    }

    public panelAnimations loadScreen;
    public panelAnimations titleScreen;
    public panelAnimations mpLobby;
    public panelAnimations categorySelect;
    public panelAnimations tutorialPanel;
    public panelAnimations mainRound;
    public panelAnimations scoreComp;
    public panelAnimations headerPanel;
    public panelAnimations IAPPanel;

    public uiPanelAnimations opponentInputPanel;
    public uiPanelAnimations debugPanel;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        loadScreen.thisPanelPos = panelAnimations.panelPos.center;
    }

    public void animatePanelsByPhase(phaseTransitions pT)
    {
        switch (pT)
        {
            case phaseTransitions.catSelectToMainRound:
                anim_catSelectToMainRound();
                break;
            case phaseTransitions.catSelectToTutorial:
                anim_catSelectToTutorial();
                break;
            case phaseTransitions.TutorialToMainRound:
                anim_tutorialToMainRound();
                break;
            case phaseTransitions.loadingToMenu:
                anim_loadingToMenu();
                break;
            case phaseTransitions.mainRoundToScoreComp:
                anim_mainRoundToScoreComp();
                break;
            case phaseTransitions.menuToCatSelect:
                anim_menuToCatSelect();
                break;
            case phaseTransitions.menuToMPLobby:
                anim_menuToMPLobby();
                break;
            case phaseTransitions.MPLobbyToCatSelect:
                anim_mpLobbyToCatSelect();
                break;
            case phaseTransitions.MPLobbyToScoreComp:
                anim_mpLobbyToScoreComp();
                break;
            case phaseTransitions.MPLobbyToMainRound:
                anim_mpLobbyToMainRound();
                break;
            case phaseTransitions.scoreCompToMainMenu:
                anim_scoreCompToMainMenu();
                break;
            case phaseTransitions.scoreCompToMPLobby:
                anim_scoreCompToMPLobby();
                break;
            case phaseTransitions.menuToIAPShop:
                anim_menuToIAP();
                break;
            case phaseTransitions.categoryToIAPShop:
                anim_categoryToIAP();
                break;
            case phaseTransitions.IAPToMenu:
                anim_IAPToMenu();
                break;
            case phaseTransitions.IAPToCatSelect:
                anim_IAPToCatSelect();
                break;
        }
    }
        
    //Being V. EXPLICIT because of the # of states. We could do a f(x) based on states, but. Whatever. 
    public void anim_loadingToMenu()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(titleScreen.toMiddle);
        loadScreen.thisPanelPos = panelAnimations.panelPos.left;
        titleScreen.thisPanelPos = panelAnimations.panelPos.center;
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_menuToMPLobby()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(loadScreen.toLeft); //Is now just the AC logo. Lol.
        setToPlay.animsToPlayInOrder.Add(titleScreen.toLeft);
        setToPlay.animsToPlayInOrder.Add(mpLobby.toMiddle);
        titleScreen.thisPanelPos = panelAnimations.panelPos.left;
        mpLobby.thisPanelPos = panelAnimations.panelPos.center;
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_menuToCatSelect()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(loadScreen.toLeft); //Is now just the AC logo. Lol.
        setToPlay.animsToPlayInOrder.Add(titleScreen.toLeft);
        setToPlay.animsToPlayInOrder.Add(categorySelect.toMiddle);
        StartCoroutine("playAnimsSimultaneously", setToPlay);
    }

    public void anim_mpLobbyToCatSelect()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        if (opponentInputPanel.thisPanelPos == uiPanelAnimations.panelPos.center)
        {
            setToPlay.animsToPlayInOrder.Add(opponentInputPanel.toTop);
            m_loadPanelManager.instance.deactivateLoadPanel();
            m_loadPanelManager.instance.panelText.text = "LOADING!";
        }
    //    setToPlay.animsToPlayInOrder.Add(mpLobby.toLeft);
        setToPlay.animsToPlayInOrder.Add(categorySelect.toMiddle);
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_mpLobbyToScoreComp()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(mpLobby.toLeft);
        setToPlay.animsToPlayInOrder.Add(scoreComp.toMiddle);
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_mpLobbyToMainRound()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(mpLobby.toLeft);
        setToPlay.animsToPlayInOrder.Add(mainRound.toMiddle);
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_catSelectToTutorial()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(categorySelect.toLeft);
        setToPlay.animsToPlayInOrder.Add(tutorialPanel.toMiddle);
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_tutorialToMainRound()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(tutorialPanel.toLeft);
        setToPlay.animsToPlayInOrder.Add(mainRound.toMiddle);
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_catSelectToMainRound()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(categorySelect.toLeft);
        setToPlay.animsToPlayInOrder.Add(mainRound.toMiddle);
        StartCoroutine("playAnimSet", setToPlay);
    }
    
    public void anim_mainRoundToScoreComp()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(mainRound.toLeft);
        setToPlay.animsToPlayInOrder.Add(scoreComp.toMiddle);
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_scoreCompToMainMenu()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(scoreComp.toLeft);
        setToPlay.animsToPlayInOrder.Add(loadScreen.toMiddle); //Is now just the AC logo. Lol.
        setToPlay.animsToPlayInOrder.Add(titleScreen.toMiddle);
        StartCoroutine("playAnimSet", setToPlay);
        m_adsManager.instance.ShowAd();

    }

    public void anim_categoryToIAP()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(categorySelect.toLeft);
        setToPlay.animsToPlayInOrder.Add(IAPPanel.toMiddle);
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_menuToIAP()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(titleScreen.toLeft);
        setToPlay.animsToPlayInOrder.Add(IAPPanel.toMiddle);
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_IAPToMenu()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(IAPPanel.toLeft);
        setToPlay.animsToPlayInOrder.Add(titleScreen.toMiddle);
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_IAPToCatSelect()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(IAPPanel.toLeft);
        setToPlay.animsToPlayInOrder.Add(categorySelect.toMiddle);
        StartCoroutine("playAnimSet", setToPlay);
    }


    public void anim_scoreCompToMPLobby()
    {

    }
    
    IEnumerator playAnimSet(animationSetToPlay cSet)
    {
        float delay = 0;

        for(int i = 0; i < cSet.animsToPlayInOrder.Count; i++)
        {
            cSet.animsToPlayInOrder[i].animationParts.ObjectState = UITween.AnimationParts.State.CLOSE;
            cSet.animsToPlayInOrder[i].OpenCloseObjectAnimation();
            delay = cSet.animsToPlayInOrder[i].GetAnimationDuration() * 0.5f;
            yield return new WaitForSeconds(delay);
        }

        Debug.Log("---ANIM SET DONE---");
    }

    IEnumerator playAnimsSimultaneously(animationSetToPlay cSet)
    {
        for (int i = 0; i < cSet.animsToPlayInOrder.Count; i++)
        {
            cSet.animsToPlayInOrder[i].animationParts.ObjectState = UITween.AnimationParts.State.CLOSE;
            cSet.animsToPlayInOrder[i].OpenCloseObjectAnimation();
        }

        yield return null;
    }

    ///---UI PANEL STUFF---\\\
    public void animateUIPanelByPhase(uiPanelTransitions thisTransition)
    {
        switch (thisTransition)
        {
            case uiPanelTransitions.playerInputToCenter:
                anim_opponentInputToMiddle();
                opponentInputPanel.thisPanelPos = uiPanelAnimations.panelPos.center;
                m_loadPanelManager.instance.activateLoadPanel();
                m_loadPanelManager.instance.setLoadText("");
                break;
            case uiPanelTransitions.playerInputToTop:
                anim_opponentInputToTop();
                break;
        }
    }

    public void anim_opponentInputToMiddle()
    {
        opponentInputPanel.toCenter.OpenCloseObjectAnimation();
    }

    public void anim_opponentInputToTop()
    {
        opponentInputPanel.toCenter.OpenCloseObjectAnimation();
    }

    public void anim_debugToTop()
    {
        debugPanel.toTop.OpenCloseObjectAnimation();
    }

    public void anim_debugToCenter()
    {
        debugPanel.toCenter.OpenCloseObjectAnimation();
    }
}

