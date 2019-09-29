using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiranhaScript : EnemyBase
{
    public Vector2 top;
    public Vector2 bottom;
    float mid;
    float midPos;

    public float speed;
    public float maxSpeed;
    public float minSpeed;

    int direction;
    bool isUp = false;

    SpriteRenderer rend;
    Animator anim;
    CapsuleCollider2D coll;
    WorldSwitcher wS;

    public AnimationClip deathClip;
    float deathWait;
    float deathTimer = 0;

    float oldHealth;

    bool deathSet = false;
    bool isDead = false;

    //fall stuff
    RaycastHit2D fallDistance;
    bool pointSet = false;
    Vector2 destination;
    Vector2 vel;
    float velAdjust = 0.005f;

    public bool inWater = true;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        coll = GetComponent<CapsuleCollider2D>();
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        FindLayer(hash);
        GetMid();
        SetInitialPos();
        oldHealth = health;
        deathWait = deathClip.length * 3;
    }

    private void Update()
    {
        RetryHash();

        CheckHealth();
    }

    private void FixedUpdate()
    {
        if (isDead == false)
        {
            ChangeDirection();
            MovePirhana();
        }
        else
        {
            Fall();
        }
    }

    private void SetInitialPos()
    {
        transform.position = bottom;
        direction = 1;
        isUp = false;
        rend.flipY = false;
        rend.flipX = false;
    }

    private void GetMid()
    {
        mid = Mathf.Abs(top.y - bottom.y) / 2;
        midPos = top.y - mid;
    }

    private void MovePirhana()
    {
        if(direction == 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, top, speed * Time.deltaTime * GetRelativeSpeed());
        }
        else if(direction == -1)
        {
            transform.position = Vector2.MoveTowards(transform.position, bottom, speed * Time.deltaTime * GetRelativeSpeed());
        }
    }

    private float GetRelativeSpeed()
    {
        float s = 0;

        if(transform.position.y >= midPos)
        {
            s = mid - (Mathf.Abs(midPos - transform.position.y));
        }
        else if(transform.position.y < midPos)
        {
            s = mid - (Mathf.Abs(midPos - transform.position.y));
        }

        //Debug.Log(s);

        if (s < minSpeed)
        {
            s = minSpeed;
        }
        else if(s > maxSpeed)
        {
            s = maxSpeed;
        }

        return s;
    }

    private void ChangeDirection()
    {
        if(isUp == false && transform.position.y >= top.y && direction == 1)
        {
            isUp = true;
            direction = -1;
            rend.flipY = true;
            rend.flipX = true;
        }
        else if(isUp == true && transform.position.y <= bottom.y && direction == -1)
        {
            isUp = false;
            direction = 1;
            rend.flipY = false;
            rend.flipX = false;
        }
    }

    private void CheckHealth()
    {
        if(health <= 0)
        {
            if(deathSet == false)
            {
                anim.SetBool("IsDead", true);
                Debug.Log(layerString);
                fallDistance = Physics2D.Raycast(transform.position, -Vector2.up, 150, LayerMask.GetMask("Ground" + layerString));
                isDead = true;
                coll.enabled = false;
                deathSet = true;
            }

            if(deathTimer >= deathWait)
            {
                wS.DestroyEnemyValue(hash);
                Destroy(gameObject);
            }

            deathTimer += Time.deltaTime;
        }
        else if(oldHealth > health)
        {
            anim.SetTrigger("IsHurt");
            oldHealth = health;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Shot")
        {
            health -= collision.gameObject.GetComponent<ShotScript>().damage;
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            inWater = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().GetHurt(coll.transform.position);
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
                vel += velAdjust * Physics2D.gravity * Time.deltaTime;

                Vector2 d = vel * Time.deltaTime;

                Vector2 m = Vector2.up * vel;

                transform.position = (Vector2)transform.position + m;
            }
            
        }
        else
        {
            //Debug.Log("Falling w/out raycast");
            vel += velAdjust * Physics2D.gravity * Time.deltaTime;

            Vector2 d = vel * Time.deltaTime;

            Vector2 m = Vector2.up * vel;

            transform.position = (Vector2)transform.position + m;
        }
    }
}
