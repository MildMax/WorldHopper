using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailScript : EnemyBase
{
    [HideInInspector]
    public int walkIndex;

    [HideInInspector]
    public int worldNum;

    public delegate void WalkMethod();
    WalkMethod walkMethod;

    public int speed;
    public float dangerClose;
    public float deathWait;

    int oldIndex = 4;
    public int direction = 1;
    int startHealth;

    SpriteRenderer rend;
    CapsuleCollider2D coll;
    Animator anim;

    public Vector2[] walkPoints;
    Vector2[] autoWalkPoints = null;
    //so snail can traverse plains, but stops at a point so as not to just fuck off anywhere it can go
    public Vector2[] autoWalkLimits;

    Point destination;
    int zRot = 0;
    public bool changeTraverse = false;

    PlayerController playerController;
    Transform playerTransform;
    WorldSwitcher wS;

    bool withinDistance = false;
    bool isDead = false;
    bool deathRoutine = false;

    [HideInInspector]
    public string groundLayer;

    [HideInInspector]
    public string wallLayer;

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
        startHealth = health;
    }

    private void FixedUpdate()
    {
        
        CheckDistance();
    }

    // Update is called once per frame
    void Update()
    {
        CheckHealth();
        FlipSprites();
        SetWalkMethod();
        
        if (withinDistance == false && isDead == false)
        {
            anim.SetBool("IsNear", false);
            walkMethod();
        }
        else if (withinDistance == true && isDead == false)
        {
            anim.SetBool("IsNear", true);
        }
        else if(isDead == true && deathRoutine == false)
        {
            StartCoroutine(DeathRoutine());
        }
    }

    private void LateUpdate()
    {
        if(walkMethod == TraverseWalk)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, zRot);
        }
    }

    //::::::::::::GENERIC::::::::::::://

    private void SetWalkMethod()
    {
        if(walkIndex != oldIndex)
        {
            switch(walkIndex)
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
        if(direction == 1)
        {
            rend.flipX = true;
        }
        else if(direction == -1)
        {
            rend.flipX = false;
        }
    }

    private void MoveSnail(Vector2[] points)
    {
        if(direction == 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, points[1], speed * Time.deltaTime);
        }
        else if(direction == -1)
        {
            transform.position = Vector2.MoveTowards(transform.position, points[0], speed * Time.deltaTime);
        }
    }

    private void CheckDistance()
    {
        Vector2 offset = transform.position - playerTransform.position;
        float magnitude = Mathf.Sqrt(Mathf.Pow(offset.x, 2) + Mathf.Pow(offset.y, 2));

        if(magnitude < dangerClose)
        {
            withinDistance = true;
        }
        else
        {
            withinDistance = false;
        }
    }

    private void CheckHealth()
    {
        if(health <= 0)
        {
            //wS.DestroyEnemyValue(hash);
            isDead = true;
        }
    }

    private IEnumerator DeathRoutine()
    {
        anim.SetBool("IsHurt", true);
        coll.enabled = false;
        deathRoutine = true;

        yield return new WaitForSeconds(deathWait);

        anim.SetBool("IsHurt", false);
        coll.enabled = true;
        isDead = false;
        deathRoutine = false;
        health = startHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            playerController.GetHurt(coll.transform.position);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Shot")
        {
            health -= collision.gameObject.GetComponent<ShotScript>().damage;
        }
    }

    //:::::::::::TRAVERSE:::::::::::::://

    private void TraverseWalk()
    {
        if(destination == null)
        {
            //Debug.Log("initial set");
            SetHeight();
            GetDirectionWalk();
        }
        else if(changeTraverse == true && (Vector2)transform.position == destination.point)
        {
            //Debug.Log("changeTraverse being used");
            if(direction > 0)
            {
                direction = -1;
            }
            else if(direction < 0)
            {
                direction = 1;
            }
            destination = null;
            changeTraverse = false;
        }
        else if((Vector2)transform.position == destination.point)
        {
            //Debug.Log("Subsequent set");
            if(destination.wall == false)
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
        if(direction > 0)
        {
            if(zRot == 0)
            {
                transform.position = new Vector2(transform.position.x + (coll.size.y / 2), transform.position.y - (coll.size.x / 2));
            }
            else if(zRot == 90 || zRot == -270)
            {
                transform.position = new Vector2(transform.position.x + (coll.size.x / 2), transform.position.y + (coll.size.y / 2));
            }
            else if(zRot == 180 || zRot == -180)
            {
                transform.position = new Vector2(transform.position.x - (coll.size.y / 2), transform.position.y + (coll.size.x / 2));
            }
            else if(zRot == -90 || zRot == 270)
            {
                transform.position = new Vector2(transform.position.x - (coll.size.x / 2), transform.position.y - (coll.size.y / 2));
            }
        }
        else if(direction < 0)
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
        else if(direction > 0)
        {
            if (newWall == false)
            {
                zRot -= 90;
            }
            else if(newWall == true)
            {
                zRot += 90;
            }

            if(zRot >= 360f || zRot <= -360f)
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

        if(ground)
        {
            if(direction > 0)
            {
                if (zRot == 0)
                {
                    dist = ground.transform.position.x + (ground.collider.bounds.size.x / 2) - ground.point.x;
                }
                else if(zRot == 90 || zRot == -270)
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
            else if(direction < 0)
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

        Debug.DrawRay(coll.transform.position, dir);
        Debug.DrawRay(coll.transform.position, upDir);

        RaycastHit2D hit = Physics2D.Raycast(coll.transform.position, dir, dist + 0.1f, LayerMask.GetMask(groundLayer));

        //if shot from transform.position, this collides with the wall it just turned around from, so the offset
        //puts the origin at the front of the sprite depending on direction to avoid this collision with the wall
        //the snail just turned away from
        float offset = 0;

        if(direction > 0)
        {
            offset += (coll.size.x / 2);
        }
        else if(direction < 0)
        {
            offset -= (coll.size.x / 2);
        }

        Vector2 newPos = new Vector2(coll.transform.position.x + offset, coll.transform.position.y);
        RaycastHit2D hitWall = Physics2D.Raycast(newPos, dir, dist + 0.1f, LayerMask.GetMask(wallLayer));

        if(hitWall && !hit)
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
            if(ground)
            {
                Vector2 pos = new Vector2();

                if(direction > 0)
                {
                    if(zRot == 0)
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
                else if(direction < 0)
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
            MoveSnail(autoWalkPoints);
            ChangeDirection(autoWalkPoints);
        }
    }

    private Vector2[] GetWalkInfo()
    {
        //Debug.Log("Getting walk info");

        Vector2[] points = new Vector2[2];

        RaycastInfo ground = DoRaycast(-Vector2.up, (coll.size.y / 2) + 0.1f);

        if(ground != null)
        {
            for(int i = 0; i != points.Length; ++i)
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

    //:::::::::::POINTWALK:::::::::::::://

    private void PointWalk()
    {
        MoveSnail(walkPoints);
        ChangeDirection(walkPoints);
    }

    
}

public class RaycastInfo
{
    public Vector3 position;
    public Vector3 point;
    public float sizeX;
    public float sizeY;

    public RaycastInfo(Vector3 pos, Vector3 p, float x, float y)
    {
        position = pos;
        point = p;
        sizeX = x;
        sizeY = y;
    }
}

public class Point
{
    public Vector2 point;
    public bool wall = false;

    public Point(Vector2 p, bool w)
    {
        point = p;
        wall = w;
    }
}
