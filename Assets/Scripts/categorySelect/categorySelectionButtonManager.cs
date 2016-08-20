using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class categorySelectionButtonManager : MonoBehaviour
{
    public string categoryName;
    public int categoryId;

    public void categoruSelected()
    {
        appManager.loadScene(appManager.sceneNames.singlePlayer); //refactor so button isn't responsible
    }
}
