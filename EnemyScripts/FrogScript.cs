using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogScript : EnemyBase
{
    public Vector2 pos1;
    public Vector2 pos2;

    public AnimationClip deathClip;
    float deathWait;
    float deathTimer = 0;

    PlayerController playerController;
    CircleCollider2D coll;
    WorldSwitcher wS;
    Animator anim;

    public float hopDistance;
    float halfD;
    public float hopHeight;

    public int direction;
    public float speed;

    public float hopWait;
    float hopTimer = 0;

    bool jumpSet = false;

    //for calculating yMovement in MoveY()
    float startPos;
    float endPos;
    float midPos;
    float xLength;
    //Set to false after jump is completed****
    bool ySet = false;
    public float hopAdjust;
    float originalYPosition;

    float oldHealth;

    bool deathSet = false;
    bool isDead = false;

    //fall stuff
    RaycastHit2D fallDistance;
    Vector2 destination;
    Vector2 vel;
    bool pointSet = false;


    private void Awake()
    {
        originalYPosition = transform.position.y;
        halfD = hopDistance / 2;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        coll = GetComponent<CircleCollider2D>();
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        anim = GetComponent<Animator>();
        oldHealth = health;
        deathWait = deathClip.length * 3;
    }

    private void Update()
    {
        CheckHealth();
    }

    //Consider moving ChangeDirection + MoveFrog to Update
    //NOTE::::::: ChangeDirection MUST COME BEFORE MoveFrog
    private void FixedUpdate()
    {

        if (isDead == false)
        {
            ChangeDirection();
            MoveFrog();
        }
        else
        {
            if (transform.position.y > originalYPosition)
            {
                Fall();
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
            playerController.GetHurt(coll.transform.position);
        }
    }

    private void CheckHealth()
    {
        if(health <= 0)
        {
            if (deathSet == false)
            {
                isDead = true;
                coll.enabled = false;
                fallDistance = Physics2D.Raycast(transform.position, -Vector2.up, 150, LayerMask.GetMask("Ground" + layerString));
                anim.SetBool("IsDead", true);
            }
            if(deathTimer >= deathWait)
            {
                wS.DestroyEnemyValue(hash);
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

    private void MoveFrog()
    {
        if(hopTimer >= hopWait)
        {
            if(jumpSet == false)
            {
                anim.SetBool("IsJumping", true);
                jumpSet = true;
            }

            //move frog here, detect when frog has finished hop, set timer to 0, set ySet to false
            float xPos = MoveX();
            //must set xLength before calling MoveY()
            xLength += Mathf.Abs(xPos);

            float yPos = MoveY();

            if (xLength >= hopDistance)
            {
                transform.position = new Vector2(transform.position.x, originalYPosition);
                hopTimer = 0;
                ySet = false;
                xLength = 0;

                anim.SetBool("IsJumping", false);
                jumpSet = false;
            }
            else
            {
                transform.position = new Vector2(transform.position.x + xPos, originalYPosition + yPos);
            }
        
        }
        else
        {
            hopTimer += Time.deltaTime;
        }
    }

    private float MoveX()
    {
        float xMovement = 0;

        if (direction > 0)
        {
            xMovement += speed * Time.deltaTime;
        }
        else if(direction < 0)
        {
            xMovement += -speed * Time.deltaTime;
        }

        return xMovement;
    }

    private float MoveY()
    {
        if(ySet == false)
        {
            SetYVariables();
            ySet = true;
        }

        float yMovement = 0;

        //if(xLength <= halfD)
        //{
        //    yMovement = (xLength / halfD) * hopHeight;
        //}
        //else if(xLength > halfD)
        //{
        //    yMovement = ((hopDistance - xLength) / halfD) * hopHeight;
        //}

        yMovement = -hopHeight * Mathf.Pow(xLength - halfD, 2) + hopAdjust;

        return yMovement;
    }

    private void SetYVariables()
    {
        startPos = transform.position.x;
        endPos = transform.position.x + hopDistance;
        midPos = startPos + halfD;
    }

    private void ChangeDirection()
    {
        if(direction == 1 && transform.position.x >= pos2.x)
        {
            direction = -1;
        }
        else if(direction == -1 && transform.position.x <= pos1.x)
        {
            direction = 1;
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
                destination = new Vector2(fallDistance.point.x, fallDistance.point.y + coll.radius);
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
