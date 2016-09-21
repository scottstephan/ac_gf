using UnityEngine;
using Facebook.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class m_fbStatusManager : MonoBehaviour {
    public  static m_fbStatusManager instance = null;
    public static AccessToken fbToken;
    // Use this for initialization
    public delegate void loginCallback(bool loginStatus);
    public delegate void nameLoadCallback(string name);
    public delegate void userFriendListPopulateCallback(List<object> users);
    public delegate void standardEventCallback();

    public enum loginRequestSource
    {
        header
    }

    public loginRequestSource lastLoginRequestSource = loginRequestSource.header;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        checkFBInit();
    }

	void Start () {
        Debug.Log("---FB MANAGER LIVE---");
	}

    public void returnUserLoginStatus(loginCallback cbF)
    {
        StartCoroutine("loginCheckDelay", cbF);
    }
  
    IEnumerator loginCheckDelay(loginCallback cbF)
    {
        yield return new WaitForSeconds(.5f);
        cbF(FB.IsLoggedIn);
    }

    public void promptForUserFBLogin(loginRequestSource requestSource)
    {
        lastLoginRequestSource = requestSource;
        var perms = new List<string>() { "public_profile", "email", "user_friends" };
        FB.LogInWithReadPermissions(perms, loginAuthCallback);
    }

    void loginAuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("---USER FB LOGIN SUCCESS---");
            fbToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            foreach (string perm in fbToken.Permissions)
            {
                Debug.Log(perm);
            }

            if (lastLoginRequestSource == loginRequestSource.header)
                m_headerManager.instance.setHeaderToLoggedIn();

        }
        else
        {
            Debug.Log("---USER LOGIN UNSUCCESSFUL---");
        }
    }
	
    void checkFBInit()
    {
        if (!FB.IsInitialized)
            FB.Init(FBInitCallback, OnHideUnity);
        else
            FB.ActivateApp();
    }

    void FBInitCallback()
    {
        if (FB.IsInitialized)
        {
            Debug.Log("---FB IS INITIALIZED; ACTIVATING APP & SETTING TOKEN---");
            FB.ActivateApp();
        }
    }

    public string returnFBUserID()
    {
        if (FB.IsLoggedIn)
        {
            Debug.Log("---FB USER IS LOGGED IN---");
            fbToken = Facebook.Unity.AccessToken.CurrentAccessToken; //Otherwise token is set at login time
            Debug.Log("FB TOKEN EXP TIME: " + fbToken.ExpirationTime);
            return fbToken.UserId;
        }
        else
        {
            Debug.Log("---FB USER NOT LOGGED IN---");
            return "ERROR";
        }
    }

    void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            Debug.Log("---FB HIDING GAME---");
        }
        else
        {
            Debug.Log("---FB SHWOING GAME---");
        }
    }


    ///---UTIL DATA LOADING---\\\
#region Various FB data loads
    public void LoadPlayerName(nameLoadCallback cbF)
    {
        string getNameString = "me";
        string returnedName = "";
        FB.API(getNameString, HttpMethod.GET,
            delegate (IGraphResult result)
            {
                if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
                {
                    returnedName = result.ResultDictionary["name"] as string;
                    cbF(returnedName);
                }
                else
                {
                    returnedName = "ERROR";
                    cbF(returnedName);
                }
            });
    }

    public void LoadPlayerPic(bool needToSave = false)
    {
        string getUserPicString = "me?fields=picture.height(100)";
        FB.API(getUserPicString, HttpMethod.GET,
            delegate (IGraphResult result)
            {
                if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
                {
                    IDictionary picData = result.ResultDictionary["picture"] as IDictionary;
                    IDictionary data = picData["data"] as IDictionary;
                    string picURL = data["url"] as string;
                    StartCoroutine(GetProfilePicRoutine(picURL, needToSave));

                }
            });
    }


    private IEnumerator GetProfilePicRoutine(string url, bool needToSave = false)
    {
        WWW www = new WWW(url);
        yield return www;
        LoadOrSavePicture(www.texture, needToSave);
    }
    
    void LoadOrSavePicture(Texture2D tex, bool needToSave)
    {
        if(lastLoginRequestSource == loginRequestSource.header)
        {
            m_headerManager.instance.playerImage.texture = tex;
        }
      //  playerDp.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }

    public void loadFriendsInstalledList(userFriendListPopulateCallback cbF)
    {
            string getFriendsInfoString = "me/friends?fields=id,name,installed";
            List<object> returnedUsers = new List<object>();
            FB.API(getFriendsInfoString, HttpMethod.GET, delegate (IGraphResult result)
            {
                if (string.IsNullOrEmpty(result.Error) && !result.Cancelled)
                {
                    returnedUsers = result.ResultDictionary["data"] as List<object>;
                    cbF(returnedUsers);
                }
                else
                {
                    Debug.Log("NO USERS RETURNED");
                }
            });
     }
#endregion
}
