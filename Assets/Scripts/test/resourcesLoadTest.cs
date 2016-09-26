using UnityEngine;
using System.Collections;

public class resourcesLoadTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R))
        {
            checkAllJSONCats();
        }
	}

    void checkAllJSONCats()
    {
        TextAsset[] resourceJsonCats; 
        resourceJsonCats = Resources.LoadAll<TextAsset>("jsonCategories");
        for(int i = 0; i < resourceJsonCats.Length; ++i)
        {
            Debug.Log(resourceJsonCats[i].ToString());
        }
    }
}
