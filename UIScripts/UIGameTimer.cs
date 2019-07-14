using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameTimer : MonoBehaviour
{
    Text text;

    float counter = 0;
    float initTime;

    private void Awake()
    {
        text = GetComponent<Text>();
        UpdateTimer();
    }

    private void Update()
    {
        UpdateTimer();
    }

    private void UpdateTimer()
    {
        counter += Time.deltaTime;

        if(counter < 10)
        {
            text.text = "00:0" + (int)counter;
        }
        else if(counter < 60)
        {
            text.text = "00:" + (int)counter;
        }
        else if(counter >= 60 && (int)(counter / 60) < 10)
        {
            if ((int)(counter % 60) < 10)
            {
                text.text = "0" + (int)(counter / 60) + ":0" + (int)(counter % 60);
            }
            else if((int)(counter % 60) >= 10)
            {
                text.text = "0" + (int)(counter / 60) + ":" + (int)(counter % 60);
            }
        }
        else if(counter >= 60 && (int)(counter / 60) >= 10)
        {
            if ((int)(counter % 60) < 10)
            {
                text.text = (int)(counter / 60) + ":0" + (int)(counter % 60);
            }
            else if ((int)(counter % 60) >= 10)
            {
                text.text =  (int)(counter / 60) + ":" + (int)(counter % 60);
            }
        }
    }
}
