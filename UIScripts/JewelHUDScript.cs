using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JewelHUDScript : MonoBehaviour
{
    Image im;
    public Sprite[] jewels;
    int jewelLim;
    int jewelNum;

    public float switchWait;
    float timer;

    private void Awake()
    {
        jewelLim = jewels.Length;
        im = GetComponent<Image>();
    }

    private void Update()
    {
        SwitchColor();
    }

    private void SwitchColor()
    {
        if (timer >= switchWait)
        {
            ++jewelNum;
            if (jewelNum == jewelLim) jewelNum = 0;
            im.sprite = jewels[jewelNum];
            timer = 0;
        }
        timer += Time.deltaTime;
    }
}
