using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatScriptFORBAT : EnemyBase
{
    public AnimatorOverrideController hang;
    public AnimatorOverrideController fly;

    Animator anim;
    SpriteRenderer rend;
    BatTriggerDetector detector;
    CapsuleCollider2D coll;
    WorldSwitcher wS;
    GameObject player;
    Vector2 startPos;
    Vector2 oldPlayerPos;

    public float speed;
    float oldHealth;

    float redirectTimer = 0;
    public float redirectWait;

    public AnimationClip deathClip;

    bool isDead = false;
    bool deathSet = false;

    float deathWait;
    float deathTimer = 0;

    //fall stuff
    RaycastHit2D fallDistance;
    bool pointSet = false;
    Vector2 vel;
    Vector2 destination;

    private void Awake()
    {
        detector = GetComponentInParent<BatTriggerDetector>();
        coll = GetComponent<CapsuleCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        wS = player.GetComponentInChildren<WorldSwitcher>();
        anim = GetComponent<Animator>();
        anim.SetBool("IsIdle", true);
        rend = GetComponent<SpriteRenderer>();
        startPos = transform.position;
        oldHealth = health;
        deathWait = deathClip.length * 3;
        RetryHash();
    }

    private void Update()
    {
        RetryHash();

        CheckHealth();
        SwitchOverrideControllers();
        FlipSprites();
        if (detector.inVicinity == true && isDead == false)
        {
            UseDelay();
        }
    }

    private void FixedUpdate()
    {
        if (isDead == false)
        {
            MoveBat();
        }
        else if(isDead == true)
        {
            Fall();
        }
    }

    private void CheckHealth()
    {
        if(health <= 0)
        {
            if(deathSet == false)
            {
                isDead = true;
                coll.enabled = false;
                rend.flipY = true;
                anim.SetBool("IsDead", true);
                fallDistance = Physics2D.Raycast(transform.position, -Vector2.up, 150, LayerMask.GetMask("Ground" + layerString));
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

    private void FlipSprites()
    {
        if (detector.inVicinity == true)
        {
            if ((Vector2)transform.position != startPos)
            {
                if (transform.position.x > oldPlayerPos.x)
                {
                    rend.flipX = false;
                }
                else if (transform.position.x <= oldPlayerPos.x)
                {
                    rend.flipX = true;
                }
            }
        }
        else if(detector.inVicinity == false && (Vector2)transform.position != startPos)
        {
            if(transform.position.x > startPos.x)
            {
                rend.flipX = false;
            }
            else if(transform.position.x <= startPos.x)
            {
                rend.flipX = true;
            }
        }
        else
        {
            rend.flipX = false;
        }
        
    }

    private void MoveBat() 
    {
        if(detector.inVicinity == true)
        {
            transform.position = transform.position = Vector2.MoveTowards(transform.position, oldPlayerPos, speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos, speed * Time.deltaTime);
        }
    }

    private void UseDelay()
    {
        if (redirectTimer >= redirectWait)
        {
            oldPlayerPos = player.transform.position;
            redirectTimer = 0;
        }

        redirectTimer += Time.deltaTime;
    }

    private void SwitchOverrideControllers()
    {
        if(detector.inVicinity == true || (Vector2)transform.position != startPos)
        {
            anim.runtimeAnimatorController = fly;
        }
        else if(detector.inVicinity == false && (Vector2)transform.position == startPos)
        {
            anim.runtimeAnimatorController = hang;
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
            player.GetComponent<PlayerController>().GetHurt(coll.transform.position);
        }
    }

    private void Fall()
    {
        //Debug.Log("Fly falling");

        if (fallDistance)
        {
            //Debug.Log("Falling w/ raycast");
            if (pointSet == false)
            {
                destination = new Vector2(fallDistance.point.x, fallDistance.point.y + (coll.size.y / 2));
                pointSet = true;
            }

            if (transform.position.y >= destination.y)
            {
                vel += 0.015f * Physics2D.gravity * Time.deltaTime;

                Vector2 d = vel * Time.deltaTime;

                Vector2 m = Vector2.up * vel;

                transform.position = (Vector2)transform.position + m;
            }

        }
        else
        {
            //Debug.Log("Falling w/out raycast");
            vel += 0.015f * Physics2D.gravity * Time.deltaTime;

            Vector2 d = vel * Time.deltaTime;

            Vector2 m = Vector2.up * vel;

            transform.position = (Vector2)transform.position + m;
        }
    }
}
