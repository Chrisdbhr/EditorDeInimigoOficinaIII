using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetColorTheme : MonoBehaviour
{
    public Color CommonColor;
    public Color HighlightedColor;

    public Image[] ImageColor;

    void Start()
    {
        Image[] objsWithImage = FindObjectsOfType<Image>();
        Text[] textsWithTexts = FindObjectsOfType<Text>();

        foreach (Image imagem in objsWithImage)
        {
            imagem.color = new Color(CommonColor.r, CommonColor.g, CommonColor.b);
        }
        foreach (Text texto in textsWithTexts)
        {
            texto.color = new Color(CommonColor.r, CommonColor.g, CommonColor.b);
        }
    }


}
