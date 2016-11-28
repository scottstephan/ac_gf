using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class categorySelectionButtonManager : MonoBehaviour
{
    public string categoryName;
    public string categoryId;
    public Color categoryColor;
    public RawImage categoryImage;
    public Text catButtonText;
    public u_acJsonUtility.acCat thisCat;
    bool isLocked = false;

    public void categorySelected()
    {
        if (appManager.curLiveGame.isMPGame)
            m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundMP);
        else
            m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundSP);
    }

    public void buttonClicked()
    {
        if (!isLocked) { 
            m_gameManager.instance.setCurrentSelectCategory(categoryName);
            //m_phaseManager.instance.changePhase(m_phaseManager.phases.mainRoundSP);
            m_phaseManager.instance.changePhase(m_phaseManager.phases.tutorial);
        }
        else
        {//IAP path
            unlockCategory(); //FOR TESTING ONLY
        }
    }

    public void setUpButton(string cN)
    {
        thisCat = u_acJsonUtility.instance.loadCategoryData(cN);
        gameObject.GetComponent<Button>().colors = setColorBlock();
        if(thisCat.categoryImageValue != null && thisCat.categoryImageValue != "")
        {
            string sCatName = u_acJsonUtility.instance.autoCompeteSanatizeString(thisCat.categoryName);
            Debug.Log("Loading cat image for: " + sCatName);

            //Try and load local. If null...
            Texture2D catTex = loadLocalCategoryImage(sCatName);
            if (catTex == null)
                StartCoroutine("loadCategoryImageFromWeb", sCatName);
            else
                setCategoryImage(catTex);
        }
        catButtonText.text = thisCat.categoryDisplayName;
    }

    private ColorBlock setColorBlock()
    {
        ColorBlock tCB = new ColorBlock();
        
        if (ColorUtility.TryParseHtmlString(thisCat.categorColorValue, out categoryColor))
        {
            tCB.normalColor = categoryColor;
        }
        else
        {
            tCB.normalColor = Color.black;
        }

        tCB.highlightedColor = tCB.normalColor;
        tCB.pressedColor = tCB.normalColor;
        tCB.colorMultiplier = 1;
        return tCB;
    }

    public void setCategoryImage(Texture2D catImage)
    {
        categoryImage.texture = catImage;
    }

    public void lockButton()
    {
        isLocked = true;
        ColorBlock bColor = new ColorBlock();
        bColor.colorMultiplier = 1;
        bColor.normalColor = Color.red;
        bColor.highlightedColor = Color.blue;

        gameObject.GetComponent<Button>().colors = bColor;
        string dispCatName = categoryName+ ":: LOCKED";
        dispCatName = char.ToUpper(dispCatName[0]) + dispCatName.Substring(1);
        catButtonText.text = dispCatName;
    }

    public void unlockCategory()
    {
        u_acJsonUtility.instance.findAndUnlockCategory(categoryName);
        setUpButton(categoryName);
    }

    //For loading web images. Removed from u_acJSONUntil because of the complex callback issue. Hell yeah, shipping code.

    IEnumerator loadCategoryImageFromWeb(string catName)
    {
        string localImgPath = u_acJsonUtility.baseSavePathString + u_acJsonUtility.categoryImageSavePath + catName + ".png";

            string url = "https://s3.amazonaws.com/autocompete/categoryimages/" + catName + ".png";

            Texture2D catImage = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            while (true)
            {
                WWW www = new WWW(url);
                yield return www;
                if (www.error == null)
                {
                    www.LoadImageIntoTexture(catImage);
                    //Save local
                    byte[] bytes = catImage.EncodeToPNG();
                    File.WriteAllBytes(localImgPath, bytes);
                    setCategoryImage(catImage);
                }
                else
                {
                    Debug.Log(catName + "CAT IMAGE LOAD ERROR:" + www.error);
                    StopCoroutine("loadCategoryImageFromWeb");
                }
            }
    }

    public Texture2D loadLocalCategoryImage(string catName)
    {
        string localImgPath = u_acJsonUtility.baseSavePathString + u_acJsonUtility.categoryImageSavePath + catName + ".png";
        //1- Check to see if the image exists locally
        if (File.Exists(localImgPath))
        {
            Debug.Log("Image exists locally!");
            Texture2D catImage = new Texture2D(4, 4, TextureFormat.RGBA32, false);
            byte[] imageData = File.ReadAllBytes(localImgPath);
            catImage.LoadImage(imageData);
            return catImage;
        }

        return null;
    }
}
