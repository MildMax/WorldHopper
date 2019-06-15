using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarnacleScript : EnemyBase
{
    public float deathWait;

    float oldHealth;

    Animator anim;
    WorldSwitcher wS;
    BoxCollider2D coll;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        coll = GetComponent<BoxCollider2D>();
        oldHealth = health;
    }

    private void Update()
    {
        HealthForHurt();
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
        if(health <= 0)
        {
            anim.SetBool("IsDead", true);
            coll.enabled = false;
            StartCoroutine(KillSequence());
        }
    }

    private IEnumerator KillSequence()
    {
        yield return new WaitForSeconds(deathWait);
        wS.DestroyEnemyValue(hash);
        Destroy(gameObject);
    }

}
