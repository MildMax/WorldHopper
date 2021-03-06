﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadyBugScript : EnemyBase
{
    [HideInInspector]
    public int walkIndex;

    [HideInInspector]
    public int worldNum;

    public delegate void WalkMethod();
    WalkMethod walkMethod;

    public int speed;

    int oldIndex = 4;
    public int direction = 1;
    int startHealth;

    SpriteRenderer rend;
    CapsuleCollider2D coll;
    Animator anim;

    public Vector2[] walkPoints;
    Vector2[] autoWalkPoints = null;
    //so snail can traverse plains, but stops at a point so as not to just go anywhere
    public Vector2[] autoWalkLimits;

    Point destination;
    public int zRot;
    bool changeTraverse = false;

    PlayerController playerController;
    Transform playerTransform;
    WorldSwitcher wS;

    bool isDead = false;
    bool deathRoutine = false;

    [HideInInspector]
    public string groundLayer;

    [HideInInspector]
    public string wallLayer;

    //layers for masking
    int gL = 0;
    int wL = 0;
    int bL = 0;

    float oldHealth;

    //flying stuff
    bool isFlying = false;
    bool limitsFound = false;
    bool areaSet = false;
    bool flyTimerSet = false;
    bool returnToGround = false;
    bool snapshotTaken = false;

    public float groundTimeMin;
    public float groundTimeMax;
    public float flyTimeMin;
    public float flyTimeMax;
    public float waitMin;
    public float waitMax;
    public float xDistance;
    public float yDistance;
    FlyLimits limits = new FlyLimits();
    Vector2 newPos;
    Vector2 originalPos;

    float timer = 0;
    float waitTime;
    float flyWait;
    float flyTimer = 0;
    float groundWait;

    Vector2 snapPos;
    float snapRot;

    //death stuff
    public AnimationClip deathClip;
    float dWait;
    float dTimer = 0;
    bool deathSet = false;

    //fall stuff
    RaycastHit2D fallDistance;
    Vector2 vel;
    Vector2 dest;
    bool pointSet = false;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        anim = GetComponent<Animator>();
        groundLayer = "Ground" + (worldNum + 1);
        wallLayer = "Wall" + (worldNum + 1);
        SetMultipleLayers();
        startHealth = health;
        oldHealth = health;
        dWait = deathClip.length * 3;
    }

    private void FixedUpdate()
    {
        if (isDead == false && isFlying == false && returnToGround == false)
        {
            //anim.SetBool("IsNear", false);
            if (walkMethod != null)
            {
                walkMethod();
            }
        }
        else if (isDead == false && isFlying == true)
        {
            TakeSnapshot();
            Fly();
        }
        else if (isDead == false && returnToGround == true)
        {
            ReturnToGround();
        }
        else if (isDead == true && isFlying == true)
        {
            Fall();
        }
    }

    // Update is called once per frame
    void Update()
    {
        FlyTimer();
        CheckHealth();
        FlipSprites();
        SetWalkMethod();

        
    }

    private void LateUpdate()
    {
        if(isFlying == true || returnToGround == true)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else if (walkMethod == TraverseWalk)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, zRot);
        }
    }

    //::::::::::::GENERIC::::::::::::://

    private void SetWalkMethod()
    {
        if (walkIndex != oldIndex)
        {
            switch (walkIndex)
            {
                case 0:
                    walkMethod = AutoWalk;
                    return;
                case 1:
                    walkMethod = PointWalk;
                    return;
                case 2:
                    walkMethod = TraverseWalk;
                    return;
            }
            oldIndex = walkIndex;
        }
    }

    private void ChangeDirection(Vector2[] points)
    {
        if ((Vector2)transform.position == points[0])
        {
            direction = 1;
        }
        else if ((Vector2)transform.position == points[1])
        {
            direction = -1;
        }
    }

    private void FlipSprites()
    {
        if (direction == 1)
        {
            rend.flipX = true;
        }
        else if (direction == -1)
        {
            rend.flipX = false;
        }
    }

    private void MoveBug(Vector2[] points)
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

    private void CheckHealth()
    {
        if (health <= 0)
        {
            if(deathSet == false)
            {
                fallDistance = Physics2D.Raycast(transform.position, -Vector2.up, 150, bL);
                anim.SetBool("IsDead", true);
                isDead = true;
                rend.flipY = true;
                coll.enabled = false;
                deathSet = true;

            }
            //wS.DestroyEnemyValue(hash);
            if(dTimer > dWait)
            {
                wS.DestroyEnemyValue(hash);
                Destroy(gameObject);
            }

            dTimer += Time.deltaTime;
        }
        else if(oldHealth > health)
        {
            anim.SetTrigger("IsHurt");

            oldHealth = health;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerController.GetHurt(coll.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Shot")
        {
            health -= collision.gameObject.GetComponent<ShotScript>().damage;
        }
    }

    private void SetMultipleLayers()
    {
        //Debug.Log(groundLayer);
        //Debug.Log(wallLayer);

        gL = 1 << LayerMask.NameToLayer(groundLayer);
        wL = 1 << LayerMask.NameToLayer(wallLayer);
        bL = gL | wL;

        //Debug.Log("gL: " + gL);
        //Debug.Log("wL: " + wL);
        //Debug.Log("bL: " + bL);
    }

    //:::::::::::TRAVERSE:::::::::::::://

    private void TraverseWalk()
    {
        if (destination == null)
        {
            //Debug.Log("initial set");
            SetHeight();
            GetDirectionWalk();
        }
        else if (changeTraverse == true && (Vector2)transform.position == destination.point)
        {
            //Debug.Log("changeTraverse being used");
            if (direction > 0)
            {
                direction = -1;
            }
            else if (direction < 0)
            {
                direction = 1;
            }
            destination = null;
            changeTraverse = false;
        }
        else if ((Vector2)transform.position == destination.point)
        {
            //Debug.Log("Subsequent set");
            if (destination.wall == false)
            {
                AdjustForCorner();
            }

            ChangeRotation(destination.wall);
            SetHeight();
            GetDirectionWalk();
        }
        //Debug.Log(destination.point.x + " " + destination.point.y);

        MoveSnailT();

    }

    private void MoveSnailT()
    {
        if (destination != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, destination.point, speed * Time.deltaTime);
        }
    }

    private void AdjustForCorner()
    {
        if (direction > 0)
        {
            if (zRot == 0)
            {
                transform.position = new Vector2(transform.position.x + (coll.size.y / 2), transform.position.y - (coll.size.x / 2));
            }
            else if (zRot == 90 || zRot == -270)
            {
                transform.position = new Vector2(transform.position.x + (coll.size.x / 2), transform.position.y + (coll.size.y / 2));
            }
            else if (zRot == 180 || zRot == -180)
            {
                transform.position = new Vector2(transform.position.x - (coll.size.y / 2), transform.position.y + (coll.size.x / 2));
            }
            else if (zRot == -90 || zRot == 270)
            {
                transform.position = new Vector2(transform.position.x - (coll.size.x / 2), transform.position.y - (coll.size.y / 2));
            }
        }
        else if (direction < 0)
        {
            if (zRot == 0)
            {
                transform.position = new Vector2(transform.position.x - (coll.size.y / 2), transform.position.y - (coll.size.x / 2));
            }
            else if (zRot == -90 || zRot == 270)
            {
                transform.position = new Vector2(transform.position.x - (coll.size.x / 2), transform.position.y + (coll.size.y / 2));
            }
            else if (zRot == 180 || zRot == -180)
            {
                transform.position = new Vector2(transform.position.x + (coll.size.y / 2), transform.position.y + (coll.size.x / 2));
            }
            else if (zRot == 90 || zRot == -270)
            {
                transform.position = new Vector2(transform.position.x + (coll.size.y / 2), transform.position.y - (coll.size.y / 2));
            }
        }
    }

    private void ChangeRotation(bool newWall)
    {
        //Debug.Log("rotation being changed");
        if (direction < 0)
        {
            if (newWall == false)
            {
                zRot += 90;
            }
            else if (newWall == true)
            {
                zRot -= 90;
            }

            if (zRot <= -360f || zRot >= 360f)
            {
                zRot = 0;
            }
        }
        else if (direction > 0)
        {
            if (newWall == false)
            {
                zRot -= 90;
            }
            else if (newWall == true)
            {
                zRot += 90;
            }

            if (zRot >= 360f || zRot <= -360f)
            {
                zRot = 0;
            }
        }
    }

    private void SetHeight()
    {
        Quaternion q = Quaternion.AngleAxis(zRot, Vector3.forward);
        Vector2 dir = q * -Vector2.up;

        RaycastHit2D hit = Physics2D.Raycast(coll.transform.position, dir, (coll.size.y / 2) + 0.5f, LayerMask.GetMask(groundLayer));
        //on top of the ground
        if (hit)
        {
            if (zRot == 0)
            {
                //Debug.Log("Calling above ground height set");
                transform.position = new Vector2(transform.position.x, hit.collider.transform.position.y + (hit.collider.bounds.size.y / 2) + (coll.size.y / 2));
            }
            //left of the ground
            else if (zRot == 90 || zRot == -270)
            {
                //Debug.Log("Calling left of ground height set");
                transform.position = new Vector2(hit.collider.transform.position.x - (hit.collider.bounds.size.x / 2) - (coll.size.y / 2), transform.position.y);
            }
            //under the ground
            else if (zRot == 180 || zRot == -180)
            {
                //Debug.Log("Calling under the ground height set");
                transform.position = new Vector2(transform.position.x, hit.collider.transform.position.y - (hit.collider.bounds.size.y / 2) - (coll.size.y / 2));
            }
            //right of the ground
            else if (zRot == 270 || zRot == -90)
            {
                //Debug.Log("Calling right of the ground height set");
                transform.position = new Vector2(hit.collider.transform.position.x + (hit.collider.bounds.size.x / 2) + (coll.size.y / 2), transform.position.y);
            }
        }

    }

    private void GetDirectionWalk()
    {

        Quaternion upAffector = Quaternion.AngleAxis(zRot, Vector3.forward);
        Vector2 upDir = upAffector * -Vector2.up;

        RaycastHit2D ground = Physics2D.Raycast(coll.transform.position, upDir, (coll.size.y / 2) + 0.5f, LayerMask.GetMask(groundLayer));

        float dist = new float();

        if (ground)
        {
            if (direction > 0)
            {
                if (zRot == 0)
                {
                    dist = ground.transform.position.x + (ground.collider.bounds.size.x / 2) - ground.point.x;
                }
                else if (zRot == 90 || zRot == -270)
                {
                    dist = ground.transform.position.y + (ground.collider.bounds.size.y / 2) - ground.point.y;
                }
                else if (zRot == 180 || zRot == -180)
                {
                    dist = ground.point.x - ground.transform.position.x + (ground.collider.bounds.size.x / 2);
                }
                else if (zRot == -90 || zRot == 270)
                {
                    dist = ground.point.y - ground.transform.position.y + (ground.collider.bounds.size.y / 2);
                }
            }
            else if (direction < 0)
            {
                if (zRot == 0)
                {
                    dist = ground.point.x - ground.transform.position.x + (ground.collider.bounds.size.x / 2);
                }
                else if (zRot == 90 || zRot == -270)
                {
                    dist = ground.point.y - ground.transform.position.y + (ground.collider.bounds.size.y / 2);
                }
                else if (zRot == 180 || zRot == -180)
                {
                    dist = ground.transform.position.x + (ground.collider.bounds.size.x / 2) - ground.point.x;
                }
                else if (zRot == -90 || zRot == 270)
                {
                    dist = ground.transform.position.y + (ground.collider.bounds.size.y / 2) - ground.point.y;
                }
            }
        }

        Vector2 dir = Vector2.right;

        if (direction < 0)
        {
            dir *= -1;
        }

        Quaternion rightDir = Quaternion.AngleAxis(zRot, Vector3.forward);
        dir = rightDir * dir;

        //Debug.DrawRay(coll.transform.position, dir);
        //Debug.DrawRay(coll.transform.position, upDir);

        RaycastHit2D hit = Physics2D.Raycast(coll.transform.position, dir, dist + 0.1f, LayerMask.GetMask(groundLayer));

        //if shot from transform.position, this collides with the wall it just turned around from, so the offset
        //puts the origin at the front of the sprite depending on direction to avoid this collision with the wall
        //the snail just turned away from
        float offset = 0;

        if (direction > 0)
        {
            offset += (coll.size.x / 2);
        }
        else if (direction < 0)
        {
            offset -= (coll.size.x / 2);
        }

        Vector2 newPos = new Vector2(coll.transform.position.x + offset, coll.transform.position.y);
        RaycastHit2D hitWall = Physics2D.Raycast(newPos, dir, dist + 0.1f, LayerMask.GetMask(wallLayer));

        if (hitWall && !hit)
        {
            //Debug.Log(hitW.collider.gameObject.name);
            changeTraverse = true;
        }

        if (hit)
        {
            destination = new Point(hit.point, true);
            //Debug.Log("wall set");
        }
        else
        {
            if (ground)
            {
                Vector2 pos = new Vector2();

                if (direction > 0)
                {
                    if (zRot == 0)
                    {
                        //Debug.Log("setting above ground pos");
                        pos = new Vector2(ground.collider.transform.position.x + (ground.collider.bounds.size.x / 2), transform.position.y);
                    }
                    else if (zRot == 90 || zRot == -270)
                    {
                        //Debug.Log("Setting left of ground pos");
                        pos = new Vector2(transform.position.x, ground.collider.transform.position.y + (ground.collider.bounds.size.y / 2));
                    }
                    else if (zRot == 180 || zRot == -180)
                    {
                        //Debug.Log("Setting under ground pos");
                        pos = new Vector2(ground.collider.transform.position.x - (ground.collider.bounds.size.x / 2), transform.position.y);
                    }
                    else if (zRot == 270 || zRot == -90)
                    {
                        //Debug.Log("Setting right of ground pos");
                        pos = new Vector2(transform.position.x, ground.collider.transform.position.y - (ground.collider.bounds.size.y / 2));
                    }
                }
                else if (direction < 0)
                {
                    if (zRot == 0)
                    {
                        pos = new Vector2(ground.collider.transform.position.x - (ground.collider.bounds.size.x / 2), transform.position.y);
                    }
                    else if (zRot == 90 || zRot == -270)
                    {
                        pos = new Vector2(transform.position.x, ground.collider.transform.position.y - (ground.collider.bounds.size.y / 2));
                    }
                    else if (zRot == 180 || zRot == -180)
                    {
                        pos = new Vector2(ground.collider.transform.position.x + (ground.collider.bounds.size.x / 2), transform.position.y);
                    }
                    else if (zRot == 270 || zRot == -90)
                    {
                        pos = new Vector2(transform.position.x, ground.collider.transform.position.y + (ground.collider.bounds.size.y / 2));
                    }
                }
                //Debug.Log("ground set");
                destination = new Point(pos, false);
            }
        }
    }

    //:::::::::::AUTOWALK:::::::::::::://

    private void AutoWalk()
    {
        if (autoWalkPoints == null)
        {
            autoWalkPoints = GetWalkInfo();
        }
        else
        {
            MoveBug(autoWalkPoints);
            ChangeDirection(autoWalkPoints);
        }
    }

    private Vector2[] GetWalkInfo()
    {
        //Debug.Log("Getting walk info");

        Vector2[] points = new Vector2[2];

        RaycastInfo ground = DoRaycast(-Vector2.up, (coll.size.y / 2) + 0.1f);

        if (ground != null)
        {
            for (int i = 0; i != points.Length; ++i)
            {
                points[i] = new Vector2(0f, ground.position.y + (ground.sizeY / 2) + (coll.size.y / 2));
            }

            RaycastInfo left = DoRaycast(-Vector2.right, Mathf.Abs((ground.point.x - ground.position.x) + (ground.sizeX / 2)));

            if (left != null)
            {
                //Debug.Log("Left boundary hit");
                points[0] = new Vector2(left.position.x + (left.sizeX / 2) + (coll.size.x / 2), points[0].y);
            }
            else
            {
                points[0] = new Vector2(ground.position.x - (ground.sizeX / 2) + (coll.size.x / 2), points[0].y);
            }

            RaycastInfo right = DoRaycast(Vector2.right, Mathf.Abs((ground.position.x - ground.point.x) + (ground.sizeX / 2)));

            if (right != null)
            {
                //Debug.Log("Right Boundary hit");
                points[1] = new Vector2(right.position.x - (right.sizeX / 2) - (coll.size.x / 2), points[0].y);
            }
            else
            {
                points[1] = new Vector2(ground.position.x + (ground.sizeX / 2) - (coll.size.x / 2), points[0].y);
            }

            for (int i = 0; i != points.Length; ++i)
            {
                //Debug.Log(i + ": x = " + points[i].x + ", y = " + points[i].y);
            }

            return points;
        }
        else
        {
            return null;
        }
    }

    private RaycastInfo DoRaycast(Vector2 dir, float dist)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, LayerMask.GetMask(groundLayer));
        //Debug.Log(hit.)
        if (hit)
        {
            RaycastInfo info = new RaycastInfo(hit.collider.transform.position, hit.point, hit.collider.bounds.size.x, hit.collider.bounds.size.y);
            return info;
        }
        else
        {
            return null;
        }
    }

    private RaycastInfo DoRaycast(Vector2 dir, float dist, int layer)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, layer);
        //Debug.Log(hit.)
        if (hit)
        {
            RaycastInfo info = new RaycastInfo(hit.collider.transform.position, hit.point, hit.collider.bounds.size.x, hit.collider.bounds.size.y);
            return info;
        }
        else
        {
            return null;
        }
    }

    //:::::::::::POINTWALK:::::::::::::://

    private void PointWalk()
    {
        MoveBug(walkPoints);
        ChangeDirection(walkPoints);
    }

    //::::::::::::FLYING::::::::::::::://

    private void FlyTimer()
    {
        if (returnToGround == false)
        {
            if (flyTimerSet == false)
            {

                groundWait = Random.Range(groundTimeMin, groundTimeMax);
                flyWait = Random.Range(flyTimeMin, flyTimeMax) + groundWait;
                flyTimerSet = true;
            }

            if (flyTimer <= groundWait)
            {
                isFlying = false;
            }
            else if (flyTimer > groundWait && flyTimer < flyWait)
            {
                anim.SetBool("IsFlying", true);
                isFlying = true;
            }
            else if (flyTimer >= flyWait)
            {
                anim.SetBool("IsFlying", false);
                returnToGround = true;
                isFlying = false;
                flyTimer = 0;
                flyTimerSet = false;
                limitsFound = false;
                snapshotTaken = false;
                areaSet = false;
            }

            flyTimer += Time.deltaTime;
        }
    }

    private void Fly()
    {
        if (limitsFound == false)
        {
            limits = FindArea();
            limits.Organize();
            //Debug.Log("xMin: " + limits.xMin);
            //Debug.Log("xMax: " + limits.xMax);
            //Debug.Log("yMin: " + limits.yMin);
            //Debug.Log("yMax: " + limits.yMax);
        }


        if(areaSet == false)
        {
            SetArea();
        }

        if (timer < waitTime)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if ((Vector2)transform.position != newPos)
            {
                transform.position = Vector2.MoveTowards(transform.position, newPos, speed * Time.deltaTime);
            }
            else
            {
                areaSet = false;
            }
        }
    }

    private void SetArea()
    {
        originalPos = newPos;

        if (Mathf.Abs(zRot) == 0 || Mathf.Abs(zRot) == 180)
        {
            newPos = new Vector2(
                Random.Range(limits.xMin, limits.xMax),
                Random.Range(limits.yMin, limits.yMax)
                );
        }
        else if(Mathf.Abs(zRot) == 90 || Mathf.Abs(zRot) == 270)
        {
            newPos = new Vector2(
                Random.Range(limits.yMin, limits.yMax),
                Random.Range(limits.xMin, limits.xMax)
                );
        }

        //Debug.Log(newPos);

        waitTime = Random.Range(waitMin, waitMax);
        timer = 0;

        areaSet = true;
    }

    private void SetDirection()
    {
        if (newPos.x > originalPos.x)
        {
            rend.flipX = true;
        }
        else if (newPos.x < originalPos.x)
        {
            rend.flipX = false;
        }
        else if (newPos.x == originalPos.x)
        {
            rend.flipX = !rend.flipX;
        }
    }

    private FlyLimits FindArea()
    {
        FlyLimits lim = new FlyLimits();
        
        Quaternion q = Quaternion.AngleAxis(zRot, Vector3.forward);

        RaycastInfo r = DoRaycast(q * Vector2.right, xDistance, bL);

        Debug.DrawRay(transform.position, q * Vector2.right, Color.red);

        if(r != null)
        {
            if (Mathf.Abs(zRot) == 0 || Mathf.Abs(zRot) == 180)
            {
                lim.xMax = r.point.x;
            }
            else if(Mathf.Abs(zRot) == 90 || Mathf.Abs(zRot) == 270)
            {
                lim.xMax = r.point.y;
            }
        }
        else
        {
            if (Mathf.Abs(zRot) == 0 || Mathf.Abs(zRot) == 180)
            {
                lim.xMax = transform.position.x + xDistance;
            }
            else if (Mathf.Abs(zRot) == 90 || Mathf.Abs(zRot) == 270)
            {
                lim.xMax = lim.xMax = transform.position.y + xDistance;
            }
            
        }

        RaycastInfo l = DoRaycast(q * Vector2.left, xDistance, bL);

        Debug.DrawRay(transform.position, q * Vector2.left, Color.red);

        if (l != null)
        {
            if (Mathf.Abs(zRot) == 0 || Mathf.Abs(zRot) == 180)
            {
                lim.xMin = l.point.x;
            }
            else if (Mathf.Abs(zRot) == 90 || Mathf.Abs(zRot) == 270)
            {
                lim.xMin = l.point.y;
            }
        }
        else
        {
            if (Mathf.Abs(zRot) == 0 || Mathf.Abs(zRot) == 180)
            {
                lim.xMin = transform.position.x - xDistance;
            }
            else if (Mathf.Abs(zRot) == 90 || Mathf.Abs(zRot) == 270)
            {
                lim.xMin = transform.position.y - xDistance;
            }
            
        }

        
        
        if(zRot == 0)
        {
            lim.yMin = transform.position.y + (coll.size.y / 2);
        }
        else if(zRot == 180 || zRot == -180)
        {
            lim.yMin = transform.position.y - (coll.size.y / 2);
        }
        else if(zRot == 90 || zRot == -270)
        {
            lim.yMin = transform.position.x - (coll.size.y / 2);
        }
        else if(zRot == 270 || zRot == -90)
        {
            lim.yMin = transform.position.x + (coll.size.y / 2);
        }

        lim.yMax = FindYLimits(lim.xMin, lim.xMax, transform.position, q);

        return lim;
    }

    private float FindYLimits(float min, float max, Vector2 pos, Quaternion q)
    {
        if(min > max)
        {
            float temp = max;
            max = min;
            min = temp;
        }

        float height = yDistance;

        float length = Mathf.Abs(max - min) / 0.7f;
        //Debug.Log("Length: " + length);

        bool newHeightSet = false;

        if (Mathf.Abs(zRot) == 0 || Mathf.Abs(zRot) == 180)
        {
            for(int i = 1; i != (int)length; ++i)
            {
                Vector2 localPos = new Vector2((min) + (i * 0.7f), pos.y);

                //Debug.Log("Drawing line " + i);
                //Debug.DrawLine(localPos, new Vector2(localPos.x, localPos.y + yDistance), Color.red);
                Debug.DrawRay(localPos, q * Vector2.up, Color.red);
                //Debug.Log(q * Vector2.up);
                RaycastHit2D hit = Physics2D.Raycast(localPos, q * Vector2.up, yDistance, bL);

                if(hit)
                {
                    newHeightSet = true;

                    //Debug.Log("Hit! - " + hit.point.y);
                    float yLim = Mathf.Abs(pos.y - hit.point.y);
                    
                    if(yLim < height)
                    {
                        if (Mathf.Abs(zRot) == 0)
                        {
                            height = pos.y + yLim;
                        }
                        else if(Mathf.Abs(zRot) == 180)
                        {
                            height = pos.y - yLim;
                        }
                    }
                }
            }

            if(newHeightSet == false)
            {
                if (Mathf.Abs(zRot) == 0)
                {
                    height = pos.y + yDistance;
                }
                else if (Mathf.Abs(zRot) == 180)
                {
                    height = pos.y - yDistance;
                }
            }
        }
        else if (Mathf.Abs(zRot) == 90 || Mathf.Abs(zRot) == 270)
        {
            for (int i = 1; i != (int)length; ++i)
            {
                Vector2 localPos = new Vector2(pos.x, (min) + (i * 0.7f));

                //Debug.DrawLine(localPos, new Vector2(localPos.x + yDistance, localPos.y), Color.red);
                Debug.DrawRay(localPos, q * Vector2.up, Color.red);
                RaycastHit2D hit = Physics2D.Raycast(localPos, q * Vector2.up, yDistance, bL);

                if (hit)
                {
                    //Debug.Log("hit " + hit.collider.gameObject.name + " at point (" + hit.point.x + ", " + hit.point.y + ")");
                    float xLim = Mathf.Abs(pos.x - hit.point.x);

                    if (xLim < height)
                    {
                        if(zRot == 270 || zRot == -90)
                        {
                            height = pos.x + xLim;
                        }
                        else if(zRot == -270 || zRot == 90)
                        {
                            height = pos.x - xLim;
                        }
                    }
                }
            }

            if(newHeightSet == false)
            {
                if (zRot == 270 || zRot == -90)
                {
                    height = pos.x + yDistance;
                }
                else if (zRot == -270 || zRot == 90)
                {
                    height = pos.x - yDistance;
                }
            }
        }

        limitsFound = true;

        return height;
    }

    private void ReturnToGround()
    {
        if ((Vector2)transform.position != snapPos)
        {
            transform.position = Vector2.MoveTowards(transform.position, snapPos, speed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 0f, snapRot);
            returnToGround = false;
        }
    }

    private void TakeSnapshot()
    {
        if (snapshotTaken == false)
        {
            //Debug.Log("Snapshot taken");
            snapPos = transform.position;
            snapRot = zRot;
            snapshotTaken = true;
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
                dest = new Vector2(fallDistance.point.x, fallDistance.point.y + (coll.size.y / 2));
                pointSet = true;
            }

            if (transform.position.y >= dest.y)
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

public class FlyLimits
{
    public float xMin = 0;
    public float xMax = 0;
    public float yMax = 0;
    public float yMin = 0;

    public void Organize()
    {
        if(xMin > xMax)
        {
            float temp = xMin;
            xMin = xMax;
            xMax = temp;
        }

        if(yMin > yMax)
        {
            float temp = yMin;
            yMin = yMax;
            yMax = temp;
        }
    }
}
