using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyScript : MonoBehaviour
{
    public bool redKey;
    public bool blueKey;
    public bool greenKey;
    public bool yellowKey;

    //set hasNewKey in script when player walks over key
    //add respective key color as well
    public bool hasNewKey;
    public bool hasRedKey;
    public bool hasBlueKey;
    public bool hasGreenKey;
    public bool hasYellowKey;

    public Sprite redKeySolid;
    public Sprite blueKeySolid;
    public Sprite greenKeySolid;
    public Sprite yellowKeySolid;

    public Sprite redKeyEmpty;
    public Sprite blueKeyEmpty;
    public Sprite greenKeyEmpty;
    public Sprite yellowKeyEmpty;

    public Sprite keyEmpty;

    Image[] keyImages;
    string[] keys;

    private void Awake()
    {
        keyImages = GetComponentsInChildren<Image>();
        keys = FindKeysInUse();
        //Debug.Log(keys.Length);
        SetKeyImages();
    }

    private void Update()
    {
        SetNewKeys();
    }

    private string[] FindKeysInUse()
    {
        List<string> keyList = new List<string>();

        if (redKey) keyList.Add("red");
        if (blueKey) keyList.Add("blue");
        if (greenKey) keyList.Add("green");
        if (yellowKey) keyList.Add("yellow");

        for(int i = keyList.Count; i != 4; ++i)
        {
            keyList.Add(null);
        }

        string[] keyArr = new string[4];

        for(int i = 0; i != keyArr.Length; ++i)
        {
            keyArr[i] = keyList[i];
        }

        return keyArr;
    }

    private void SetKeyImages()
    {
        for(int i = 0; i != keyImages.Length; ++i)
        {
            if (keys[i] != null)
            {
                switch (keys[i])
                {
                    case "red":
                        keyImages[i].sprite = redKeyEmpty;
                        break;
                    case "blue":
                        keyImages[i].sprite = blueKeyEmpty;
                        break;
                    case "green":
                        keyImages[i].sprite = greenKeyEmpty;
                        break;
                    case "yellow":
                        keyImages[i].sprite = yellowKeyEmpty;
                        break;
                    default:
                        keyImages[i].sprite = keyEmpty;
                        break;
    
                }
            }
            else
            {
                keyImages[i].sprite = keyEmpty;
            }
        }
    }

    private void SetNewKeys()
    {
         if(hasNewKey == true)
        {
            for (int i = 0; i != keys.Length; ++i)
            {
                if (hasRedKey && keys[i] == "red")
                {
                    keyImages[i].sprite = redKeySolid;
                }
                else if(hasBlueKey && keys[i] == "blue")
                {
                    keyImages[i].sprite = blueKeySolid;
                }
                else if(hasGreenKey && keys[i] == "green")
                {
                    keyImages[i].sprite = greenKeySolid;
                }
                else if(hasYellowKey && keys[i] == "yellow")
                {
                    keyImages[i].sprite = yellowKeySolid;
                }
            }

            hasNewKey = false;
        }
    }
}
