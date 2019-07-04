using System.Collections;
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

    bool isDead = false;
    bool deathRoutine = false;

    [HideInInspector]
    public string groundLayer;

    [HideInInspector]
    public string wallLayer;

    //layers for masking
    int gL;
    int wL;
    int bL;

    float oldHealth;

    //flying stuff
    bool isFlying = false;
    bool limitsFound = false;
    bool areaSet = false;
    bool flyTimerSet = false;

    public float groundTimeMin;
    public float groundTimeMax;
    public float flyTimeMin;
    public float flyTimeMax;
    public float waitMin;
    public float waitMax;
    public float xDistance;
    public float yDistance;
    public int yDivisions;
    FlyLimits limits = new FlyLimits();
    Vector2 newPos;
    Vector2 originalPos;

    float timer = 0;
    float waitTime;
    float flyWait;
    float flyTimer = 0;
    float groundWait;

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
    }

    private void FixedUpdate()
    {
    }

    // Update is called once per frame
    void Update()
    {
        FlyTimer();
        CheckHealth();
        FlipSprites();
        SetWalkMethod();

        if (isDead == false && isFlying == false)
        {
            //anim.SetBool("IsNear", false);
            walkMethod();
        }
        else if(isDead == false && isFlying == true)
        {
            Fly();
        }
        else if (isDead == true)
        {
            //Fall if in air
        }
    }

    private void LateUpdate()
    {
        if (walkMethod == TraverseWalk)
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
            //wS.DestroyEnemyValue(hash);
            isDead = true;
        }
        else if(oldHealth > health)
        {
            //set anim trigger

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
        gL = 1 << LayerMask.NameToLayer(groundLayer);
        wL = 1 << LayerMask.NameToLayer(wallLayer);
        bL = gL | wL;
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

        Debug.DrawRay(coll.transform.position, dir);
        Debug.DrawRay(coll.transform.position, upDir);

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

    //:::::::::::POINTWALK:::::::::::::://

    private void PointWalk()
    {
        MoveBug(walkPoints);
        ChangeDirection(walkPoints);
    }

    //::::::::::::FLYING::::::::::::::://

    private void FlyTimer()
    {
        if(flyTimerSet == false)
        {
            
            groundWait = Random.Range(groundTimeMin, groundTimeMax);
            flyWait = Random.Range(flyTimeMin, flyTimeMax) + groundWait;
            flyTimerSet = true;
        }

        if(flyTimer <= groundWait)
        {
            isFlying = false;
        }
        if(flyTimer > groundWait && flyTimer < flyWait)
        {
            isFlying = true;
        }
        else if(flyTimer >= flyWait)
        {
            isFlying = false;
            flyTimer = 0;
            flyTimerSet = false;
        }

        flyTimer += Time.deltaTime;
    }

    private void Fly()
    {
        if (limitsFound == false)
        {
            limits = FindArea();
            Debug.Log("xMin: " + limits.xMin);
            Debug.Log("xMax: " + limits.xMax);
            Debug.Log("yMin: " + limits.yMin);
            Debug.Log("yMax: " + limits.yMax);
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

        newPos = new Vector2(
            Random.Range(limits.xMin, limits.xMax),
            Random.Range(limits.yMin, limits.yMax)
            );

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

        RaycastInfo r = DoRaycast(Vector2.right, xDistance);
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
            lim.xMax = xDistance;
        }

        RaycastInfo l = DoRaycast(Vector2.left, xDistance);
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
            lim.xMin = xDistance;
        }

        
        
        if(zRot == 0)
        {
            lim.yMin = transform.position.y + (coll.size.y / 2);
            lim.yMax = FindYLimits(lim.xMin, lim.xMax, transform.position);
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

        return lim;
    }

    private float FindYLimits(float min, float max, Vector2 pos)
    {
        if(min > max)
        {
            float temp = max;
            max = min;
            min = temp;
        }

        float height = yDistance;

        float length = Mathf.Abs(max - min) / yDivisions;

        if (Mathf.Abs(zRot) == 0 || Mathf.Abs(zRot) == 180)
        {
            for(int i = 0; i != yDivisions + 1; ++i)
            {
                Vector2 localPos = new Vector2((transform.position.x - min) + (i * length), transform.position.y);

                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.up, yDistance, bL);

                if(hit)
                {
                    float yLim = Mathf.Abs(transform.position.y - hit.point.y);
                    
                    if(yLim < height)
                    {
                        height = yLim;
                    }
                }
            }
        }
        else if (Mathf.Abs(zRot) == 90 || Mathf.Abs(zRot) == 270)
        {
            for (int i = 0; i != yDivisions + 1; ++i)
            {
                Vector2 localPos = new Vector2(transform.position.x, (transform.position.y - min) + (i * length));

                RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.up, yDistance, bL);

                if (hit)
                {
                    float xLim = Mathf.Abs(transform.position.x - hit.point.x);

                    if (xLim < height)
                    {
                        height = xLim;
                    }
                }
            }
        }

        limitsFound = true;

        return height;
    }
}

public class FlyLimits
{
    public float xMin = 0;
    public float xMax = 0;
    public float yMax = 0;
    public float yMin = 0;
}
