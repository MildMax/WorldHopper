﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishScript : EnemyBase
{
    public Vector2[] swimPoints;

    public float speed;
    public float frequency;
    public float magnitude;

    private int direction = 1;

    float xPos;
    bool isDead = false;

    SpriteRenderer rend;
    Animator anim;
    Rigidbody2D body;
    PlayerController playerController;
    WorldSwitcher wS;

    private void Awake()
    {
        transform.position = swimPoints[0];
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
    }

    private void Update()
    {
        CheckHealth();
        if (isDead == false)
        {
            SwitchDirection();
            Swim();
        }
        else if(isDead == true)
        {
            FloatGentlyDownStream();
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
        if(transform.position.x <= swimPoints[0].x)
        {
            direction = 1;
            rend.flipX = true;
        }
        else if(transform.position.x >= swimPoints[1].x)
        {
            direction = -1;
            rend.flipX = false;
        }
    }

    private void CheckHealth()
    {
        if(health <= 0)
        {
            anim.SetBool("IsDead", true);
            isDead = true;
            body.isKinematic = false;
            StartCoroutine(DestroyFish());
        }
    }

    private IEnumerator DestroyFish()
    {
        yield return new WaitForSeconds(2);
        wS.enemyDestroyed = true;
        Destroy(gameObject);
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
        else if(collision.gameObject.tag == "Player" && isDead == true)
        {
            Physics2D.IgnoreLayerCollision(gameObject.layer, collision.gameObject.layer);
        }
    }

    private void FloatGentlyDownStream()
    {
        if(isDead == true && body.velocity.y <= -1.5f)
        {
            body.velocity = new Vector2(0, -1.5f);
        }
    }
}
