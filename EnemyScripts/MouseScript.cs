using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseScript : EnemyBase
{

    [HideInInspector]
    public int actionIndex;

    public delegate bool MoveCondition();
    MoveCondition condition;

    SpriteRenderer rend;
    MouseTriggerDetector detector;

    public float speed;

    public Vector2 pos1;
    public Vector2 pos2;

    public bool isLeft = false;
    public bool isMoving = false;

    public float minWait;
    public float maxWait;
    float timeWait;
    float timer = 0;
    bool timerSet = false;

    bool flipSprites = false;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        detector = GetComponentInParent<MouseTriggerDetector>();
        SetCondition();
    }

    private void Update()
    {
        DetectLocation();
        FlipSprites();

        if (isMoving == false)
        {
            isMoving = condition();
        }
    }

    private void FixedUpdate()
    {
        if(isMoving == true)
        {
            MoveMouse();
            CheckArrival();
        }
    }

    //::::::::::::::::GENERIC:::::::::::::://

    private void SetCondition()
    {
        switch(actionIndex)
        {
            case 0:
                condition = DetectContact;
                return;
            case 1:
                condition = MoveTimer;
                return;
        }
    }

    private void MoveMouse()
    {
        if(isLeft == true)
        {
            transform.position = Vector2.MoveTowards(transform.position, pos2, speed * Time.deltaTime);
        }
        else if(isLeft == false)
        {
            transform.position = Vector2.MoveTowards(transform.position, pos1, speed * Time.deltaTime);
        }
    }

    private void CheckArrival()
    {
        if(isLeft == true && (Vector2)transform.position == pos2)
        {
            isMoving = false;
        }
        else if(isLeft == false && (Vector2)transform.position == pos1)
        {
            isMoving = false;
        }
    }

    private void DetectLocation()
    {
        if((Vector2)transform.position == pos1)
        {
            isLeft = true;
        }
        else if((Vector2)transform.position == pos2)
        {
            isLeft = false;
        }
    }

    private void FlipSprites()
    {
        if (isMoving == true)
        {
            if (isLeft == false)
            {
                if (rend.flipX != false)
                {
                    rend.flipX = false;
                }


            }
            else if (isLeft == true)
            {
                if (rend.flipX != true)
                {
                    rend.flipX = true;
                }
            }
        }
    }

    //::::::::::::::::CONTACT::::::::::::://

    private bool DetectContact()
    {
        if (detector.inVicinity == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //::::::::::::::TIMER:::::::::::::::://

    private bool MoveTimer()
    {
        SetTimer();

        if (timer >= timeWait)
        {
            timerSet = false;
            timer = 0;
            return true;
        }
        else
        {
            timer += Time.deltaTime;
            return false;
        }

        
    }

    private void SetTimer()
    {
        if(timerSet == false)
        {
            timeWait = Random.Range(minWait, maxWait);
            timerSet = true;
        }
    }

}
