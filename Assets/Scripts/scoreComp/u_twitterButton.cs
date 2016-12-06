using UnityEngine;
using System.Collections;

public class u_twitterButton : MonoBehaviour {

    private const string TWITTER_ADDRESS = "http://twitter.com/intent/tweet";
    private const string TWEET_LANGUAGE = "en";

    public string score = "500";
    public string bitlyLink = "http://autocompete.net";
    public string catName = "PLACEHOLDER"; //if qName > 100....

    public void onButtonClick()
    {
        string tweetMessage = "I just got " + score + " points playing '" + catName + "' in #autocompete . Challenge me to a game!: " + bitlyLink;
        Debug.Log("Tweet is chars long: " + tweetMessage.ToCharArray().Length);
        ShareToTwitter(tweetMessage);
    }

    public void setInfo(int s, string cN)
    {
        score = s.ToString("N0");
        catName = cN;
    }

    void ShareToTwitter(string textToDisplay)
    {
        Application.OpenURL(TWITTER_ADDRESS +
                    "?text=" + WWW.EscapeURL(textToDisplay) +
                    "&amp;lang=" + WWW.EscapeURL(TWEET_LANGUAGE));
    }
}
