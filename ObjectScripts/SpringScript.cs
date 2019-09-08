using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringScript : MonoBehaviour
{

    Vector2 pos1;
    public Vector2 pos2;
    Vector2 midPos;
    Vector2 midMidPos;
    GameObject player;
    PlayerController playerController;
    BoxCollider2D coll;
    BoxCollider2D playerColl;
    public Sprite springDown;
    public Sprite springUp;
    SpriteRenderer rend;

    bool isDown = true;
    bool setSpringUp = false;
    public float springTime;
    float springTimer = 0;

    public float speed;
    public float lerpSpeed;
    public float adjustSpeed;
    bool atMidPos = false;

    float xDist;
    float halfX;
    float dist = 0;
    float distTraveled = 0f;
    float arcSpeed;

    int direction;

    public bool isTraveling = false;

    private void Awake()
    {
        FindMidPos();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerColl = player.GetComponent<BoxCollider2D>();
        coll = GetComponent<BoxCollider2D>();
        rend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ManageSpring();
    }

    private void FixedUpdate()
    {
        if (isTraveling) MovePlayer();   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isTraveling = true;
            playerController.isSprung = true;
            playerController.MakeKinematic();
            playerController.body.velocity = Vector2.zero;
            playerController.transform.position = new Vector2(coll.transform.position.x, coll.transform.position.y + coll.bounds.extents.y + playerColl.bounds.extents.y);
            isDown = false;
        }
    }

    private void MovePlayer()
    {
        if((Vector2)player.transform.position == pos2)
        {
            //arrived
            
        }
        else if(player.transform.position.y >= midPos.y)
        {
            atMidPos = true;
        }

        adjustSpeed = speed + lerpSpeed * (pos2.y - player.transform.position.y);

        if (adjustSpeed < speed) adjustSpeed = speed;

        if (atMidPos == false)
        {
            player.transform.position = Vector2.MoveTowards(player.transform.position, midPos, adjustSpeed * Time.deltaTime);
            Debug.Log("Adjust speed = " + adjustSpeed * Time.deltaTime);
        }
        else
        {
            distTraveled += speed * (halfX / arcSpeed) * Time.deltaTime;
            //Debug.Log(xDist / arcSpeed);
            Debug.Log("arcSpeed = " + (halfX / arcSpeed));

            if (distTraveled <= xDist)
            {
                //Debug.Log("Doing half circle motion");
                float x = direction * (halfX - distTraveled);
                float y = Mathf.Sqrt(Mathf.Pow(halfX, 2) - Mathf.Pow(x, 2));
                //Debug.Log("x = " + x + "\ny = " + y);
                float theta = Mathf.Atan(y / x);
                //Debug.Log("angle theta is " + theta);
                if (theta < 0) theta = Mathf.PI + theta;

                player.transform.position = new Vector2(midMidPos.x + halfX * Mathf.Cos(theta),
                                                        midPos.y + halfX * Mathf.Sin(theta));
            }
            else
            {
                isTraveling = false;
                player.transform.position = pos2;
                //Debug.Log("Player has arrived");
                playerController.isSprung = false;
                playerController.MakeDynamic();
                atMidPos = false;
                distTraveled = 0;
            }
        }
    }

    private void FindMidPos()
    {
        pos1 = transform.position;
        midPos = new Vector2(pos1.x, pos2.y);
        midMidPos = new Vector2(pos1.x + ((pos2.x - pos1.x) / 2), pos2.y);
        xDist = Mathf.Abs(pos1.x - pos2.x);
        halfX = xDist / 2;
        arcSpeed = Mathf.PI * halfX;

        if (pos1.x > pos2.x) direction = 1;
        else if (pos1.x < pos2.x) direction = -1;
    }

    private void ManageSpring()
    {
        if(!isDown)
        {
            if(!setSpringUp)
            {
                rend.sprite = springUp;
                setSpringUp = true;
            }

            springTimer += Time.deltaTime;

            if(springTimer >= springTime)
            {
                isDown = true;
                rend.sprite = springDown;
                springTimer = 0;
                setSpringUp = false;
            }
        }
    }
}
