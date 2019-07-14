using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombTextScript : MonoBehaviour
{
    //track this in bombscript
    public int bombCount;

    Text bombText;

    private void Awake()
    {
        bombText = GetComponent<Text>();
        bombText.text = "x" + bombCount;
    }

    private void Update()
    {
        bombText.text = "x" + bombCount;
    }
}
