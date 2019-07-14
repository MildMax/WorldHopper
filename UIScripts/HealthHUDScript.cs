using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHUDScript : MonoBehaviour
{
    //set health to pull from player(0 - 6)
    public int health;

    public Sprite heartEmpty;
    public Sprite heartHalf;
    public Sprite heartFull;

    Image[] hearts;
    Vector2 startPos;

    int oldHealth = 100;

    private void Awake()
    {
        hearts = GetComponentsInChildren<Image>();
        //Debug.Log(hearts.Length);
        SetHearts();
    }

    private void Update()
    {
        SetHearts();
    }

    private void SetHearts()
    {
        if(oldHealth != health)
        {
            SetEmptyHearts(health);
            SetFullHearts(health);

            switch(health)
            {
                case 1:
                    hearts[0].sprite = heartHalf;
                    break;
                case 3:
                    hearts[1].sprite = heartHalf;
                    break;
                case 5:
                    hearts[2].sprite = heartHalf;
                    break;

            }

            oldHealth = health;
        }
    }

    private void SetEmptyHearts(int h)
    {
        if (h < 0) h = 0;

        if (h % 2 != 0 || h == 1) --h;

        h /= 2;

        for(int i = h; i < hearts.Length; ++i)
        {
            hearts[i].sprite = heartEmpty;
        }
    }

    private void SetFullHearts(int h)
    {
        if (h > 6) h = 6;

        if (h % 2 != 0 || h == 1) ++h;

        h /= 2;

        for(int i = 0; i < h; ++i)
        {
            hearts[i].sprite = heartFull;
        }
    }
}
