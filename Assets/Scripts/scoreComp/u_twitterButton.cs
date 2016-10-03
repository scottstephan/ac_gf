using UnityEngine;
using System.Collections;

public class u_twitterButton : MonoBehaviour {

    private const string TWITTER_ADDRESS = "http://twitter.com/intent/tweet";
    private const string TWEET_LANGUAGE = "en";

    public void onButtonClick()
    {
        string score = "500";
        string bitlyLink = "http://shutup.com";
        string qName = appManager.currentQuestion.questionDisplayText; //if qName > 100....
        string tweetMessage = "I just scored " + score + " answering '" + qName + "...' in #googlefeud . Play now!: " + bitlyLink;
        Debug.Log("Tweet is chars long: " + tweetMessage.ToCharArray().Length);
        ShareToTwitter(tweetMessage);
    }

    void ShareToTwitter(string textToDisplay)
    {
        Application.OpenURL(TWITTER_ADDRESS +
                    "?text=" + WWW.EscapeURL(textToDisplay) +
                    "&amp;lang=" + WWW.EscapeURL(TWEET_LANGUAGE));
    }
}
