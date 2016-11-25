using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class obj_IAPCatButtonManager : MonoBehaviour
{
    public string categoryName;
    public string categoryId;
    public Color categoryColor;
    public RawImage categoryImage;
    public Text catButtonText;
    public u_acJsonUtility.categoryUnlockInfo thisCat;
    bool isLocked = false;

    public void setUpButton(u_acJsonUtility.categoryUnlockInfo tC)
    {
        thisCat = tC;
        gameObject.GetComponent<Button>().colors = setColorBlock();
        gameObject.GetComponent<u_miscButtonBehaviors>().catName = tC.categoryName;
        if (thisCat.categoryImageURL != null && thisCat.categoryImageURL != "")
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
        string dispCatName = categoryName;
        
        catButtonText.text = thisCat.categoryDisplayName;
    }

    private ColorBlock setColorBlock()
    {
        ColorBlock tCB = new ColorBlock();

        if (ColorUtility.TryParseHtmlString(thisCat.categoryColorHex, out categoryColor))
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
        string dispCatName = categoryName + ":: LOCKED";
        dispCatName = char.ToUpper(dispCatName[0]) + dispCatName.Substring(1);
        catButtonText.text = dispCatName;
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
