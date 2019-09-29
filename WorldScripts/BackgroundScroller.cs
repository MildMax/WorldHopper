using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    public float adjustSpeed;
    public int bgOffset;

    SpriteRenderer[] spriteRenderer;
    SpriteRenderer current;
    SpriteRenderer alternate;
    Transform playerTransform;

    Vector2 bgSize;

    float centerAdjust;
    float newCenter;

    private void Awake()
    {
        spriteRenderer = GetComponentsInChildren<SpriteRenderer>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        bgSize = spriteRenderer[0].sprite.bounds.size;
        current = spriteRenderer[0];
        alternate = spriteRenderer[1];
        current.transform.position = new Vector2(playerTransform.position.x, playerTransform.position.y + bgOffset);
    }

    private void FixedUpdate()
    {
        AdjustCenter();
        Modulate();
        MoveSpriteRenderer();
        MoveAlt();
        MoveY();
    }

    private void MoveSpriteRenderer()
    {
        if (playerTransform != null)
        {
            if (playerTransform.position.x > current.transform.position.x)
            {
                current.transform.position = new Vector2(centerAdjust, current.transform.position.y);
                alternate.transform.position = new Vector2(current.transform.position.x - bgSize.x, alternate.transform.position.y);
            }
            else if (playerTransform.position.x <= current.transform.position.x)
            {
                current.transform.position = new Vector2(centerAdjust, current.transform.position.y);
                alternate.transform.position = new Vector2(current.transform.position.x + bgSize.x, alternate.transform.position.y);
            }
        }
    }

    private void MoveAlt()
    {
        if (playerTransform != null)
        {
            if (playerTransform.position.x - current.transform.position.x > 0)
            {
                alternate.transform.position = new Vector2(current.transform.position.x + bgSize.x, current.transform.position.y);
            }
            else
            {
                alternate.transform.position = new Vector2(current.transform.position.x - bgSize.x, current.transform.position.y);
            }
        }
    }

    private void Modulate()
    {
        if (playerTransform != null)
        {
            if (playerTransform.position.x > (centerAdjust + bgSize.x) || playerTransform.position.x < (centerAdjust - bgSize.x))
            {
                SwitchCurrent();
            }
        }
    }

    private void SwitchCurrent()
    {
        //Debug.Log("switching"); 
        if (current == spriteRenderer[0])
        {
            current = spriteRenderer[1]; 
            alternate = spriteRenderer[0];
        }
        else
        {
            current = spriteRenderer[0];
            alternate = spriteRenderer[1];
        }

        Debug.Log(current.transform.position.x);
        centerAdjust = current.transform.position.x;

        newCenter = centerAdjust;
    }

    private void AdjustCenter()
    {
        if (playerTransform != null)
        {
            centerAdjust = (playerTransform.position.x * adjustSpeed) + (newCenter * (1 - adjustSpeed));
        }
    }

    private void MoveY()
    {
        if (playerTransform != null)
        {
            foreach (SpriteRenderer i in spriteRenderer)
            {
                i.transform.position = new Vector2(i.transform.position.x, playerTransform.position.y);
            }
        }
    }

    private float LerpY(float playerY, float bgY, float speed)
    {
        if (playerY != bgY + bgOffset)
        {
            return (bgY + (playerY - bgY) + bgOffset ) * speed;
        }
        else return bgY + bgOffset;
    }

    private void OnEnable()
    {
        if (playerTransform != null)
        {
            current.transform.position = playerTransform.position;
        }
    }
}
