using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOW USED FOR BEE
//BAT WILL RUN ON PATHFINDING-DEPENDENT SCRIPT

public class BatScript : EnemyBase
{
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;

    public float waitMin;
    public float waitMax;
    public float speed;

    public AnimationClip deathClip;

    Vector2 originalPos;
    Vector2 newPos;
    SpriteRenderer rend;
    PlayerController playerController;
    Animator animator;
    CapsuleCollider2D coll;
    RaycastHit2D fallDistance;
    WorldSwitcher wS;

    float waitTime;
    float timer;

    float oldHealth;

    bool set = false;
    bool startFall = false;
    bool isDead = false;
    float deathTime;
    float dTimer = 0;
    bool pointSet = false;
    Vector2 destination;
    Vector2 vel;

    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        animator = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();
        originalPos = transform.position;
        oldHealth = health;
        deathTime = deathClip.length * 3;
        RetryHash();
    }

    private void Update()
    {
        RetryHash();
        HealthForHurt();
        CheckHealth();

        if(set == true)
        {
            SetTimer();
            SetMove();
            SetDirection();
            set = false;
        } 
    }

    private void FixedUpdate()
    {
        if (startFall == false)
        {
            if (timer < waitTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                if ((Vector2)transform.position != newPos)
                {
                    transform.position = Vector2.MoveTowards(transform.position, newPos, speed * Time.deltaTime);
                }
                else
                {
                    set = true;
                }
            }
        }
        else
        {
            Fall();
        }
    }

    private void SetTimer()
    {
        waitTime = Random.Range(waitMin, waitMax);
        timer = 0;
    }

    private void SetMove()
    {
        originalPos = newPos;

        newPos = new Vector2(
            Random.Range(xMin, xMax),
            Random.Range(yMin, yMax)
            );
    }

    private void SetDirection()
    {
        if(newPos.x > originalPos.x)
        {
            rend.flipX = true;
        }
        else if(newPos.x < originalPos.x)
        {
            rend.flipX = false;
        }
        else if(newPos.x == originalPos.x)
        {
            rend.flipX = !rend.flipX;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Shot")
        {
            //Debug.Log("Collision with shot");
            health -= collision.gameObject.GetComponent<ShotScript>().damage;
            //animator.SetTrigger("IsHurt");
            //CheckHealth();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collision detected");
        if (collision.gameObject.tag == "Player")
        {
            //Debug.Log("Collision with Player");

            playerController.GetHurt(transform.position);

            //set damage here as well;
        }
    }

    private void HealthForHurt()
    {
        if (oldHealth > health)
        {
            animator.SetTrigger("IsHurt");
        }
        oldHealth = health;
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            if (isDead == false)
            {
                fallDistance = Physics2D.Raycast(transform.position, -Vector2.up, 150, LayerMask.GetMask("Ground" + layerString));
                animator.SetBool("IsDead", true);
                coll.enabled = false;
                startFall = true;
                isDead = true;
            }

            if (dTimer >= deathTime)
            {
                wS.DestroyEnemyValue(hash);
                Destroy(gameObject);
            }

            dTimer += Time.deltaTime;
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

    
