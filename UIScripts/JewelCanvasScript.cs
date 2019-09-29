using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JewelCanvasScript : MonoBehaviour
{
    public float adjust;

    Text text;
    Image image;

    //[HideInInspector]
    public int jewelCount = 0;

    bool textAdjusted = false;

    private void Awake()
    {
        text = GetComponentInChildren<Text>();
        image = GetComponentInChildren<Image>();
    }

    private void Update()
    {
        
        UpdateText();
        ShiftText();
    }

    private void UpdateText()
    {
        text.text = "x" + jewelCount;
    }

    private void ShiftText()
    {
        if (jewelCount >= 10 && textAdjusted == false)
        {
            text.rectTransform.position = new Vector3(text.rectTransform.position.x - (adjust / 2), text.rectTransform.position.y, text.rectTransform.position.z);
            //text.rectTransform.sizeDelta = new Vector2(text.rectTransform.sizeDelta.x + adjust, text.rectTransform.sizeDelta.y);
            image.rectTransform.position = new Vector3(image.rectTransform.position.x - (adjust / 2), image.rectTransform.position.y, image.rectTransform.position.z);
            textAdjusted = true;
        }
    }
}
