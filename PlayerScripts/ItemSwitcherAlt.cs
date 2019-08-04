using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSwitcherAlt : MonoBehaviour
{

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

    //holds the playerController script attached to the Player GameObject
    //retrieved in Awake()
    //used in SetSprite(), FlipSprites(), AdjustItemsForDash(), AdjustRaygunForJump()
    PlayerController playerController;


    //determines if the Swap button has been pressed 
    //set in Switcher()
    //used in FixedUpdate()
    //reset in SetSprite()
    bool startSwitcher = false;

    InputManager IM;

    SpriteRenderer[] renderers;

    SpriteRenderer current;

    int oldDir = 0;
    int oldIndex = 0;
    bool ground;

    //initializes necessary objects on load
    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        IM = GameObject.FindGameObjectWithTag("GameController").GetComponent<InputManager>();
        GetRenderers();

        //Debug.Log(renderers.Length);
    }

    //Update and Fixed Update functions are taken care of in Player Executor

    private void GetRenderers()
    {
        renderers = GetComponentsInChildren<SpriteRenderer>();
        for(int i = 0; i != renderers.Length; ++i)
        {
            renderers[i].enabled = false;
        }
    }

    public void IncrementIndex()
    {
        if (Input.GetButtonDown(IM.swap))
        {
            ++itemIndex;
            if (itemIndex == 5)
            {
                itemIndex = 0;
            }
        }
    }


    public void SetRenderer()
    {
        if (oldDir != playerController.direction || oldIndex != itemIndex || ground != playerController.grounded)
        {
            switch (itemIndex)
            {
                //empty
                case 0:
                    current = null;
                    SetNewSprite(current);
                    return;
                case 1:
                    renderers[0].enabled = true;
                    current = renderers[0];
                    SetNewSprite(current);
                    return;
                //raygun
                case 2:
                    if (playerController.grounded == true)
                    {
                        if (playerController.direction > 0)
                        {
                            renderers[1].enabled = true;
                            renderers[2].enabled = false;
                            current = renderers[1];
                        }
                        else if (playerController.direction < 0)
                        {
                            renderers[2].enabled = true;
                            renderers[1].enabled = false;
                            current = renderers[2];
                        }
                    }
                    else if(playerController.grounded == false)
                    {
                        if (playerController.direction > 0)
                        {
                            renderers[3].enabled = true;
                            renderers[4].enabled = false;
                            current = renderers[3];
                        }
                        else if (playerController.direction < 0)
                        {
                            renderers[4].enabled = true;
                            renderers[3].enabled = false;
                            current = renderers[4];
                        }
                    }
                    SetNewSprite(current);
                    return;
                //bomb
                case 3:
                    if (playerController.direction > 0)
                    {
                        renderers[5].enabled = true;
                        renderers[6].enabled = false;
                        current = renderers[5];
                    }
                    else if (playerController.direction < 0)
                    {
                        renderers[6].enabled = true;
                        renderers[5].enabled = false;
                        current = renderers[6];
                    }
                    SetNewSprite(current);
                    return;
                //shield
                case 4:
                    if (playerController.direction > 0)
                    {
                        renderers[7].enabled = true;
                        renderers[8].enabled = false;
                        current = renderers[7];
                    }
                    else if (playerController.direction < 0)
                    {
                        renderers[8].enabled = true;
                        renderers[7].enabled = false;
                        current = renderers[8];
                    }
                    SetNewSprite(current);
                    return;
                //torch
                //case 5:
                //    renderers[9].enabled = true;
                //    current = renderers[9];
                //    SetNewSprite(current);
                //    return;
                ////umbrella
                
            }

            
        }
    }

    private void SetNewSprite(SpriteRenderer current)
    {
        for (int i = 0; i != renderers.Length; ++i)
        {
            //Debug.Log("Disabling renderer[" + i + "]");
            if (current == renderers[i])
                continue;

            renderers[i].enabled = false;
        }

        oldDir = playerController.direction;
        oldIndex = itemIndex;
        ground = playerController.grounded;
    }

    //checks to see if UseItem button has been pressed and sets appropriate boolean values to be used in other scripts
    public void UseItem()
    {
        //button pushed
        if (Input.GetButtonDown(IM.useItem))
        {
            switch (itemIndex)
            {
                case 0:
                    break;
                case 1:
                    deployUmbrella = true;
                    break;
            }
        }

        //button released
        if (Input.GetButtonUp(IM.useItem) || Input.GetButtonDown(IM.swap))
        {
            switch (itemIndex)
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
        if (itemIndex == 1 && deployUmbrella == true)
        {
            //spriteRenderer.sprite = itemAlt[0];
            renderers[0].sprite = itemAlt[0];
        }
        else if (itemIndex == 1 && deployUmbrella == false)
        {
            //spriteRenderer.sprite = items[1];
            renderers[0].sprite = items[0];
        }
    }

    
}

