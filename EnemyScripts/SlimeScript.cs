using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeScript : EnemyBase
{
    public int speed;
    public Vector2[] walkPoints;

    PlayerController playerController;
    Animator anim;
    CapsuleCollider2D coll;
    SpriteRenderer rend;
    Rigidbody2D body;
    Vector2[] autoWalkPoints = new Vector2[3] { Vector2.positiveInfinity, Vector2.positiveInfinity, Vector2.positiveInfinity };
    Vector2[] autoWalkExtents = new Vector2[2];
    WorldSwitcher wS;

    int direction = 1;
    bool isDead = false;

    //serialize/gui shite
    public delegate void WalkMethod();
    public WalkMethod walkType;
 
    [HideInInspector]
    public int walkIndex;

    bool deathPos = false;

    bool ignore = false;

    bool isGrounded = false;

    public string groundLayer;

    [HideInInspector]
    public int worldNum;

    private void Awake()
    {
        SetInitWalk();
        coll = GetComponent<CapsuleCollider2D>();
        rend = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        autoWalkPoints = SetAutoWalk();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        groundLayer = "Ground" + (worldNum + 1);
    }

    private void Update()
    {
        CheckHealth();
    }

    private void FixedUpdate()
    {
        SetCollisionType();
        SwitchColliderType();
        walkType();
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    //:::::::::::::::::GENERIC:::::::::::::::::://

    private void UpdateDirection(Vector2[] points)
    {
        if(changeDirection == true)
        {
            direction *= -1;
            changeDirection = false;
        }
        else if ((Vector2)transform.position == points[1])
        {
            direction = -1;
        }
        else if ((Vector2)transform.position == points[0])
        {
            direction = 1;
        }
    }

    private void FlipSprite()
    {
        if (direction == -1)
        {
            rend.flipX = false;
        }
        else if (direction == 1)
        {
            rend.flipX = true;
        }
    }

    private void SetInitWalk()
    {
        switch (walkIndex)
        {
            case 0:
                walkType = AutoWalk;
                return;
            case 1:
                walkType = PointWalk;
                return;
            default:
                return;

        }
    }

    private void SetCollisionType()
    {
        if(body.isKinematic == false)
        {
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }
        else
        {
            body.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
        }
    }

    private void SwitchColliderType()
    {
        if(isGrounded == false)
        {
            coll.isTrigger = true;
        }
        else if(isGrounded == true)
        {
            coll.isTrigger = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("Collision detected");
        if (collision.gameObject.tag == "Player")
        {
            playerController.GetHurt(coll.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Shot" && health > 0)
        {
            health -= collision.gameObject.GetComponent<ShotScript>().damage;
        }
        else if(collision.gameObject.tag == "Player")
        {
            playerController.GetHurt(coll.transform.position);
        }
    }

    private void CheckHealth()
    {
        if(health <= 0)
        {
            anim.SetBool("IsDead", true);
            body.isKinematic = true;
            
            if(deathPos == false)
            {
                transform.position -= new Vector3(0f, 0.1f, 0f);
                deathPos = true;
            }

            walkType = NoWalk;
            StartCoroutine(KillWait());
        }
    }

    private IEnumerator KillWait()
    {
        yield return new WaitForSeconds(2.5f);
        wS.DestroyEnemyValue(hash);
        Destroy(gameObject);
    }

    //::::::::::::::AUTOWALK:::::::::::::::://

    public void AutoWalk()
    {
        if (autoWalkPoints[2] == Vector2.positiveInfinity
            || transform.position.x < autoWalkPoints[0].x - 0.5f
            || transform.position.x > autoWalkPoints[1].x + 0.5f
            || transform.position.y > autoWalkPoints[0].y + 0.5f
            || transform.position.z < autoWalkPoints[0].y - 0.5f)
        {

            //Debug.Log("Setting points");
            autoWalkPoints = SetAutoWalk();
        }
        else
        {
            body.isKinematic = true;
            isGrounded = true;
            transform.position = new Vector2(transform.position.x, autoWalkPoints[0].y);
            body.velocity = Vector2.zero;
            //Debug.Log("Walking");
            Walk(autoWalkPoints);
        }
        //Debug.Log("Autowalk being called");
        //
    }

    private Vector2[] SetAutoWalk()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);

        Vector2[] results = new Vector2[3] {Vector2.positiveInfinity, Vector2.positiveInfinity, Vector2.positiveInfinity };

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, (rend.size.y / 2) + 0.25f, LayerMask.GetMask(groundLayer));

        if(hit)
        {
            //Debug.Log(hit.transform.gameObject.name);

            results[0] = new Vector2(   hit.transform.position.x - hit.collider.bounds.extents.x + (rend.size.x / 2), 
                                        hit.transform.position.y + hit.collider.bounds.extents.y + (rend.size.y / 2));
            results[1] = new Vector2(   hit.transform.position.x + hit.collider.bounds.extents.x - (rend.size.x / 2),
                                        hit.transform.position.y + hit.collider.bounds.extents.y + (rend.size.y / 2));
            results[2] = hit.transform.position;

            //CHECK THIS:::::::::::::::::::::
            float distanceL = (hit.point.x - hit.transform.position.x) + hit.collider.bounds.extents.x;

            //Debug.Log(hit.transform.gameObject.layer.ToString());

            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, -transform.right, distanceL, LayerMask.GetMask(groundLayer));

            //if(hitLeft)
            //{
            //    Debug.Log("HitLeft: " + hitLeft.transform.gameObject.name);
            //}

            if (hitLeft && hitLeft.distance < Mathf.Abs(hit.transform.position.x - hit.collider.bounds.extents.x))
            {

                results[0] = new Vector2(transform.position.x - hitLeft.distance + (rend.size.x / 2), results[0].y);
            }

            //CHECK THIS:::::::::::::::::::::::::
            float distanceR = (hit.transform.position.x - hit.point.x) + hit.collider.bounds.extents.x;

            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, transform.right, distanceR, LayerMask.GetMask(groundLayer));

            //if(hitRight)
            //{
            //    Debug.Log("HitRight: " + hitRight.transform.gameObject.name);
            //}

            if(hitRight && hitRight.distance < Mathf.Abs(hit.transform.position.x + hit.collider.bounds.extents.x))
            {
                //Debug.Log("HitRight");

                results[1] = new Vector2(transform.position.x + hitRight.distance - (rend.size.x / 2), results[0].y);
            }

            
        }
        else
        {
            body.isKinematic = false;
            isGrounded = false;
        }
        //if (hit)
        //{
            //Debug.Log(hit.transform.position.x + " - " + hit.collider.bounds.extents.x);
            //for(int i = 0; i != results.Length; ++i)
            //{
            //    Debug.Log(results[i].x + " " + results[i].y);
            //}
        //}

        return results;
    }

    private void Walk(Vector2[] points)
    {
        MoveSlime(points);
        UpdateDirection(points);
        FlipSprite();
    }

    private void MoveSlime(Vector2[] points)
    {
        if (isDead == false)
        {
            if (direction == 1)
            {
                transform.position = Vector2.MoveTowards(transform.position, points[1], speed * Time.deltaTime);
            }
            else if (direction == -1)
            {
                transform.position = Vector2.MoveTowards(transform.position, points[0], speed * Time.deltaTime);
            }
        }
    }

    //::::::::::::::POINTWALK::::::::::::::::://

    //NOTE:::: 
    //change this to utilize two x values, and adjust rotation and y coordinate using raycast
    //whole new function not using MoveSlime()
    public void PointWalk()
    {
        //Debug.Log("PointWalk being called");
        //
        Walk(walkPoints);
    }

    //::::::::::::::NOWALK:::::::::::::::::::://

    private void NoWalk()
    {
        transform.position = transform.position;
    }
    
}
