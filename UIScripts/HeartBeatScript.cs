using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartBeatScript : MonoBehaviour
{
    public Sprite heartEmpty;
    public Sprite heartHalf;
    public Sprite heartFull;

    public float beatModifier;
    public float frequencyModifier;

    Image heart;
    RectTransform rect;
    public Vector2 heartSize;
    public Vector2 startPosition;

    private void Awake()
    {
        heart = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        heartSize = heart.rectTransform.sizeDelta;
        startPosition = rect.localPosition;
    }

    private void Update()
    {
        HeartBeat();
    }

    private void LateUpdate()
    {
        SetHeartPosition();
    }

    private void HeartBeat()
    {
        if(heart.sprite == heartEmpty)
        {
            heart.rectTransform.sizeDelta = heartSize;
        }
        else if(heart.sprite == heartFull || heart.sprite == heartHalf)
        {
            float beat = Mathf.Sin(Time.time * frequencyModifier);
            heart.rectTransform.sizeDelta = new Vector2(heartSize.x + beatModifier * beat, heartSize.y + beatModifier * beat);
        }
    }

    private void SetHeartPosition()
    {
        float x = startPosition.x - ((heart.rectTransform.sizeDelta.x - heartSize.x) / 2);
        float y = startPosition.y - ((heart.rectTransform.sizeDelta.x - heartSize.y) / 2);

        Debug.Log("X: " + (heart.rectTransform.sizeDelta.x - heartSize.x));
        Debug.Log("Y: " + (heart.rectTransform.sizeDelta.x - heartSize.y));

        rect.localPosition = new Vector2(x, y);
    }


}
