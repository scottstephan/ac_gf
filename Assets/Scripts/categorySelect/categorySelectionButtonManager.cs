using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class categorySelectionButtonManager : MonoBehaviour
{
    public string categoryName;
    public int categoryId;

    public void categoruSelected()
    {
        SceneManager.LoadScene(2); //refactor so button isn't responsible
    }
}
