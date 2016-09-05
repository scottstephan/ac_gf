using UnityEngine;
using System.Collections;

public class m_colorPaletteManager : MonoBehaviour { 
    public static m_colorPaletteManager instance = null;

    [System.Serializable]
    public class colorPalette {
        public Color[] palette = new Color[5];

        public Color returnRandomColor()
        {
            return palette[Random.Range(0, palette.Length)];
        }
    }

    public colorPalette buttonColorPalette = new colorPalette();

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }
    
}
