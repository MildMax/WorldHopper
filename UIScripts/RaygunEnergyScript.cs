using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaygunEnergyScript : MonoBehaviour
{
    public float regenRate;
    public float shootWait;

    public Color fillNormal;
    public Color fillLow;
    public Color bgNormal;
    public Color bgLow;

    [HideInInspector]
    public Slider slider;
    Image[] bars;

    //use this value to prevent player from shooting
    [HideInInspector]
    public bool cantShoot = false;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        bars = GetComponentsInChildren<Image>();
        Debug.Log(bars.Length);
    }

    private void Update()
    {
        CheckZero();
        CheckBelowThreshold();
        RegenerateSlider();
    }

    private void RegenerateSlider()
    {
        if(slider.value < slider.maxValue)
        {
            slider.value += regenRate * Time.deltaTime;
        }

        if(cantShoot && slider.value >= shootWait)
        {
            cantShoot = false;
        }
    }

    private void CheckZero()
    {
        if(slider.value <= slider.minValue)
        {
            cantShoot = true;
        }
    }

    private void CheckBelowThreshold()
    {
        if(slider.value < shootWait)
        {
            bars[1].color = fillLow;
        }
        else
        {
            bars[1].color = fillNormal;
        }

        if(cantShoot && slider.value < shootWait)
        {
            bars[0].color = bgLow;
        }
        else
        {
            bars[0].color = bgNormal;
        }
    }
}
