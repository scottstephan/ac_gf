using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        catSelectToMainRound,
        mainRoundToScoreComp,
        scoreCompToMainMenu,
        scoreCompToMPLobby
    }

    public panelAnimations loadScreen;
    public panelAnimations titleScreen;
    public panelAnimations mpLobby;
    public panelAnimations categorySelect;
    public panelAnimations mainRound;
    public panelAnimations scoreComp;
    public panelAnimations headerPanel;

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
            case phaseTransitions.scoreCompToMainMenu:
                anim_scoreCompToMainMenu();
                break;
            case phaseTransitions.scoreCompToMPLobby:
                anim_scoreCompToMPLobby();
                break;
        }
    }
        
    //Being V. EXPLICIT because of the # of states. We could do a f(x) based on states, but. Whatever. 
    public void anim_loadingToMenu()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(loadScreen.toLeft);
        setToPlay.animsToPlayInOrder.Add(headerPanel.toMiddle);
        setToPlay.animsToPlayInOrder.Add(titleScreen.toMiddle);
        loadScreen.thisPanelPos = panelAnimations.panelPos.left;
        titleScreen.thisPanelPos = panelAnimations.panelPos.center;
        headerPanel.thisPanelPos = panelAnimations.panelPos.center;
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_menuToMPLobby()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(titleScreen.toLeft);
        setToPlay.animsToPlayInOrder.Add(mpLobby.toMiddle);
        titleScreen.thisPanelPos = panelAnimations.panelPos.left;
        mpLobby.thisPanelPos = panelAnimations.panelPos.center;
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_menuToCatSelect()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(titleScreen.toLeft);
        setToPlay.animsToPlayInOrder.Add(categorySelect.toMiddle);
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_mpLobbyToCatSelect()
    {
        animationSetToPlay setToPlay = new animationSetToPlay();
        setToPlay.animsToPlayInOrder.Add(mpLobby.toLeft);
        setToPlay.animsToPlayInOrder.Add(categorySelect.toMiddle);
        StartCoroutine("playAnimSet", setToPlay);
    }

    public void anim_mpLobbyToScoreComp()
    {

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
        setToPlay.animsToPlayInOrder.Add(titleScreen.toMiddle);
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
}

/*  
    public GameObject loadOverlay;
    public GameObject persistentHeader;
    public GameObject loadingPanel;
    public GameObject titlePanel;
    public GameObject mpLobby;
    public GameObject categorySelect;
    public GameObject mainRoundPanel;
    public GameObject scoreCompPanel;

    public class panelSet {
       public EasyTween cPanelTween;
       public EasyTween nPanelTween;
    }


    public enum panelNames
    {
        loadPanel,
        header,
        titlePanel,
        mpLobby,
        categorySelect,
        mainRound,
        scoreCompPanel
    }

    

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public static GameObject getPanelObjectByEnum(panelNames pName)
    {
        switch (pName){
            case panelNames.categorySelect:
                return m_panelManager.instance.categorySelect;
                
            case panelNames.header:
                return m_panelManager.instance.persistentHeader;
                
            case panelNames.loadPanel:
                return m_panelManager.instance.loadingPanel;
                
            case panelNames.mainRound:
                return m_panelManager.instance.mainRoundPanel;
                
            case panelNames.mpLobby:
                return m_panelManager.instance.mpLobby;
                
            case panelNames.scoreCompPanel:
                return m_panelManager.instance.scoreCompPanel;
                
            case panelNames.titlePanel:
                return m_panelManager.instance.titlePanel;
        }
        return null;
    }
    
    public void swapPanels(panelNames cPanel, panelNames nPanel)
    {
        GameObject obj_cPanel = getPanelObjectByEnum(cPanel);
        GameObject obj_nPanel = getPanelObjectByEnum(nPanel);

        m_panelManager.instance.movePanel(obj_cPanel,obj_nPanel);
        
    }
    
    private void movePanel(GameObject panelC,GameObject panelN)
    {
        panelSet curPSet = new panelSet();
        curPSet.cPanelTween = panelC.GetComponent<EasyTween>();
        curPSet.nPanelTween = panelN.GetComponent<EasyTween>();
        StartCoroutine("movePanels", curPSet);
    }

    IEnumerator movePanels(panelSet cSet)
    {
        cSet.cPanelTween.OpenCloseObjectAnimation();
        float delay = cSet.cPanelTween.GetAnimationDuration() / 2;
        yield return new WaitForSeconds(delay);
        cSet.nPanelTween.OpenCloseObjectAnimation();
    }*/
