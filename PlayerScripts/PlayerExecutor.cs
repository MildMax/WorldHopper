using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerExecutor : MonoBehaviour
{
    //in gameObject
    PlayerController playerController;
    PlayerAnimationScript playerAnimationScript;
    PlayerLightScript playerLightScript;

    //in children
    //ItemSwitcher itemSwitcher;
    ItemSwitcherAlt itemSwitcherAlt;
    WorldSwitcher worldSwitcher;

    //children of ItemSwitcher
    Shoot shoot;
    ThrowBomb throwBomb;
    ShieldScript shieldScript;

    // Start is called before the first frame update
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        playerAnimationScript = GetComponent<PlayerAnimationScript>();
        playerLightScript = GetComponent<PlayerLightScript>();

        //itemSwitcher = GetComponentInChildren<ItemSwitcher>();
        itemSwitcherAlt = GetComponentInChildren<ItemSwitcherAlt>();
        worldSwitcher = GetComponentInChildren<WorldSwitcher>();

        shoot = GetComponentInChildren<Shoot>();
        throwBomb = GetComponentInChildren<ThrowBomb>();
        shieldScript = GetComponentInChildren<ShieldScript>();
    }

    private void FixedUpdate()
    {
        WorldSwitcherFU();
        PlayerControllerFU();
        ShootFU();
        ThrowBombFU();
        //ItemSwitcherFU();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerControllerU();
        PlayerAnimationScriptU();
        PlayerLightScriptU();

        //ItemSwitcherU();
        ItemSwitcherAltU();
        WorldSwitcherU();

        ShootU();
        ThrowBombU();

        ShieldScriptU();
    }

    private void LateUpdate()
    {
        PlayerControllerLU();
        
    }

    //player controller

    private void PlayerControllerFU()
    {
        playerController.DoPhysics();
    }
    
    private void PlayerControllerU()
    {
        //playerController.vel = playerController.body.velocity;
        playerController.CheckKeyInput();
        if (playerAnimationScript.hurt == false)
        {
            playerController.PerformKeyPresses();
        }

        playerController.CheckDirection();
        playerController.Swim();
        playerController.groundedFrames++;
    }

    private void PlayerControllerLU()
    {
        //playerController.transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.z);
        playerController.KeepGroundedLU();
        playerController.NoXMovement();
    }

    //player animation script

    private void PlayerAnimationScriptU()
    {
        playerAnimationScript.CheckRun();
        playerAnimationScript.CheckDirection();
        playerAnimationScript.CheckAnimationState();
    }

    //player light script

    private void PlayerLightScriptU()
    {
        if (playerLightScript.itemSwitcherAlt.itemIndex == 5 && playerLightScript.worldLight.activeSelf == false)
        {
            playerLightScript.torchLight.enabled = true;
        }
        else
        {
            playerLightScript.torchLight.enabled = false;
        }
    }

    // item switcher

    //private void ItemSwitcherU()
    //{
    //    itemSwitcher.Switcher();
    //    itemSwitcher.UseItem();
    //    itemSwitcher.DisplayOpenUmbrella();
    //}

    //private void ItemSwitcherFU()
    //{
    //    itemSwitcher.StartSwitcher();
    //    itemSwitcher.AdjustRaygunForJump();
    //    itemSwitcher.AdjustItemsForDash();
    //}

    //item switcher alt

    private void ItemSwitcherAltU()
    {
        itemSwitcherAlt.IncrementIndex();
        itemSwitcherAlt.SetRenderer();
        itemSwitcherAlt.UseItem();
        itemSwitcherAlt.DisplayOpenUmbrella();
    }

    //world switcher
    private void WorldSwitcherU()
    {
        worldSwitcher.Preview();
    }

    private void WorldSwitcherFU()
    {
        worldSwitcher.Switcher();
    }

    //shoot

    private void ShootU()
    {
        shoot.GetButtonPress();
    }

    private void ShootFU()
    {
        //shoot.MoveShootPoint();
        shoot.ShootRaygun();
    }

    //throwBomb
    private void ThrowBombU()
    {
        throwBomb.GetButtonPress();
    }

    private void ThrowBombFU()
    {
        throwBomb.Throw();
    }

    //shield script
    private void ShieldScriptU()
    {
        shieldScript.EnableShield();
    }
}
