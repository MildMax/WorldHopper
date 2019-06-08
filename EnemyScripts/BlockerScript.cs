using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerScript : EnemyBase
{
    public Sprite idle;
    public Sprite mad;
    public Sprite hurt;

    public float force;
    public float endHealth;

    SpriteRenderer rend;
    Animator animator;
    PlayerController playerController;
    WorldSwitcher wS;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        endHealth = health;
    }

    private void Update()
    {
        CheckHealth();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision detected");

        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("Collision with Player");

            playerController.GetHurt(transform.position);

            //set damage here as well;

            animator.SetTrigger("IsMad");
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Shot")
        {
            Debug.Log("Collision with shot");
            health -= collision.gameObject.GetComponent<ShotScript>().damage;
            animator.SetTrigger("IsHurt");

            CheckHealth();
        }
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            wS.DestroyEnemyValue(hash);
            Destroy(gameObject);
        }
    }
}
