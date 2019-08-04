using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //pull and interact with HealthHUDScript
    //FOR TIME BEING -- CANNOT BE GREATER THAN 6
    public int health = 6;

    public float invulTime;
    float invulTimer = 0;
    bool isInvul = false;

    //used for debugging purposes
    //set in Update() method
    //public Vector2 bodyVelocity;
    public Vector2 vel;

    //speed of the player's horizontal movement
    //used in Move() function
    public float speed;

    //velocity added during double jump
    //used in Jump() function
    public float jumpVelocity;

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
    public int direction;

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
    [HideInInspector]
    public BoxCollider2D boxCollider;

    //taken from child object Items
    //used in DeployUmbrella()
    //used as conditional in CheckFreeFall()
    //ItemSwitcher itemSwitcher;
    ItemSwitcherAlt itemSwitcherAlt;

    PlayerAnimationScript playerAnimScript;

    [HideInInspector]
    public bool isHurt = false;

    public float hurtForce;
    public float yHurtForce;

    Coroutine dashRoutine = null;
    Coroutine fG;

    bool waterSet = false;

    public bool inWater = false;
    bool waterVel = false;

    bool waterSpeed = false;
    bool belowEnemy = false;

    InputManager IM;

    WorldSwitcher worldSwitcher;

    Vector3 halfCollSizeX;
    Vector3 halfCollSizeY;

    SpriteRenderer rend;
    Color white = Color.white;
    Color fade = new Color(1, 1, 1, 0.75f);

    float deathTime;
    float deathSwitch;
    float deathTimer = 0;
    float switchTimer = 0;
    bool deathSet = false;
    [HideInInspector]
    public bool isDead = false;

    float waterDownVelocity = -2;
    float downVelocity = -10;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        //itemSwitcher = GetComponentInChildren<ItemSwitcher>();
        itemSwitcherAlt = GetComponentInChildren<ItemSwitcherAlt>();
        playerAnimScript = GetComponent<PlayerAnimationScript>();
        rend = GetComponent<SpriteRenderer>();
        previousDirection = transform.position.x;
        direction = awakeDirection;
        IM = GetComponent<InputManager>();
        worldSwitcher = GetComponentInChildren<WorldSwitcher>();
        halfCollSizeX = new Vector3((boxCollider.size.x / 2) - (boxCollider.size.x / 10), 0f, 0f);
        halfCollSizeY = new Vector3(0f, boxCollider.size.y / 2, 0f);
        offset = transform.position.x - previousDirection;
        deathTime = 2;
        deathSwitch = deathTime / 6;
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

    public void DoPhysics()
    {
        if (playerAnimScript.hurt == false)
        {
            //PerformKeyPresses();
            UmbrellaDeployed();
        }
        else if (isHurt == true && inWater == false && belowEnemy == false)
        {
            //Debug.Log("get hurt");
            KillDashOnHurt();
            body.velocity = Vector2.zero;
            body.AddForce(new Vector2(-direction * hurtForce, yHurtForce));
            StartCoroutine(CheckHurtHeight(body.position.x, body.position.y));
            isHurt = false;
        }
        else if (isHurt == true && inWater == false && belowEnemy == true)
        {
            //Debug.Log("get hurt");
            KillDashOnHurt();
            body.velocity = Vector2.zero;
            body.AddForce(new Vector2(-direction * hurtForce, yHurtForce));
            StartCoroutine(BelowHurtTime());
            belowEnemy = false;
            isHurt = false;
        }
        else if(isHurt == true && inWater == true)
        {
            Vector2 vel = body.velocity;
            body.velocity = Vector2.zero;
            body.AddForce(new Vector2(-direction * hurtForce, (vel.y / Mathf.Abs(vel.y)) * yHurtForce));
            StartCoroutine(WaterHurtTime());
            isHurt = false;
        }

        CheckIfGrounded();
        KeepGroundedFU();
        CheckDistanceToGround();
        CheckFreeFall();

    }

    public void CheckHealth()
    {
        if(health > 6)
        {
            health = 6;
        }
        else if(health <= 0)
        {
            if(deathSet == false)
            {
                isDead = true;
                boxCollider.enabled = false;
                rend.color = white;
                rend.enabled = false;
                body.constraints = RigidbodyConstraints2D.FreezeAll;
                deathSet = true;
            }

            if(switchTimer >= deathSwitch)
            {
                rend.enabled = !rend.enabled;
                switchTimer = 0;
            }

            if(deathTimer >= deathTime)
            {
                Destroy(gameObject);
            }

            switchTimer += Time.deltaTime;
            deathTimer += Time.deltaTime;
            //destroy game object
        }
    }

    private IEnumerator CheckHurtHeight(float x, float y)
    {
        yield return new WaitForSeconds(0.1f);
        while (grounded == false && body.position.y > y)
        {
            //Debug.Log(grounded);
            yield return null;
        }
        playerAnimScript.hurt = false;
    }

    private IEnumerator WaterHurtTime()
    {
        yield return new WaitForSeconds(1);
        playerAnimScript.hurt = false;
    }

    private IEnumerator BelowHurtTime()
    {
        yield return new WaitForSeconds(0.5f);
        playerAnimScript.hurt = false;
    }

    //pretty straight forward
    //called in Update() method
    public void PerformKeyPresses()
    {
        Move();
        HoldDown();

        if ((grounded || doubleJump == false) && jumpDown == true)
        {
            Jump();
        }

        if (dash && inWater == false)
        {
            Dash();
        }
    }

    //shoots raycast down from center of capsule collider and looks for the ground layer
    //sets bool grounded to true if raycast returns a hit
    public void CheckIfGrounded()
    {
        //Debug.Log("Ground" + (worldSwitcher.activeWorldNum + 1));

        Vector3 leftPos = boxCollider.transform.position - halfCollSizeX - halfCollSizeY;                
        RaycastHit2D left = Physics2D.Raycast(leftPos, -Vector2.up, 0.1f, LayerMask.GetMask("Ground" + (worldSwitcher.activeWorldNum + 1)));

        Vector3 rightPos = boxCollider.transform.position + halfCollSizeX - halfCollSizeY;
        RaycastHit2D right = Physics2D.Raycast(rightPos, -Vector2.up, 0.1f, LayerMask.GetMask("Ground" + (worldSwitcher.activeWorldNum + 1)));

        RaycastHit2D mid = Physics2D.Raycast(boxCollider.transform.position - halfCollSizeY, -Vector2.up, 0.1f, LayerMask.GetMask("Ground" + (worldSwitcher.activeWorldNum + 1)));

        Debug.DrawRay(leftPos, -Vector2.up);
        Debug.DrawRay(rightPos, -Vector2.up);
        Debug.DrawRay(boxCollider.transform.position - halfCollSizeY, -Vector2.up);

        //ground = Physics2D.Raycast(boxCollider.transform.position, -Vector2.up, (boxCollider.size.y / 2) + 0.1f, LayerMask.GetMask("Ground" + (worldSwitcher.activeWorldNum + 1)));

        //Debug.DrawLine(boxCollider.transform.position, boxCollider.transform.position - new Vector3(0f, Mathf.Sqrt(Mathf.Pow(boxCollider.size.y / 2, 2) * 2) + 0.1f, 0f), Color.red);

        if (mid || left || right)
        {
            if(mid)
            {
                //Debug.Log("Mid set");
                ground = mid;
            }
            else if(left)
            {
                //Debug.Log("Left set");
                ground = left;
            }
            else if(right)
            {
                //Debug.Log("Right set");
                ground = right;
            }

            if(body.velocity.y <= 0)
            {
                grounded = true;
                doubleJump = false;
                airDash = false;
            }
        }
        else
        {
            grounded = false;
        } 
    }

    //Checks what keys are being pressed
    public void CheckKeyInput()
    {
        //jump key
        if (Input.GetButtonDown(IM.jump))
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
        if(Input.GetAxisRaw(IM.dash) > 0)
        {
            //Debug.Log("Right trigger pressed");
            dash = true;
        }
        else
        {
            dash = false;
        }

        if(Input.GetAxisRaw(IM.run) > 0)
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
                //Debug.Log(body.velocity.y);
                body.velocity = new Vector2(body.velocity.x, 0);
                //Debug.Log(body.velocity.y);
                body.AddForce(new Vector2(body.velocity.x, jumpVelocity));
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
                body.AddForce(new Vector2(body.velocity.x, jumpVelocity));

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

    //preeeeetttyyyy straight forward -- if player isn't dashing, move player
    private void Move()
    {
        if (isDashing == false)
        {
            if(run == true && grounded == true)
            {
                body.velocity = new Vector2(Input.GetAxisRaw(IM.horizontal) * speed * 1.5f, body.velocity.y);
            }
            else if(grounded == false && runAtTimeOfJump == true)
            {
                if (directionAtTimeOfJump > 0)
                {
                    if (Input.GetAxisRaw(IM.horizontal) > 0)
                    {
                        body.velocity = new Vector2(Input.GetAxisRaw(IM.horizontal) * (speed * 1.5f), body.velocity.y);
                    }
                    else if(Input.GetAxisRaw(IM.horizontal) < 0)
                    {
                        body.velocity = new Vector2(Input.GetAxisRaw(IM.horizontal) * (speed * 0.75f), body.velocity.y);
                    }
                }
                else if(directionAtTimeOfJump < 0)
                {
                    if (Input.GetAxisRaw(IM.horizontal) < 0)
                    {
                        body.velocity = new Vector2(Input.GetAxisRaw(IM.horizontal) * (speed * 1.5f), body.velocity.y);
                    }
                    else if (Input.GetAxisRaw(IM.horizontal) > 0)
                    {
                        body.velocity = new Vector2(Input.GetAxisRaw(IM.horizontal) * (speed * 0.75f), body.velocity.y);
                    }
                }
            }
            else
            {
                body.velocity = new Vector2(Input.GetAxisRaw(IM.horizontal) * speed, body.velocity.y);
            }
        }       
    }

    private void HoldDown()
    {
        if(!grounded && !isDashing && Input.GetAxisRaw(IM.vertical) < 0)
        {
            if(inWater && body.velocity.y > -2)
            {
                body.velocity = new Vector2(body.velocity.x, waterDownVelocity);
            }
            else if(body.velocity.y > downVelocity)
            {
                Debug.Log("Holding down down");
                body.velocity = new Vector2(body.velocity.x, downVelocity);
            }
        }
    }

    //If player is in the air, checks the distance from the character to the ground, and starts Coroutine when it is close to the ground
    //used to prevent colliders from overlapping
    public void CheckDistanceToGround()
    {
        if (!grounded)
        {
            RaycastHit2D distanceToGround = Physics2D.Raycast(boxCollider.transform.position, -Vector2.up, boxCollider.size.y, LayerMask.GetMask("Ground" + (worldSwitcher.activeWorldNum + 1)));

            //Debug.DrawRay(transform.position, -Vector2.up * boxCollider.size.y, Color.red, 5.0f);

            if (distanceToGround && distanceToGround.distance < boxCollider.size.y / 2 + 0.5f && body.velocity.y < 0)
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
            dashRoutine = StartCoroutine(EnactDash());
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

        fG = StartCoroutine(FuckGravity());

        yield return new WaitForSeconds(0.2f);

        if (fG != null)
        {
            StopCoroutine(fG);
        }

        body.velocity = Vector2.zero;
        noDashJump = false;

        body.velocity = new Vector2(Input.GetAxisRaw(IM.horizontal) * speed, body.velocity.y);

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

    public void KillDashOnHurt()
    {
        if (dashRoutine != null && playerAnimScript.hurt == true)
        {
            //Debug.Log("Killing coroutine");
            StopCoroutine(dashRoutine);
            StopCoroutine(fG);
            noDashJump = false;
            isDashing = false;
            dash = false;
        }
    }

    //prevents player bouncing on ground surfaces
    //used in LateUpdate() method
    public void KeepGroundedFU()
    {
        if (grounded && hasJumped == false && playerAnimScript.hurt == false)
        {
            //transform.position = new Vector2(transform.position.x, ground.transform.position.y + (ground.collider.bounds.size.y / 2) + (boxCollider.size.y / 2));
        }
    }

    public void KeepGroundedLU()
    {
        transform.rotation = Quaternion.Euler(Vector3.zero);
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

        if (Input.GetAxis(IM.horizontal) > 0)
        {
            direction = 1;
        }
        else if (Input.GetAxis(IM.horizontal) < 0)
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
        if(Input.GetAxisRaw(IM.horizontal) == 0 && noDashJump == false && playerAnimScript.hurt == false)
        {
            body.constraints = RigidbodyConstraints2D.FreezePositionX;
        }
        else
        {
            body.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
        }
    }

    //keeps player from reaching near terminal velocity during free falls
    //NOTE::::: potentially augment this function to limit speed during in game velocity changes
    //          may react badly with CheckYVelocity()
    //called in FixedUpdate method
    public void CheckFreeFall()
    {
        if (body.velocity.y <= -15 && itemSwitcherAlt.deployUmbrella == false && inWater == false)
        {
            body.velocity = new Vector2(body.velocity.x, -15);
        }
        else if(body.velocity.y <= -2 && inWater == true)
        {
            body.velocity = new Vector2(body.velocity.x, -2);
        }
    }

    //resets body.velocity.y when umbrella key has been pressed
    //called in FixedUpdate() method
    public void UmbrellaDeployed()
    {
        if(itemSwitcherAlt.deployUmbrella == true && body.velocity.y <= -2)
        {
            body.velocity = new Vector2(body.velocity.x, -2);
        }
    }

    public void Swim()
    {
        if(inWater == true && waterSet == false)
        {
            body.gravityScale /= 2;
            waterSet = true;
        }
        else if(inWater == false && waterSet == true)
        {
            body.gravityScale *= 2;
            waterSet = false;
        }

        if (inWater == true && waterVel == false)
        {
            jumpVelocity /= 2.5f;
            waterVel = true;
        }
        else if(inWater == false && waterVel == true)
        {
            jumpVelocity *= 2.5f;
            waterVel = false;
        }

        if(inWater == true && waterSpeed == false)
        {
            speed /= 2;
            waterSpeed = true;
        }
        else if(inWater == false && waterSpeed == true)
        {
            speed *= 2;
            waterSpeed = false;
        }

        if (inWater == true)
        {
            doubleJump = false;
        }
    }

    public void GetHurt(Vector2 pos)
    {
        if (!isInvul)
        {
            isHurt = true;
            playerAnimScript.hurt = true;
            isInvul = true;
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);
            Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("EnemyB"), true);

            health -= 1;
            rend.color = fade;

            if (transform.position.y < pos.y)
            {
                belowEnemy = true;
            }
        }
    }

    public void InvulnerableOnHurt()
    {
        if(isInvul)
        {
            invulTimer += Time.deltaTime;

            if(invulTimer >= invulTime)
            {
                rend.color = white;
                isInvul = false;
                invulTimer = 0;
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
                Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("EnemyB"), false);
            }
        }
    }

}
