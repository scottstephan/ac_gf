using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class m_loadPanelManager : MonoBehaviour {
    public static m_loadPanelManager instance = null;
    public GameObject panelParent;
    public Text panelText;
    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

    }

    void Start () {
        activateLoadPanel();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void activateLoadPanel()
    {
        panelParent.SetActive(true);
        panelText.text = "LOADING; WAIT A GODDAM SECOND";
    }

    public void deactivateLoadPanel()
    {
        panelParent.SetActive(false);
    }
}
