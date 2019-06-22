using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishScript : EnemyBase
{
    public Vector2[] swimPoints;
    public AnimationClip deathAnim;

    public float speed;
    public float frequency;
    public float magnitude;

    private int direction = 1;

    float xPos;
    bool isDead = false;

    float deathTime;
    float timer = 0;

    SpriteRenderer rend;
    Animator anim;
    Rigidbody2D body;
    PlayerController playerController;
    WorldSwitcher wS;
    CapsuleCollider2D coll;

    float oldHealth;

    //for falling
    bool startFall = false;
    bool pointSet = false;
    RaycastHit2D fallDistance;
    Vector2 destination;
    Vector2 vel;

    private void Awake()
    {
        transform.position = swimPoints[0];
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
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
        if (startFall == false)
        {
            SwitchDirection();
        }
    }

    private void FixedUpdate()
    {
        if(startFall == false)
        {
            Swim();
        }
        else
        {
            Fall();
        }
    }

    private void Swim()
    {
        xPos = speed * Time.deltaTime;
        if (direction == 1)
        {
            transform.position = new Vector3(transform.position.x + xPos, transform.position.y + Mathf.Sin(Time.time * frequency) * magnitude, 0f);
        }
        else if(direction == -1)
        {
            transform.position = new Vector3(transform.position.x - xPos, transform.position.y + Mathf.Sin(Time.time * frequency) * magnitude, 0f);
        }
    }

    private void SwitchDirection()
    {
        if(changeDirection == true)
        {
            direction *= -1;
            rend.flipX = !rend.flipX;
            changeDirection = false;
        }
        else if(transform.position.x <= swimPoints[0].x)
        {
            direction = 1;
            rend.flipX = true;
            changeDirection = false;
        }
        else if(transform.position.x >= swimPoints[1].x)
        {
            direction = -1;
            rend.flipX = false;
            changeDirection = false;
        }
    }

    private void HealthForHurt()
    {
        if(oldHealth > health)
        {
            anim.SetTrigger("IsHurt");
        }

        oldHealth = health;
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            if (isDead == false)
            {
                coll.enabled = false;
                anim.SetBool("IsDead", true);
                fallDistance = Physics2D.Raycast(transform.position, -Vector2.up, 150, LayerMask.GetMask("Ground" + layerString));
                startFall = true;
                isDead = true;
            }

            if (timer >= deathTime)
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
                vel += 0.005f * Physics2D.gravity * Time.deltaTime;

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
            vel += 0.005f * Physics2D.gravity * Time.deltaTime;

            Vector2 d = vel * Time.deltaTime;

            Vector2 m = Vector2.up * vel;

            body.position = body.position + m;
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
        if (collision.gameObject.tag == "Player" && isDead == false)
        {
            //Debug.Log("Collision with Player");

            playerController.GetHurt(transform.position);

            //set damage here as well;
        }
    }
}
