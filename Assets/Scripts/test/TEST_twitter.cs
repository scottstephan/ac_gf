using UnityEngine;
using System.Collections;

public class TEST_twitter : MonoBehaviour {

    private const string TWITTER_ADDRESS = "http://twitter.com/intent/tweet";
    private const string TWEET_LANGUAGE = "en";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShareToTwitter("fartMan");
        }
    }

    void ShareToTwitter(string textToDisplay)
    {
        Application.OpenURL(TWITTER_ADDRESS +
                    "?text=" + WWW.EscapeURL(textToDisplay) +
                    "&amp;lang=" + WWW.EscapeURL(TWEET_LANGUAGE));
    }
}


