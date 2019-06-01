using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeScriptAlt : EnemyBase
{
    public int speed;
    public Vector2[] walkPoints;

    CapsuleCollider2D coll;
    SpriteRenderer rend;
    Rigidbody2D body;
    Vector2[] autoWalkPoints = new Vector2[3] { Vector2.positiveInfinity, Vector2.positiveInfinity, Vector2.positiveInfinity };
    Vector2[] autoWalkExtents = new Vector2[2];

    int direction = 1;
    bool isDead = false;

    //slope walk indicators
    bool slopeLeft = false;
    bool slopeRight = false;
    bool slopeWalk = false;

    //slope walk data
    bool slopeSet = false;
    Vector2 slopeDest;

    //serialize/gui shite
    public delegate void WalkMethod();
    public WalkMethod walkType;

    [SerializeField]
    [HideInInspector]
    public int walkIndex;

    private void Awake()
    {
        SetInitWalk();
        coll = GetComponent<CapsuleCollider2D>();
        rend = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        autoWalkPoints = SetAutoWalk();
    }

    private void FixedUpdate()
    {
        walkType();
    }

    //:::::::::::::::::GENERIC:::::::::::::::::://

    private void UpdateDirection(Vector2[] points)
    {
        if ((Vector2)transform.position == points[1])
        {
            if (slopeRight == true)
            {
                //only used in AutoWalk() methods
                slopeWalk = true;
            }
            else
            {
                //check for downward slope
                direction = -1;
                slopeWalk = false;
            }
        }
        else if ((Vector2)transform.position == points[0])
        {
            if (slopeLeft == true)
            {
                //only used in AutoWalk() methods
                slopeWalk = true;
            }
            else
            {
                //check for downward slope
                direction = 1;
                slopeWalk = false;
            }
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
            body.velocity = Vector2.zero;
            //Debug.Log("Walking");
            Walk(autoWalkPoints);
        }
        //Debug.Log("Autowalk being called");
        //
    }

    private Vector2[] SetAutoWalk()
    {
        Vector2[] results = new Vector2[3] { Vector2.positiveInfinity, Vector2.positiveInfinity, Vector2.positiveInfinity };

        RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, (rend.size.y / 2) + 0.25f, LayerMask.GetMask("Ground"));

        if (hit)
        {
            //Debug.Log(hit.transform.gameObject.name);

            results[0] = new Vector2(hit.transform.position.x - hit.collider.bounds.extents.x + (rend.size.x / 2),
                                        hit.transform.position.y + hit.collider.bounds.extents.y + (rend.size.y / 2));
            results[1] = new Vector2(hit.transform.position.x + hit.collider.bounds.extents.x - (rend.size.x / 2),
                                        hit.transform.position.y + hit.collider.bounds.extents.y + (rend.size.y / 2));
            results[2] = hit.transform.position;

            //CHECK THIS:::::::::::::::::::::
            float distanceL = (hit.point.x - hit.transform.position.x) + hit.collider.bounds.extents.x;

            //Debug.Log(hit.transform.gameObject.layer.ToString());

            RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, -transform.right, distanceL, LayerMask.GetMask("Ground"));

            if (hitLeft)
            {
                Debug.Log("HitLeft: " + hitLeft.transform.gameObject.name);
            }

            if (hitLeft && hitLeft.distance < Mathf.Abs(hit.transform.position.x - hit.collider.bounds.extents.x))
            {
                //Debug.Log("Hitleft");
                if (hitLeft.transform.gameObject.tag == "Slope")
                {
                    slopeLeft = true;
                }
                else
                {
                    slopeLeft = false;
                }

                results[0] = new Vector2(transform.position.x - hitLeft.distance + (rend.size.x / 2), results[0].y);
            }

            //CHECK THIS:::::::::::::::::::::::::
            float distanceR = (hit.transform.position.x - hit.point.x) + hit.collider.bounds.extents.x;

            RaycastHit2D hitRight = Physics2D.Raycast(transform.position, transform.right, distanceR, LayerMask.GetMask("Ground"));

            if (hitRight)
            {
                Debug.Log("HitRight: " + hitRight.transform.gameObject.name);
            }

            if (hitRight && hitRight.distance < Mathf.Abs(hit.transform.position.x + hit.collider.bounds.extents.x))
            {
                //Debug.Log("HitRight");

                if (hitRight.transform.gameObject.tag == "Slope")
                {
                    slopeRight = true;
                }
                else
                {
                    slopeRight = false;
                }

                results[1] = new Vector2(transform.position.x + hitRight.distance - (rend.size.x / 2), results[0].y);
            }

        }
        else
        {
            body.isKinematic = false;
        }
        if (hit)
        {
            //Debug.Log(hit.transform.position.x + " - " + hit.collider.bounds.extents.x);
            //for(int i = 0; i != results.Length; ++i)
            //{
            //    Debug.Log(results[i].x + " " + results[i].y);
            //}
        }

        return results;
    }

    private void SetSlopeWalk(int dir)
    {
        if (dir > 0 && slopeRight == true)
        {
            //Debug.Log("Setting slope right");

            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 0.7f, LayerMask.GetMask("Ground"));

            transform.rotation = Quaternion.Euler(0, 0, 45f);

            if (hit)
            {
                float distLen = Mathf.Sqrt(Mathf.Pow(hit.collider.bounds.size.x, 2) / 2) + 0.1f;

                slopeDest = new Vector2(transform.position.x + distLen, transform.position.y + distLen);

                slopeSet = true;

                Debug.Log(slopeDest.x + " " + slopeDest.y);
            }
        }
        else if (dir < 0 && slopeLeft == true)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.right, 0.7f, LayerMask.GetMask("Ground"));

            transform.rotation = Quaternion.Euler(0, 0, 315f);

            if (hit)
            {
                float distLen = Mathf.Sqrt(Mathf.Pow(hit.collider.bounds.size.x, 2) / 2) + 0.1f;

                slopeDest = new Vector2(transform.position.x - distLen, transform.position.y + distLen);

                slopeSet = true;
            }
        }
    }

    private void SlopeWalk(int dir)
    {

        if ((Vector2)transform.position != slopeDest)
        {
            transform.position = Vector2.MoveTowards(transform.position, slopeDest, speed * Time.deltaTime);
            //Debug.Log(slopeDest.x + " " + slopeDest.y);
        }
        else
        {
            slopeWalk = false;
            slopeSet = false;
            //PUT NEW RAYCAST HERE TO SET NEW POINTS
        }
    }

    private void Walk(Vector2[] points)
    {
        if (slopeWalk == true)
        {
            if (slopeSet == false)
            {
                SetSlopeWalk(direction);
            }
            SlopeWalk(direction);
        }
        else
        {
            //Debug.Log("Normal walk");
            MoveSlime(points);
        }
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



}
