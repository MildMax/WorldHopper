using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyObjectScript : MonoBehaviour
{
    Vector2 startPos;
    KeyScript keyScript;

    public bool isWorldSpecific;
    public int worldNum;
    WorldSwitcher ws;
    bool activeSet = false;
    bool deactiveSet = false;
    SpriteRenderer rend;
    Collider2D coll;

    public bool isBlue;
    public bool isRed;
    public bool isGreen;
    public bool isYellow;

    public float frequency;
    public float magnitude;
    float yPos;

    private void Awake()
    {
        keyScript = GameObject.FindGameObjectWithTag("KeyCanvas").GetComponentInChildren<KeyScript>();
        startPos = (Vector2)transform.position;

        if (isWorldSpecific)
        {
            ws = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
            rend = GetComponent<SpriteRenderer>();
            coll = GetComponent<Collider2D>();
        }
    }

    private void Update()
    {
        AnimateKey();
        if (isWorldSpecific) ManageWorldSpecific();
    }

    private void AnimateKey()
    {
        yPos = startPos.y + Mathf.Sin(Time.time * frequency) * magnitude;
        transform.position = new Vector2(startPos.x, yPos);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            keyScript.hasNewKey = true;

            if(isBlue)
            {
                keyScript.hasBlueKey = true;
            }
            else if(isRed)
            {
                keyScript.hasRedKey = true;
            }
            else if(isGreen)
            {
                keyScript.hasGreenKey = true;
            }
            else if(isYellow)
            {
                keyScript.hasYellowKey = true;
            }

            Destroy(gameObject);
        }
    }

    private void ManageWorldSpecific()
    {
        if(ws.activeWorldNum == worldNum && !activeSet)
        {
            rend.enabled = true;
            coll.enabled = true;
            deactiveSet = false;
            activeSet = true;
        }
        else if(ws.activeWorldNum != worldNum && !deactiveSet)
        {
            rend.enabled = false;
            coll.enabled = false;
            activeSet = false;
            deactiveSet = true;
        }
    }
}
