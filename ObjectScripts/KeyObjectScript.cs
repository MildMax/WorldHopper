using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyObjectScript : MonoBehaviour
{
    Vector2 startPos;
    KeyScript keyScript;

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
    }

    private void Update()
    {
        AnimateKey();
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
}
