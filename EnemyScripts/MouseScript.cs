﻿using System.Collections;
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
    Animator anim;
    CapsuleCollider2D coll;
    WorldSwitcher wS;

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

    public AnimationClip deathClip;
    float deathWait;
    float deathTimer = 0;
    bool isDead = false;
    bool deathSet = false;

    float oldHealth;

    public int worldNum = 4;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        detector = GetComponentInParent<MouseTriggerDetector>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider2D>();
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        oldHealth = health;
        deathWait = deathClip.length * 3;
        SetCondition();
        GetWorldNum();
        SetInitialPos();
        HideRenderer();
    }

    private void Update()
    {
        GetWorldNum();
        CheckHealth();
        if (isDead == false)
        {
            DetectLocation();
            FlipSprites();
            ChangeAnimState();
            HideRenderer();

            if (isMoving == false)
            {
                isMoving = condition();
            }
        }
    }

    private void FixedUpdate()
    {
        if(isMoving == true && isDead == false)
        {
            MoveMouse();
            CheckArrival();
        }
    }

    //::::::::::::::::GENERIC:::::::::::::://

    private void CheckHealth()
    {
        if(health <= 0)
        {
            if (deathSet == false)
            {
                isDead = true;
                coll.enabled = false;
                anim.SetBool("IsDead", true);
            }
            
            if(deathTimer >= deathWait)
            {
                wS.DestroyEnemyValue(hash);
                wS.DestroyEnemyValue(detector.hash);
                detector.destroyThis = true;
                Destroy(gameObject);
            }

            deathTimer += Time.deltaTime;
        }
        else if(health < oldHealth)
        {
            anim.SetTrigger("IsHurt");
            oldHealth = health;
        }
    }

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

    private void HideRenderer()
    {
        if (wS.activeWorldNum == worldNum)
        {
            if ((Vector2)transform.position == pos1 || (Vector2)transform.position == pos2)
            {
                rend.enabled = false;
                coll.enabled = false;
            }
            else
            {
                rend.enabled = true;
                coll.enabled = true;
            }
        }
    }

    private void ChangeAnimState()
    {
        if (isDead == false)
        {
            if (isMoving == true)
            {
                anim.SetBool("IsWalking", true);
            }
            else
            {
                anim.SetBool("IsWalking", false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Shot")
        {
            health -= collision.gameObject.GetComponent<ShotScript>().damage;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().GetHurt(coll.transform.position);
        }
    }

    private void GetWorldNum()
    {
        if (worldNum == 4)
        {
            if (hash.Contains("W1")) worldNum = 0;
            else if (hash.Contains("W2")) worldNum = 1;
            else if (hash.Contains("W3")) worldNum = 2;
            else if (hash.Contains("W4")) worldNum = 3;
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

    private void SetInitialPos()
    {
        if(isLeft == true)
        {
            transform.position = pos1;
        }
        else if(isLeft == false)
        {
            transform.position = pos2;
        }
    }

}
