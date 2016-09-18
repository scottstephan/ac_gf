using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class m_headerManager : MonoBehaviour {
    public static m_headerManager instance = null;
    public Text playerName;
    public RawImage playerImage;
    public Image fbLoginButton;
    public GameObject loggedInInfoParent;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

	// Use this for initialization
	void Start () {
	
	}

    public void setHeaderInfoCallback(string name)
    {
        playerName.text = name;
    }

    public void setHeaderToLoggedIn()
    {
        // m_fbStatusManager.instance.LoadPlayerPic();
        playerName.text = appManager.instance.FB_NAME;
        loggedInInfoParent.SetActive(true);
        fbLoginButton.gameObject.SetActive(false);
        m_titleScreenManager.instance.mpButton.interactable = true;
    }

    public void setHeaderToLoggedOut()
    {
        loggedInInfoParent.SetActive(false);
        fbLoginButton.gameObject.SetActive(true);
    }

    public void onFBLoginButtonPush()
    { 
        m_fbStatusManager.instance.promptForUserFBLogin(m_fbStatusManager.loginRequestSource.header);
    }

    public void loginCallback(bool loginStatus)
    {
        if (loginStatus)
        {
            setHeaderToLoggedIn();
        }
        else
        {
            //Error dialog
        }
    }

    // Update is called once per frame
    void Update () {
	
	}
}
