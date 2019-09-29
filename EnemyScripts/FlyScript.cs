using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyScript : EnemyBase
{
    public Vector2 pos1;
    public Vector2 pos2;
    public AnimationClip deathAnim;
    public float speed;

    public int direction = 1;
    float oldHealth;

    float deathTime;
    float timer = 0;

    SpriteRenderer rend;
    Animator animator;
    Rigidbody2D body;
    CapsuleCollider2D coll;
    PlayerController playerController;
    WorldSwitcher wS;

    bool isDead = false;

    //for Fall() function
    RaycastHit2D fallDistance;
    Vector2 destination;
    Vector2 vel;

    bool pointSet = false;
    bool startFall = false;

    private void Awake()
    {
        //transform.position = pos1;
        rend = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        oldHealth = health;
        deathTime = deathAnim.length * 3;
        RetryHash();
    }

    private void Update()
    {
        RetryHash();
        HealthForHurt();
        CheckHealth();
        
        UpdateDirection();
        FlipSprite();
    }

    private void FixedUpdate()
    {
        if (startFall == false)
        {
            MoveFly();
        }
        else
        {
            Fall();
        }
    }

    private void UpdateDirection()
    {
        if(changeDirection == true)
        {
            direction *= -1;
            changeDirection = false;
        }
        if ((Vector2)transform.position == pos2)
        {
            direction = -1;           
        }
        else if((Vector2)transform.position == pos1)
        {
            direction = 1; 
        }
    }

    private void FlipSprite()
    {
        if(direction == -1)
        {
            rend.flipX = false;
        }
        else if(direction == 1)
        {
            rend.flipX = true;
        }
    }

    private void MoveFly()
    {
        if (isDead == false)
        {
            if (direction == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position, pos2, speed * Time.deltaTime);
            }
            else if (direction == -1)
            {
                transform.position = Vector2.MoveTowards(transform.position, pos1, speed * Time.deltaTime);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collision detected");
        if (collision.gameObject.tag == "Player")
        {
            //Debug.Log("Collision with Player");

            playerController.GetHurt(coll.transform.position);

            //set damage here as well;
        }
        else if(collision.gameObject.layer == LayerMask.GetMask("Ground") >> 5)
        {
            //Debug.Log("contact with ground");
            body.constraints = RigidbodyConstraints2D.FreezeAll;
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

    private void HealthForHurt()
    {
        if(oldHealth > health)
        {
            animator.SetTrigger("IsHurt");
        }
        oldHealth = health;
    }

    private void CheckHealth()
    {
        if(health <= 0)
        {
            if (isDead == false)
            {
                fallDistance = Physics2D.Raycast(transform.position, -Vector2.up, 150, LayerMask.GetMask("Ground" + layerString));
                animator.SetBool("IsDead", true);
                coll.enabled = false;
                startFall = true;
                isDead = true;
            }
            
            if(timer >= deathTime)
            {
                wS.DestroyEnemyValue(hash);
                Destroy(gameObject);
            }

            timer += Time.deltaTime;
        }
    }

    private void Fall()
    {
        //Debug.Log("Fly falling");

        if(fallDistance)
        {
            //Debug.Log("Falling w/ raycast");
            if(pointSet == false)
            {
                destination = new Vector2(fallDistance.point.x, fallDistance.point.y + (coll.size.y / 2));
                pointSet = true;
            }

            if(transform.position.y >= destination.y)
            {
                vel += 0.015f * Physics2D.gravity * Time.deltaTime;

                Vector2 d = vel * Time.deltaTime;

                Vector2 m = Vector2.up * vel;

                body.position = body.position + m;
            }
            else
            {
                body.velocity = Vector2.zero;
            }
        }
        else
        {
            //Debug.Log("Falling w/out raycast");
            vel += 0.015f * Physics2D.gravity * Time.deltaTime;

            Vector2 d = vel * Time.deltaTime;

            Vector2 m = Vector2.up * vel;

            body.position = body.position + m;
        }
    }

    //private IEnumerator KillSequence()
    //{
    //    yield return new WaitForSeconds(2.5f);
    //    wS.DestroyEnemyValue(hash);
    //    Destroy(gameObject);
    //}
}
