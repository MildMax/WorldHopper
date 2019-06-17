using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnacleScript : EnemyBase
{
    public float deathWait;
    public AnimationClip deathAnim;

    float oldHealth;

    float deathTime;
    float timer = 0;

    bool isDead = false;

    Animator anim;
    WorldSwitcher wS;
    BoxCollider2D coll;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        coll = GetComponent<BoxCollider2D>();
        oldHealth = health;
        deathTime = deathAnim.length * 3;
    }

    private void Update()
    {
        CheckHealth();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().GetHurt(transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Shot")
        {
            health -= collision.gameObject.GetComponent<ShotScript>().damage;
        }
    }

    private void CheckHealth()
    {
        if(health <= 0)
        {
            if (isDead == false)
            {
                anim.SetBool("IsDead", true);
                coll.enabled = false;
                isDead = true;
            }

            if(timer >= deathTime)
            {
                wS.DestroyEnemyValue(hash);
                Destroy(gameObject);
            }

            timer += Time.deltaTime;
        }
        else
        {
            if (oldHealth > health)
            {
                anim.SetTrigger("IsHurt");
            }

            oldHealth = health;
        }
    }

}
