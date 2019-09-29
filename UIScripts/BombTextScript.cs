using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BombTextScript : MonoBehaviour
{

    BombCounterScript counterScript;

    Text bombText;

    private void Awake()
    {
        counterScript = GameObject.FindGameObjectWithTag("BombCanvas").GetComponentInChildren<BombCounterScript>();
        bombText = GetComponent<Text>();
        bombText.text = "x" + counterScript.bombCount;
    }

    private void Update()
    {
        bombText.text = "x" + counterScript.bombCount;
    }
}
