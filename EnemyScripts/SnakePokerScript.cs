﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakePokerScript : EnemyBase
{
    public float initialDelay;
    float initDelayTimer = 0;

    //[SerializeField]
    [HideInInspector]
    public int actionIndex;

    private int oldIndex = 4;

    public delegate void ActionMethod();
    public ActionMethod actionMethod;

    //0: PokerAscending
    //1: PokerDescending
    //2: PokerUp
    //3: PokerDown
    public AnimationClip[] anims;

    //arrays of sprites for death-flash-sequence in GetDeathStates()
    public Sprite[] idleSpritesA;
    public Sprite[] idleSpritesB;
    public Sprite[] whiteSpritesA;
    public Sprite[] whiteSpritesB;
    Sprite sadSprite;
    Sprite whiteSprite;

    //values for timer-based movement
    public float timer;
    public float delay;

    public float dangerClose;

    private bool start = false;

    private bool startA = false;
    private bool startD = false;

    private bool isHurt = false;

    Coroutine routineP = null;
    Coroutine hurtRoutine = null;

    Animator anim;
    SpriteRenderer rend;
    Transform player;
    PlayerController playerController;
    WorldSwitcher wS;
    CapsuleCollider2D coll;

    public float animChangeTime;
    public AnimatorOverrideController AcontrollerIdle;
    public AnimatorOverrideController BcontrollerIdle;
    public AnimatorOverrideController controllerHurt;

    float changeTimer;
    bool isControllerA = true;
    
    int worldNum;

    float oldHealth;
    float deathTime;
    float dTimer = 0;
    bool deathSet = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("IsDown", true);
        rend = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        SetActionMethod();
        if(initialDelay == 0) actionMethod();
        worldNum = GetWorldNum();
        oldHealth = health;
        deathTime = 0.667f * 3;
        //Debug.Log(actionIndex);
    }

    private void Update()
    {
        RetryGetWorldNum();

        CheckHealth();

        if (deathSet == false && initDelayTimer >= initialDelay)
        {
            
            ChangeAnimState();
            SetActionMethod();
            actionMethod();
        }
        else initDelayTimer += Time.deltaTime;
    }

    private void LateUpdate()
    {
        if(!deathSet) HandleCollider();

    }

    //:::::::::GENERIC::::::::::://

    private void SetActionMethod()
    {
        if (actionIndex != oldIndex)
        {
            switch (actionIndex)
            {
                case 0:
                    actionMethod = Continuous;
                    return;
                case 1:
                    actionMethod = Timed;
                    return;
                case 2:
                    actionMethod = Proximity;
                    return;
                case 3:
                    actionMethod = Up;
                    return;
            }
            oldIndex = actionIndex;
            ResetStart();
        }
    }

    private void ResetStart()
    {
        start = false;
        startA = false;
        startD = false;
        anim.SetBool("IsContinuous", false);
    }

    private void ChangeAnimState()
    {
        if(changeTimer >= animChangeTime)
        {
            //Debug.Log("Changing Animation Controller");
            if (worldNum == wS.activeWorldNum)
            {
                if (anim.runtimeAnimatorController == AcontrollerIdle)
                {
                    anim.runtimeAnimatorController = BcontrollerIdle;
                    isControllerA = false;
                }
                else if (anim.runtimeAnimatorController == BcontrollerIdle)
                {
                    anim.runtimeAnimatorController = AcontrollerIdle;
                    isControllerA = true;
                }
            }

            changeTimer = 0;
        }

        changeTimer += Time.deltaTime;
    }

    //private void ChangeAnims()
    //{
    //    if(isHurt)
    //    {
    //        anim.runtimeAnimatorController = controllerSad;
    //    }
    //    else
    //    {
    //        anim.runtimeAnimatorController = controllerMad;
    //    }
    //}



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Shot")
        {
            health -= collision.gameObject.GetComponent<ShotScript>().damage;

        }
    }

    private IEnumerator HurtTimer()
    {
        //isHurt = true;
        anim.runtimeAnimatorController = controllerHurt;

        yield return new WaitForSeconds(0.2f);

        if(isControllerA)
        {
            anim.runtimeAnimatorController = AcontrollerIdle;
        }
        else
        {
            anim.runtimeAnimatorController = BcontrollerIdle;
        }
        //isHurt = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag == "Player")
        {
            //Debug.Log("Collision with Player");

            playerController.GetHurt(transform.position);
        }
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            anim.enabled = false;
            coll.enabled = false;

            if (deathSet == false)
            {
                GetDeathStates();
                actionMethod = null;
                StopAllCoroutines();
                StartCoroutine(DeathSequence());
                deathSet = true;
            }

            if (dTimer >= deathTime)
            {
                wS.DestroyEnemyValue(hash);
                Destroy(gameObject);
            }

            dTimer += Time.deltaTime;
        }
        else if (oldHealth > health && deathSet == false)
        {
            if (hurtRoutine != null)
            {
                StopCoroutine(hurtRoutine);
            }
            hurtRoutine = StartCoroutine(HurtTimer());

            oldHealth = health;
        }
    }

    private IEnumerator DeathSequence()
    {
        for (int i = 0; i != 3; ++i)
        {
            rend.sprite = whiteSprite;
            yield return new WaitForSeconds(0.33f);
            rend.sprite = sadSprite;
            yield return new WaitForSeconds(0.33f);
        }
    }

    private void RetryGetWorldNum()
    {
        if (worldNum == 10)
        {
            worldNum = GetWorldNum();
        }
    }

    private int GetWorldNum()
    {
        int num = 10;
        if (hash != null)
        {
            if (hash.Contains("W1"))
            {
                num = 0;
            }
            else if (hash.Contains("W2"))
            {
                num = 1;
            }
            else if (hash.Contains("W3"))
            {
                num = 2;
            }
            else if (hash.Contains("W4"))
            {
                num = 3;
            }
        }
        return num;

    }

    private void HandleCollider()
    {
        if (anim.GetBool("IsDown") == true || wS.activeWorldNum != worldNum)
        {
            if (coll.enabled == true)
            {
                coll.enabled = false;
            }
        }
        else if (coll.enabled == false)
        {
            coll.enabled = true;
        }
    }

    private void GetDeathStates()
    {
        Sprite c = rend.sprite;
        Debug.Log(c.name);

        string num = null;

        for (int i = 0; i != c.name.Length; ++i)
        {
            //Debug.Log(c.name[i] + " : " + (int)char.GetNumericValue(c.name[i]));
            int temp = (int)char.GetNumericValue(c.name[i]);

            if (temp != -1)
            {
                num += temp.ToString();
            }
        }

        Debug.Log(num);

        if (anim.runtimeAnimatorController == AcontrollerIdle)
        {
            if (num != null)
            {
                for (int i = 0; i != whiteSpritesA.Length; ++i)
                {
                    if (whiteSpritesA[i].name.Contains(num))
                    {
                        whiteSprite = whiteSpritesA[i];
                        break;
                    }
                }
            }
            else
            {
                whiteSprite = whiteSpritesA[0];
            }

            if (num != null)
            {
                for (int i = 0; i != idleSpritesA.Length; ++i)
                {
                    if (idleSpritesA[i].name.Contains(num))
                    {
                        sadSprite = idleSpritesA[i];
                        break;
                    }
                }
            }
            else
            {
                sadSprite = idleSpritesA[0];
            }
        }
        else if (anim.runtimeAnimatorController == BcontrollerIdle)
        {
            if (num != null)
            {
                for (int i = 0; i != whiteSpritesB.Length; ++i)
                {
                    if (whiteSpritesB[i].name.Contains(num))
                    {
                        whiteSprite = whiteSpritesB[i];
                        break;
                    }
                }
            }
            else
            {
                whiteSprite = whiteSpritesB[0];
            }

            if (num != null)
            {
                for (int i = 0; i != idleSpritesB.Length; ++i)
                {
                    if (idleSpritesB[i].name.Contains(num))
                    {
                        sadSprite = idleSpritesB[i];
                        break;
                    }
                }
            }
            else
            {
                sadSprite = idleSpritesB[0];
            }
        }

        Debug.Log(whiteSprite.name);
        Debug.Log(sadSprite.name);

    }

    //::::::::::::CONTINOUS::::::::://
    private void Continuous()
    {
        anim.SetBool("IsDown", false);
        anim.SetBool("IsContinuous", true);
        if (!start)
        {
            StartCoroutine(ContinousAnimation());
        }
    }

    private IEnumerator ContinousAnimation()
    {
        start = true;
        while (actionIndex == 0)
        {
            anim.SetTrigger("IsAscending");
            yield return new WaitForSeconds(anims[0].length);
            anim.SetTrigger("IsDescending");
            yield return new WaitForSeconds(anims[1].length);
        }
        start = false;
    }


    //:::::::::::TIMED::::::::::::://
    private void Timed()
    {
        anim.SetBool("IsDown", false);
        if (start == false)
        {
            StartCoroutine(TimedAnimation());
        }
    }

    private IEnumerator TimedAnimation()
    {
        start = true;
        yield return new WaitForSeconds(delay);
        while (actionIndex == 1)
        {
            anim.SetTrigger("IsAscending");
            yield return new WaitForSeconds(anims[0].length);
            anim.SetBool("IsUp", true);
            yield return new WaitForSeconds(timer);
            anim.SetBool("IsUp", false);
            anim.SetTrigger("IsDescending");
            yield return new WaitForSeconds(anims[1].length);
            anim.SetBool("IsDown", true);
            yield return new WaitForSeconds(timer);
            anim.SetBool("IsDown", false);
        }
        start = false;
    }

    //::::::::::PROXIMITY:::::::::://
    private void Proximity()
    {
        bool withinDistance = FindMagnitude();

        //Debug.Log("Get Bool - " + anim.GetBool("IsDown") + " : " + anim.GetBool("IsUp"));
        //Debug.Log("WD + SA - " + withinDistance + " : " + startA);

        if (!anim.GetBool("IsUp") && !anim.GetBool("IsDown"))
        {
            //
            //Debug.Log("not up or down");
        }
        else if (withinDistance && startA == false && !anim.GetBool("IsUp"))
        {
            if (routineP != null)
            {
                StopCoroutine(routineP);
            }
            routineP = StartCoroutine(ProximityA());
        }
        else if (!withinDistance && startD == false && !anim.GetBool("IsDown"))
        {
            if (routineP != null)
            {
                StopCoroutine(routineP);
            }
            routineP = StartCoroutine(ProximityD());
        }
    }

    private IEnumerator ProximityA()
    {
        //Debug.Log("Starting Coroutine A:");
        startD = false;
        startA = true;
        anim.SetBool("IsDown", false);
        anim.SetBool("IsUp", false);
        anim.SetTrigger("IsAscending");
        yield return new WaitForSeconds(anims[0].length);
        anim.SetBool("IsUp", true);
    }

    private IEnumerator ProximityD()
    {
        //Debug.Log("Starting Coroutine D");
        startA = false;
        startD = true;
        anim.SetBool("IsUp", false);
        anim.SetBool("IsDown", false);
        anim.SetTrigger("IsDescending");
        yield return new WaitForSeconds(anims[1].length);
        anim.SetBool("IsDown", true);
    }

    private bool FindMagnitude()
    {
        bool withinDistance = false;

        if (player != null)
        {
            Vector3 offset = player.position - transform.position;
            float magnitude = Mathf.Sqrt(Mathf.Pow(offset.x, 2) + Mathf.Pow(offset.y, 2));

            if (magnitude < dangerClose)
            {
                withinDistance = true;
            }
        }

        return withinDistance;
    }

    //::::::::::::UP::::::::::::://

    private void Up()
    {
        anim.SetBool("IsDown", false);
        anim.SetBool("IsUp", true);
    }
}
