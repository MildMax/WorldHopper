using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationScript : MonoBehaviour {

    //set necessasry component variables, they pretty much speak for themselves
    PlayerController playerController;
    Animator animator;
    SpriteRenderer spriteRenderer;
    InputManager IM;

    public AnimationClip walk;

    //set in enemy collision script
    //used in PlayerController script
    [HideInInspector]
    public bool hurt = false;

    private float direction;

    //get component variables from the hierarchy
    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        IM = GetComponent<InputManager>();
    }

    //private void Update()
    //{
    //    CheckRun();
    //    CheckDirection();
    //    CheckAnimationState();
    //}

    //get the direction variables from the player component and use it to flip the character sprite
    //so the sprite's direction matches the direction the player is moving
    public void CheckDirection()
    {
        direction = playerController.direction;
        if(direction < 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    //use boolean values from playerController to set boolean values in animator controller
    public void CheckAnimationState()
    {
        //hurt
        if(hurt == true)
        {
            SetHurt();
        }
        //dash
        else if(playerController.noDashJump == true)
        {
            SetDash();
        }
        //jump
        else if(playerController.grounded == false || playerController.hasJumped == true)
        {
            SetJump();
        }
        //walk
        else if(Mathf.Abs(Input.GetAxis(IM.horizontal)) > 0)
        {
            SetWalk();
        }
        //idle
        else
        {
            SetFalse();
        }
    }

    //set all boolean values in animator to false, thus rendering the idle state
    private void SetFalse()
    {
        animator.SetBool("IsDashing", false);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsHurt", false);
        animator.SetBool("IsWalking", false);
    }

    private void SetDash()
    {
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsHurt", false);
        animator.SetBool("IsWalking", false);
        
        if(animator.GetBool("IsDashing") == false)
        {
            animator.SetBool("IsDashing", true);
        }
    }

    private void SetJump()
    {
        animator.SetBool("IsDashing", false);
        animator.SetBool("IsHurt", false);
        animator.SetBool("IsWalking", false);

        if (animator.GetBool("IsJumping") == false)
        {
            animator.SetBool("IsJumping", true);
        }
    }

    private void SetWalk()
    {
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsHurt", false);
        animator.SetBool("IsDashing", false);

        if (animator.GetBool("IsWalking") == false)
        {
            animator.SetBool("IsWalking", true);
        }
    }

    private void SetHurt()
    {
        animator.SetBool("IsDashing", false);
        animator.SetBool("IsJumping", false);
        animator.SetBool("IsHurt", true);
        animator.SetBool("IsWalking", false);
        //StartCoroutine(HurtWait());
    }

    //private IEnumerator HurtWait()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    hurt = false;
    //}

    //increases the framerate of the animation while the player is running
    //called in Update() method
    public void CheckRun()
    {
        if(animator.GetBool("IsWalking") == true && Input.GetAxisRaw(IM.run) > 0)
        {
            animator.speed = 1.5f;
        }
        else
        {
            animator.speed = 1;
        }
    }

}
