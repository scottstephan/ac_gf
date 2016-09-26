using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class m_categorySelectionManager : MonoBehaviour {
    public static m_categorySelectionManager instance = null;
    public GameObject categoryButton;
    public GameObject parentCategoryListGrid;

    void Awake()
    { //Maintain singleton pattern
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void initCategoryPhase()
    {
        clearCategoryLayout();
        createCategoryLayout();
    }

    void clearCategoryLayout()
    {
        foreach (Transform t in parentCategoryListGrid.transform)
            Destroy(t.gameObject);
    }

    void createCategoryLayout()
    {
        List<string> catNames = new List<string>();
        catNames.Clear();
        catNames = u_acJsonUtility.instance.discoverCategories();

        for(int i = 0; i < catNames.Count; ++i)
        {
            GameObject tButton = Instantiate(categoryButton);
            tButton.transform.SetParent(parentCategoryListGrid.transform);

            categorySelectionButtonManager tManager = tButton.GetComponent<categorySelectionButtonManager>();
            tManager.categoryName = catNames[i];
            tManager.categoryId = "NOTUSINGFORNOW";
            tManager.setUpButton();
        }
    }
    /// <summary>
    /// Loads and sets the 'current question' attribute for the game
    /// </summary>
    /// <param name="catName"></param>
    public void loadCategoryQuestion(string catName)
    {
        u_acJsonUtility.acQ tQ = new u_acJsonUtility.acQ();
        tQ = u_acJsonUtility.instance.loadRandomQuestionData(catName);
        appManager.currentQuestion = tQ; //COuld skip the above hullabaloo.
    }
}
