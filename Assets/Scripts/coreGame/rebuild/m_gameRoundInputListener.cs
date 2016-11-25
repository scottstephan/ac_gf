using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class m_gameRoundInputListener : MonoBehaviour {
    public static m_gameRoundInputListener instance;
    public InputField playerInput;

    static TouchScreenKeyboard tSK;
    static bool tSKIsOpen = false;
    static bool acceptInput = false;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_ANDROID
        if (playerInput.isFocused && !tSKIsOpen)
        {
            Debug.Log("PLAYER INPUT FIELD FOCUSED");
            tSK = TouchScreenKeyboard.Open("jim", TouchScreenKeyboardType.Default, true, false, false, false, "");
            tSKIsOpen = true;
            acceptInput = true;
        }
        else if(!playerInput.isFocused && tSKIsOpen)
        {
            Debug.Log("PLAYER INPUT FIELD UNFOCUSED");
            tSKIsOpen = false;
            acceptInput = false;
        }
        
        if (tSK.done && acceptInput)
        {
            Debug.Log("PLAYER IS SUMITTING INPUT");
            m_gameManager.instance.playerInputComplete();
        }
#endif
    }

    public static void showKeyboard()
    {

    }

    public static void hideKeyboard()
    {

    }

    public static void listenForMobileInput()
    {

    }
}
