using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class obj_loadingWheelManager : MonoBehaviour {
    public float fillSpeed = 2f;
    public Image loadWheelImage;
    bool isClockwise = false;
	// Use this for initialization
	void Start () {
	
	}
	
	public void startWheelAnimation()
    {
        gameObject.SetActive(true);
        loadWheelImage.fillAmount = 0;
        loadWheelImage.fillClockwise = false;
        isClockwise = false;
        StartCoroutine("fillLoadWheel");
    }

    public void stopWheelAnimation()
    {
        gameObject.SetActive(false);
    }

    public void setWheelPosition(Vector3 pos)
    {
        gameObject.transform.position = pos;
    }

    public void setWheelTransform(Transform t)
    {
        gameObject.transform.SetParent(t, false);
    }


    IEnumerator fillLoadWheel()
    { // go 0 - 1, flip clockwise, go 1 - 0
        float rate = 1 / fillSpeed;
        float fillStartPos = isClockwise ? 0 : 1;
        float fillDestPos = isClockwise ? 1 : 0;
        float i = 0;

        while(i < 1)
        {
            i += Time.deltaTime * rate;
            float fillVal = Mathf.Lerp(fillStartPos, fillDestPos,i);
            loadWheelImage.fillAmount = fillVal;

            yield return null;
        }

        isClockwise = !isClockwise;
        loadWheelImage.fillClockwise = isClockwise;
        StartCoroutine("fillLoadWheel");
    }
}
