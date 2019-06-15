﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyScript : EnemyBase
{
    public Vector2 pos1;
    public Vector2 pos2;
    public float speed;

    int direction = 1;
    float oldHealth;

    SpriteRenderer rend;
    Animator animator;
    Rigidbody2D body;
    CapsuleCollider2D coll;
    PlayerController playerController;
    WorldSwitcher wS;

    bool isDead = false;

    private void Awake()
    {
        transform.position = pos1;
        rend = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        oldHealth = health;
    }

    private void Update()
    {
        HealthForHurt();
        CheckHealth();
        MoveFly();
        UpdateDirection();
        FlipSprite();
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
        if(health <= 0 && isDead == false)
        {
            animator.SetBool("IsDead", true);
            body.isKinematic = false;
            Physics2D.IgnoreLayerCollision(gameObject.layer, 9);
            StartCoroutine(KillSequence());
            isDead = true;
        }
    }

    private IEnumerator KillSequence()
    {
        yield return new WaitForSeconds(2.5f);
        wS.DestroyEnemyValue(hash);
        Destroy(gameObject);
    }
}
