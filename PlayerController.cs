using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //used for debugging purposes
    //set in Update() method
    //public Vector2 bodyVelocity;

    //speed of the player's horizontal movement
    //used in Move() function
    public float speed;

    //velocity added during double jump
    //used in Jump() function
    public float doubleJumpVelocity;

    //Set in CheckIfGrounded() function
    //used to enable jumping in PerformKeyPresses() function
    [HideInInspector]
    public bool grounded = false;

    //Changed to false in CheckIfGrounded() function
    //set to true after second jump in Jump() function
    private bool doubleJump = false;

    //speed at which character dashes
    //used in EnactDash() method
    public float dashVelocity;

    //used and set in CheckIfGrounded() method
    //properties used in KeepGrounded() method
    RaycastHit2D ground;

    //lets player jump despite KeepGrounded() method
    //set and reset in HasJumped() method
    [HideInInspector]
    public bool hasJumped = false;

    //keeps track of whether or not the Jump key has been pressed
    //is used in the update function
    bool jumpDown = false;

    //checks if player has pressed dash button
    //set in CheckKeys() method
    //used in Dash() method
    //reset in EnactDash() method
    bool dash = false;

    //checks if player is currently dashing
    //set in EnactDash() method
    //used in Dash() method
    bool isDashing = false;

    //checks if player is dashing and prevents them from jumping during the dash
    //is terminated BEFORE isDashing to allow jump immediately after dash but not second dash
    //called in EnactDash() method
    //is used in NoXMovement() method
    [HideInInspector]
    public bool noDashJump = false;

    //used to prevent multiple dashes per jump
    //set in Dash() method
    //reset in CheckIfGrounded() method
    //used in Dash() method
    bool airDash = false;

    //checks if player is running
    //is set and reset in CheckKeys() method
    //used in Move() method
    bool run = false;

    //checks if run was held at the time of the jump
    //set in the Jump() method
    //reset in CheckIfGrounded() method
    //used in Move() method
    private bool runAtTimeOfJump = false;

    //takes direction at the time of first jump
    //set in Jump() function
    //used in Move() function
    private float directionAtTimeOfJump = 0;

    //determines direction character is facing 
    //initially set in Awake() method
    //set thereon in CheckDirection() method
    //Used in Move() method
    //Used in EnactDash() method
    [HideInInspector]
    public float direction;

    //lets the awake direction be set without changing direction directly
    //used in Awake()
    public int awakeDirection;

    //finds prior position of player
    //initially set in Awake() method
    //set then on from CheckDirection() method
    //used in CheckDirection() method
    private float previousDirection;

    //used to control costly calculations by reducing them to once in however many frames
    //used in CheckIfGrounded() function
    //incremented in Update() function
    [HideInInspector]
    public int groundedFrames = 0;

    //finds distance on the x-axis between previous and current location of the character
    //set in CheckDirection() method
    //used in CheckDirection() method
    //[HideInInspector]
    public float offset = 0;

    //used literally fucking everywhere
    [HideInInspector]
    public Rigidbody2D body;

    //used in CheckIfGrounded() function for position and height of raycast
    //used in CheckDistanceToGround() function for "" "" "" "" ""
    CapsuleCollider2D capsuleCollider;

    //taken from child object Items
    //used in DeployUmbrella()
    //used as conditional in CheckFreeFall()
    ItemSwitcher itemSwitcher;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        itemSwitcher = GetComponentInChildren<ItemSwitcher>();
        previousDirection = transform.position.x;
        direction = awakeDirection;

        offset = transform.position.x - previousDirection;
    }

    //private void FixedUpdate()
    //{
    //    CheckIfGrounded();
    //    CheckDistanceToGround();
    //    CheckFreeFall();
    //    UmbrellaDeployed();
    //}

    //private void Update()
    //{
    //    //bodyVelocity = body.velocity;

    //    CheckKeyInput();
    //    PerformKeyPresses();
    //    CheckDirection();

    //    ++groundedFrames;

    //    //Debug.Log(transform.position.x);
    //}

    //private void LateUpdate()
    //{
    //    transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    //    KeepGrounded();
    //    NoXMovement();

    //}

    //pretty straight forward
    //called in Update() method
    public void PerformKeyPresses()
    {
        Move();

        if ((grounded || doubleJump == false) && jumpDown == true)
        {
            Jump();
        }

        if (dash)
        {
            Dash();
        }
    }

    //shoots raycast down from center of capsule collider and looks for the ground layer
    //sets bool grounded to true if raycast returns a hit
    public void CheckIfGrounded()
    {
        if (groundedFrames >= 1)
        {
            ground = Physics2D.Raycast(capsuleCollider.transform.position, -Vector2.up, capsuleCollider.size.y / 2 + 0.05f, LayerMask.GetMask("Ground"));

            if (ground)
            {
                grounded = true;
                doubleJump = false;
                airDash = false;
                //runAtTimeOfJump = false;
            }
            else
            {
                grounded = false;
            }

            groundedFrames = 0;
        }
    }

    //Checks what keys are being pressed
    public void CheckKeyInput()
    {
        //jump key
        if (Input.GetButtonDown("Jump"))
        {
            StartCoroutine(HasJumped());
            jumpDown = true;
            //Debug.Log("Jump key pressed");
        }
        else
        {
            jumpDown = false;
        }

        //dash key
        if(Input.GetAxisRaw("Dash") > 0)
        {
            //Debug.Log("Right trigger pressed");
            dash = true;
        }
        else
        {
            dash = false;
        }

        if(Input.GetAxisRaw("Run") > 0)
        {
            //Debug.Log("Left Trigger Pressed");
            run = true;
        }
        else
        {
            run = false;
        }
    }

    //lets character jump
    private void Jump()
    {
        if (noDashJump == false)
        {
            if (grounded == false)
            {
                doubleJump = true;
                //body.velocity = new Vector2(body.velocity.x, doubleJumpVelocity);
                body.velocity = new Vector2(body.velocity.x, 0);
                body.AddForce(new Vector2(body.velocity.x, doubleJumpVelocity));
            }
            else
            {
                directionAtTimeOfJump = direction;
                if (run == true)
                {
                    runAtTimeOfJump = true;
                    StartCoroutine(KillRunAtTimeOfJump());
                }

                //body.velocity = new Vector2(body.velocity.x, doubleJumpVelocity);
                body.AddForce(new Vector2(body.velocity.x, doubleJumpVelocity));

            }
        }
        //CheckYVelocity();
    }

    private IEnumerator KillRunAtTimeOfJump()
    {
        while(body.velocity.y >= 0 || grounded == false)
        {
            yield return null;
        }
        runAtTimeOfJump = false;
    }

    //NOTE::: rendered obsolete by negating y velocity of rigidbody in Jump()
    //prevent the double jump from getting totally fucking insane (up to 7.5 units w/o check)
    //private void CheckYVelocity()
    //{
    //    if (body.velocity.y > 5)
    //    {
    //        body.velocity = new Vector2(body.velocity.x, 5);
    //    }
    //}

    //preeeeetttyyyy straight forward -- if player isn't dashing, move player
    private void Move()
    {
        if (isDashing == false)
        {
            if(run == true && grounded == true)
            {
                body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * (speed * 1.5f), body.velocity.y);
            }
            else if(grounded == false && runAtTimeOfJump == true)
            {
                if (directionAtTimeOfJump > 0)
                {
                    if (Input.GetAxisRaw("Horizontal") > 0)
                    {
                        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * (speed * 1.5f), body.velocity.y);
                    }
                    else if(Input.GetAxisRaw("Horizontal") < 0)
                    {
                        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * (speed * 0.75f), body.velocity.y);
                    }
                }
                else if(directionAtTimeOfJump < 0)
                {
                    if (Input.GetAxisRaw("Horizontal") < 0)
                    {
                        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * (speed * 1.5f), body.velocity.y);
                    }
                    else if (Input.GetAxisRaw("Horizontal") > 0)
                    {
                        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * (speed * 0.75f), body.velocity.y);
                    }
                }
            }
            else
            {
                body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, body.velocity.y);
            }
        }       
    }

    //If player is in the air, checks the distance from the character to the ground, and starts Coroutine when it is close to the ground
    //used to prevent colliders from overlapping
    public void CheckDistanceToGround()
    {
        if (!grounded)
        {
            RaycastHit2D distanceToGround = Physics2D.Raycast(capsuleCollider.transform.position, -Vector2.up, capsuleCollider.size.y, LayerMask.GetMask("Ground"));

            //Debug.DrawRay(transform.position, -Vector2.up * capsuleCollider.size.y, Color.red, 5.0f);

            if (distanceToGround && distanceToGround.distance < capsuleCollider.size.y / 2 + 0.5f && body.velocity.y < 0)
            {
                StartCoroutine(CollisionDetectionWait());
            }
        }
    }

    //sets collision detection to Continuous upon impact to prevent colliders from overlapping
    //used in CheckDistanceToGround() method
    private IEnumerator CollisionDetectionWait()
    {
        body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        yield return new WaitForSeconds(0.2f);

        body.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
    }

    //checks that the dash button has been pressed and if the character is already dashing
    //used in PerformKeyPresses() method
    private void Dash()
    {
        if (dash == true && isDashing == false && airDash == false)
        {
            StartCoroutine(EnactDash());
            if(grounded == false)
            {
                airDash = true;
            }
        }
    }

    //causes player to dash and resets dash and isDashing values to false after finishing
    //used in Dash() method
    private IEnumerator EnactDash()
    {
        isDashing = true;
        noDashJump = true;

        body.velocity = Vector2.zero;
        body.velocity = new Vector2(direction * dashVelocity, 0f);

        Coroutine fG = StartCoroutine(FuckGravity());

        yield return new WaitForSeconds(0.2f);

        StopCoroutine(fG);

        body.velocity = Vector2.zero;
        noDashJump = false;

        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, body.velocity.y);

        yield return new WaitForSeconds(0.1f);

        isDashing = false;
        dash = false;
    }

    //prevents player from dropping while dashing in the air
    //called and terminated in EnactDash() method
    private IEnumerator FuckGravity()
    {
        bool placeHolder = true;
        while (placeHolder)
        {
            body.velocity = new Vector2(body.velocity.x, body.velocity.y + (9.81f * Time.deltaTime));
            yield return null;
        }

    }

    //prevents player bouncing on ground surfaces
    //used in LateUpdate() method
    public void KeepGrounded()
    {
        if(grounded && hasJumped == false)
        {
            transform.position = new Vector2(transform.position.x, ground.transform.position.y + (ground.collider.bounds.size.y / 2) + (capsuleCollider.size.y / 2));
        }
    }
    
    //lets player jump from ground surfaces
    //juxtaposes KeepGrounded() method
    //used in CheckKeyInput() method
    private IEnumerator HasJumped()
    {
        hasJumped = true;

        yield return new WaitForSeconds(0.2f);

        hasJumped = false;
    }

    //Checks the direction the player is facing
    //used in the Update() method
    public void CheckDirection()
    {
        //////////////////
        ////NOTE: this portion of code is obsolete because I'm a moron. I'm keeping it so all future me's can
        ////see it and think, wow, maybe just take a step back and consider simpler alternate solutions
        //////////////////
        //this exists because the player turns around on collision with other objects because the offset
        //becomes positive/negative when the player rebounds and thusly flips the player around
        //affecting the dash direction, jump forces, and bomb throws, AND YES ITS CHEAP AND PROBABLY NOT THE BEST SOLUTION
        //BUT IT WORKS SO JUST MOVE ON, ALL RIGHT, JUST MOVE THE FUCK ON, DUDE
        //if (noDashJump == false)
        //{
        //    if (direction == 1)
        //    {
        //        offset = (transform.position.x - previousDirection) + 0.06f;
        //    }
        //    else if (direction == -1)
        //    {
        //        offset = (transform.position.x - previousDirection) - 0.06f;
        //    }
        //}
        //else
        //{
        //    if (direction == 1)
        //    {
        //        offset = (transform.position.x - previousDirection) + 0.1f;
        //    }
        //    else if (direction == -1)
        //    {
        //        offset = (transform.position.x - previousDirection) - 0.1f;
        //    }
        //}

        //if (offset != 0)
        //{
        //    if (Input.GetAxisRaw("Horizontal") > 0 || offset > 0)
        //    {
        //        direction = 1;
        //    }
        //    else if (Input.GetAxisRaw("Horizontal") < 0 || offset < 0)
        //    {
        //        direction = -1;
        //    }
        //}

        if (Input.GetAxis("Horizontal") > 0)// || offset > 0)
        {
            direction = 1;
        }
        else if (Input.GetAxis("Horizontal") < 0)// || offset < 0)
        {
            direction = -1;
        }

        //previousDirection = transform.position.x;
    }

    //freezes movement on x axis when there is no input to prevent player from sliding
    //used to ensure direction is set properly in CheckDirection() method
    //used in LateUpdate() method
    public void NoXMovement()
    {
        if(Input.GetAxisRaw("Horizontal") == 0 && noDashJump == false)
        {
            body.constraints = RigidbodyConstraints2D.FreezePositionX;
        }
        else
        {
            body.constraints = RigidbodyConstraints2D.None;
        }
    }

    //keeps player from reaching near terminal velocity during free falls
    //NOTE::::: potentially augment this function to limit speed during in game velocity changes
    //          may react badly with CheckYVelocity()
    //called in FixedUpdate method
    public void CheckFreeFall()
    {
        if (body.velocity.y <= -15 && itemSwitcher.deployUmbrella == false)
        {
            body.velocity = new Vector2(body.velocity.x, -15);
        }
    }

    //resets body.velocity.y when umbrella key has been pressed
    //called in FixedUpdate() method
    public void UmbrellaDeployed()
    {
        if(itemSwitcher.deployUmbrella == true && body.velocity.y <= -2)
        {
            body.velocity = new Vector2(body.velocity.x, -2);
        }
    }

}
