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
        createCategoryLayout();
    }

    void createCategoryLayout()
    {
        List<string> catNames = u_acJsonUtility.instance.discoverCategories();
        Debug.Log("---CAT NAMES---:" + catNames);

        for(int i = 0; i < catNames.Count; ++i)
        {
            //Spawn a button!
            GameObject tButton = Instantiate(categoryButton);
            tButton.transform.SetParent(parentCategoryListGrid.transform);

            categorySelectionButtonManager tManager = tButton.GetComponent<categorySelectionButtonManager>();
            tManager.categoryName = catNames[i];
            tManager.categoryId = "NOTUSINGFORNOW";
            tManager.setUpButton();
        }
    }

    public void loadCategoryQuestion(string catName)
    {
        u_acJsonUtility.acQ tQ = new u_acJsonUtility.acQ();
        tQ = u_acJsonUtility.instance.loadRandomQuestionData(catName);
        appManager.currentQuestion = tQ; //COuld skip the above hullabaloo.
    }
}
