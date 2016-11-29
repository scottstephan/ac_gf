using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class obj_highScoreList : MonoBehaviour {
    public Color categoryGreen;
    public Color scoreBlue;
    public Text categoryName;
 //   public Text highScoreValue;

	public void setupReadout(string catname, string highscore, string catColor)
    {
        int sVal = int.Parse(highscore);
        highscore = sVal.ToString("N0");

        string formattedCategoryName = "<color=" + catColor + "><size=80><b>"+ catname +"</b></size></color>";
        string formattedScore = "<color=#4470E6FF><size=135><b>"+highscore+"</b></size></color>";
        categoryName.text = formattedCategoryName + "\n" + formattedScore;
       // highScoreValue.text = highscore;
    }
}
