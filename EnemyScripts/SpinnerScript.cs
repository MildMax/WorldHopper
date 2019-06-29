using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerScript : EnemyBase
{
    public AnimationClip deathAnim;

    float stunTime;
    float timer = 0f;

    Animator animator;
    CircleCollider2D coll;
    PlayerController playerController;

    bool deathSet = false;
    int startHealth;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        coll = GetComponent<CircleCollider2D>();
        stunTime = deathAnim.length;
        startHealth = health;
    }

    private void Update()
    {
        CheckHealth();
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Shot")
        {
            //Debug.Log("Collision with shot");
            health -= collision.gameObject.GetComponent<ShotScript>().damage;
            animator.SetTrigger("IsHurt");
        }
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            if (timer >= stunTime)
            {
                coll.enabled = true;
                animator.SetBool("IsDead", false);
                health = startHealth;
                timer = 0;
                deathSet = false;
            }
            else if (deathSet == false)
            {
                coll.enabled = false;
                animator.SetBool("IsDead", true);
                deathSet = true;
            }

            timer += Time.deltaTime;
        }
    }
}
