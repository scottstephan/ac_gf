using UnityEngine;
using System.Collections;
using UnityEditor;

public class FBProEditor : MonoBehaviour {

	#if UNITY_EDITOR

	
	[MenuItem("Facebook/FBPro/Docs")]
	public static void OpenHelp()
	{
		string url = "https://goo.gl/Ovqwqb";
		Application.OpenURL(url);
	}
	
	[MenuItem("Facebook/FBPro/About")]
	public static void ShowAbout()
	{
		EditorUtility.DisplayDialog("FBPro Version",
		                            "GameSlyce Facebook Integration Pro Plugin Version is 1.0", "OK");
	}
	
	[MenuItem("Facebook/FBPro/Support")]
	public static void ShowCredits()
	{
		EditorUtility.DisplayDialog("Contact Info",
		                            "Game Slyce: info.gameslyce@gmail.com", "OK");
	}
	#endif
}
