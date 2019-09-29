using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombCounterScript : MonoBehaviour
{
    Slider slider;

    public float regenRate;

    //[HideInInspector]
    public int bombCount;
    public int bombLimit;

    bool startRegen = false;

    [HideInInspector]
    public bool canThrow = true;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        bombCount = bombLimit;
    }

    private void Update()
    {
        ManageBombThrow();
        RegenBombs();
    }

    private void RegenBombs()
    {
        if(bombCount < bombLimit)
        {
            if(startRegen == false)
            {
                slider.value = 0;
                startRegen = true;
            }

            slider.value += regenRate * Time.deltaTime;

            if(slider.value == slider.maxValue)
            {
                bombCount += 1;
                startRegen = false;
            }
        }
        else
        {
            slider.value = slider.maxValue;
        }
    }

    private void ManageBombThrow()
    {
        if(bombCount <= 0)
        {
            canThrow = false;
        }
        else
        {
            canThrow = true;
        }

        if(bombCount < 0)
        {
            bombCount = 0;
        }
    }

}
