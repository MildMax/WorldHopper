using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{
    BoxCollider2D coll;
    SpriteRenderer rend;
    Animator anim;
    GhostTriggerScript detector;
    Vector2 oldPlayerPos;
    GameObject player;

    public AnimationClip deathClip;
    public AnimationClip awakeClip;

    public float speed;
    public float redirectWait;
    public float followWait;

    [HideInInspector]
    public int worldNum;

    float redirectTimer = 0;
    float deathTimer = 0;
    float deathWait;
    float followTimer = 0;
    float awakeWait;
    float awakeTimer = 0;
    bool inWorld = false;
    bool activeSet = false;
    bool deathSet = false;
    bool isDead = false;
    bool isAwake = false;

    private void Awake()
    {
        detector = GetComponentInParent<GhostTriggerScript>();
        player = GameObject.FindGameObjectWithTag("Player");
        coll = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        SetInitial();
        deathWait = deathClip.length;
        awakeWait = awakeClip.length;
    }

    private void Update()
    {
        if (detector.inVicinity == true && isDead == false)
        {
            if (activeSet == false)
            {
                SetToActive();
            }

            if (isAwake == false)
            {
                AwakeTimer();
                
            }
            else if (isAwake == true)
            {
                
                FlipSprites();
                UseDelay();
                FollowTimer();
            }
        }

        if(isDead == true)
        {
            KillSequence();
        }
    }

    private void FixedUpdate()
    {
        MoveGhost();
    }

    private void MoveGhost()
    {
        if (detector.inVicinity == true && isDead == false && isAwake == true)
        {
            transform.position = transform.position = Vector2.MoveTowards(transform.position, oldPlayerPos, speed * Time.deltaTime);
        }
    }

    private void KillSequence()
    {
        if(deathSet == false)
        {
            coll.enabled = false;
            anim.SetBool("IsDead", true);
            deathSet = true;
        }

        if(deathTimer >= deathWait)
        {
            detector.destroyThis = true;
            Destroy(gameObject);
        }

        deathTimer += Time.deltaTime;
    }

    private void UseDelay()
    {
        if (redirectTimer >= redirectWait)
        {
            oldPlayerPos = player.transform.position;
            redirectTimer = 0;
        }

        redirectTimer += Time.deltaTime;
    }

    private void FlipSprites()
    {
        if (detector.inVicinity == true)
        {
            if (transform.position.x > oldPlayerPos.x)
            {
                rend.flipX = false;
            }
            else if (transform.position.x <= oldPlayerPos.x)
            {
                rend.flipX = true;
            }
        }

    }

    private void FollowTimer()
    {
        if(followTimer >= followWait)
        {
            isDead = true;
        }

        followTimer += Time.deltaTime;
    }

    private void AwakeTimer()
    {
        if(awakeTimer >= awakeWait)
        {
            isAwake = true;
            oldPlayerPos = player.transform.position;
        }

        awakeTimer += Time.deltaTime;
    }

    private void SetInitial()
    {
        rend.enabled = false;
        coll.enabled = false;
        anim.enabled = false;
    }

    private void SetToActive()
    {
        anim.enabled = true;
        rend.enabled = true;
        coll.enabled = true;
        activeSet = true;
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().GetHurt(coll.transform.position);
            isDead = true;
        }
    }
}
