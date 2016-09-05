using UnityEngine;
using System.Collections;

public class animCallbackTest : MonoBehaviour {

    public void onIntro()
    {
        Debug.Log("Anm call back intro");
    }

    public void OnExit()
    {
        Debug.Log("Anim call back exit");
    }
}
