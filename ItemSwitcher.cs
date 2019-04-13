using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSwitcher : MonoBehaviour {

    //public array of sprites for items in game
    //initialized in Unity window
    //indexed in Awake(), SetSprite()
    public Sprite[] items;

    //public array of alternate sprites for items that have an active state
    //used in DisplayOpenUmbrella()
    public Sprite[] itemAlt;
    
    //keeps track of which item is being displayed in numerical order
    //incremented in FixedUpdate()
    //used as index in Awake()
    //reset in FixedUpdate
    //passed as parameter to SetSprite() in FixedUpdate()
    //used as conditional in AdjustItemsForDash(), AdjustRaygunForJump(), FixedUpdate()
    public int itemIndex = 0;

    //holds value whether or not the umbrella is being used
    //set and reset in UseItem()
    //called in PlayerController script in parent
    [HideInInspector]
    public bool deployUmbrella = false;

    //holds the sprite that is being rendered
    //component retrieved in Awake() and sets initial sprite
    //used in SetSprite(), FlipSprite(), AdjustRaygunForJump
    SpriteRenderer spriteRenderer;

    //variable holds the Player GameObject
    //retrieved in Awake()
    //used in SetSprite(), FlipSprites(), AdjustItemsForDash(), AdjustRaygunForJump()
    GameObject player;

    //holds the playerController script attached to the Player GameObject
    //retrieved in Awake()
    //used in SetSprite(), FlipSprites(), AdjustItemsForDash(), AdjustRaygunForJump()
    PlayerController playerController;
    
    //holds what direction player is facing at the time of an item swap
    //is set in the SetSprite() method
    //used as conditional, inverted in FlipSprites()
    bool facingRight;

    //determines if the player's character sprite is Dash
    //set and used in AdjustItemsForDash()
    bool dashPosition = false;

    //determines if the player's character sprite is Jump
    //used in AdjustRaygunForJump()
    bool jumpPosition = false;

    //determines if the Swap button has been pressed 
    //set in Switcher()
    //used in FixedUpdate()
    //reset in SetSprite()
    bool startSwitcher = false; 

    //holds sprite position relative to parent Player object RIGHT
    //used in SetSprite()
    Vector3[] itemPositionsR = 
    {
        Vector3.zero,
        new Vector3(0, 0.575f, 0),
        new Vector3(0.35f, -0.15f, 0),
        new Vector3(0.385f, -0.145f, 0),
        new Vector3(0.385f, -0.145f, 0),
        new Vector3(0, 0.575f, 0)
    };

    //holds sprite position relative to parent Player object LEFT
    //used in SetSprite()
    Vector3[] itemPositionsL =
    {
        Vector3.zero,
        new Vector3(0, 0.575f, 0),
        new Vector3(-0.35f, -0.15f, 0),
        new Vector3(-0.385f, -0.145f, 0),
        new Vector3(-0.385f, -0.145f, 0),
        new Vector3(0, 0.575f, 0)
    };

    //initializes necessary objects on load
    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = items[itemIndex];
    }

    //private void Update()
    //{
    //    Switcher();
    //    UseItem();
    //    DisplayOpenUmbrella();
    //}

    //private void FixedUpdate()
    //{
    //    StartSwitcher();

    //    //transform.position = player.transform.position + itemPositionsR[itemIndex];
    //    AdjustRaygunForJump();
    //    AdjustItemsForDash();

    //}

    private void LateUpdate()
    {
        //CorrectPosition();
    }

    public void StartSwitcher()
    {
        if (startSwitcher == true)
        {
            ++itemIndex;
            if (itemIndex == 6)
            {
                itemIndex = 0;
            }
            SetSprite(itemIndex);
        }

        if (itemIndex != 1 && itemIndex != 5)
        {
            FlipSprites();
        }
    }

    //determines wheter or not Swap button has been pressed
    public void Switcher()
    {
        if(Input.GetButtonDown("Swap"))
        {
            startSwitcher = true;
        }
    }

    //sets sprite according to player direction and item being displayed
    private void SetSprite(int index)
    {
        //Debug.Log(player.transform.position.x + " " + player.transform.position.y);
        //transform.position = Vector3.zero + player.transform.position;

        transform.position = Vector3.zero;

        if (playerController.direction > 0)
        {
            transform.position = player.transform.position + itemPositionsR[index];
            spriteRenderer.flipX = false;
            facingRight = true;
        }
        else if (playerController.direction < 0)
        {
            transform.position = player.transform.position + itemPositionsL[index];
            spriteRenderer.flipX = true;
            facingRight = false;
        }

        if(index == 1 || index == 5)
        {
            spriteRenderer.sortingOrder = 0;
        }
        else
        {
            spriteRenderer.sortingOrder = 2;
        }

        spriteRenderer.sprite = items[index];
        startSwitcher = false;
    }

    //flips sprites across player body when direction changes
    private void FlipSprites()
    {
        if(playerController.direction > 0 && facingRight == false)
        {
            facingRight = !facingRight;
            spriteRenderer.flipX = !spriteRenderer.flipX;
            transform.position = new Vector3(transform.position.x + Mathf.Abs(player.transform.position.x - transform.position.x) * 2, transform.position.y, transform.position.z);
        }
        else if(playerController.direction < 0 && facingRight == true)
        {
            facingRight = !facingRight;
            spriteRenderer.flipX = !spriteRenderer.flipX;
            transform.position = new Vector3(transform.position.x - Mathf.Abs(player.transform.position.x - transform.position.x) * 2, transform.position.y, transform.position.z);
        }
    }

    //adjusts umbrella and torch when the player is dashing
    public void AdjustItemsForDash()
    {
        if (playerController.noDashJump == true && dashPosition == false && (itemIndex == 1 || itemIndex == 5))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.125f, transform.position.z);
            dashPosition = true;
        }
        else if (playerController.noDashJump == false && dashPosition == true && (itemIndex == 1 || itemIndex == 5))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.125f, transform.position.z);
            dashPosition = false;
        }
    }

    //adjusts raygun when player jumps
    public void AdjustRaygunForJump()
    {
        if(playerController.grounded == false && itemIndex == 2 && jumpPosition == false)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            jumpPosition = true;
        }
        else if(playerController.grounded == true && itemIndex == 2 && jumpPosition == true)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
            jumpPosition = false;
        }

        if (playerController.grounded == true && itemIndex == 2)
        {
            transform.position = new Vector3(transform.position.x, player.transform.position.y - 0.15f, transform.position.z);
        }
    }

    //checks to see if UseItem button has been pressed and sets appropriate boolean values to be used in other scripts
    public void UseItem()
    {
        //button pushed
        if(Input.GetButtonDown("UseItem"))
        {
            switch(itemIndex)
            {
                case 0:
                    break;
                case 1:
                    deployUmbrella = true;
                    break;
            }
        }

        //button released
        if(Input.GetButtonUp("UseItem") || Input.GetButtonDown("Swap"))
        {
            switch(itemIndex)
            {
                case 1:
                    deployUmbrella = false;
                    break;
            }
        }
    }

    //if the umbrella is displayed and the use item button is being held, display open umbrella sprite
    public void DisplayOpenUmbrella()
    {
        if(itemIndex == 1 && deployUmbrella == true)
        {
            spriteRenderer.sprite = itemAlt[0];
        }
        else if(itemIndex == 1 && deployUmbrella == false)
        {
            spriteRenderer.sprite = items[1];
        }
    }

    //if the position of the item is off, lerps toward correct position
    //NOTE:::: LERP/MOVETOWARDS IS SUPER IMPORTANT, DO NOT CHANGE
    //called in LateUpdate()
    private void CorrectPosition()
    {
        if (playerController.direction > 0 && transform.position != player.transform.position + itemPositionsR[itemIndex])
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position + itemPositionsR[itemIndex], 1);

            Debug.Log("Correcting position");
        }
        else if (playerController.direction < 0 && transform.position != player.transform.position + itemPositionsL[itemIndex])
        {
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position + itemPositionsL[itemIndex], 1);

            Debug.Log("Correcting position");
        }
    }

}
