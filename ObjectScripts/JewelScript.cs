using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JewelScript : CollectableBase
{

    SpriteRenderer rend;
    public Sprite[] jewels;
    int jewelLim;
    int jewelNum;

    public float switchWait;
    float timer;

    Vector2 startPos;
    public float frequency;
    public float magnitude;
    float yPos;

    //int? worldNum;
    WorldSwitcher wS;

    JewelCanvasScript jS;

    private void Awake()
    {
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        jS = GameObject.FindGameObjectWithTag("JewelCanvas").GetComponentInChildren<JewelCanvasScript>();
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = jewels[jewelNum];
        jewelLim = jewels.Length;
        startPos = (Vector2)transform.position;
    }

    private void Update()
    {
        GetWorldNum();
        SwitchColor();
    }

    private void FixedUpdate()
    {
        AnimateJewel();
    }

    private void SwitchColor()
    {
        if(timer >= switchWait)
        {
            ++jewelNum;
            if (jewelNum == jewelLim) jewelNum = 0;
            rend.sprite = jewels[jewelNum];
            timer = 0;
        }
        timer += Time.deltaTime;
    }

    private void AnimateJewel()
    {
        yPos = startPos.y + Mathf.Sin(Time.time * frequency) * magnitude;
        transform.position = new Vector2(startPos.x, yPos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && worldNum.Value == wS.activeWorldNum)
        {
            //add value to be read and displayed on HUD here
            jS.jewelCount += 1;
            Destroy(gameObject);
        }
    }

    private void GetWorldNum()
    {
        if(!worldNum.HasValue)
        {
            string a = LayerMask.LayerToName(gameObject.layer);
            //Debug.Log(a);

            if (a.Contains("1")) worldNum = 0;
            else if (a.Contains("2")) worldNum = 1;
            else if (a.Contains("3")) worldNum = 2;
            else if (a.Contains("4")) worldNum = 3;

            //Debug.Log(worldNum.Value);
            //Debug.Log(wS.activeWorldNum);
        }
    }
}
