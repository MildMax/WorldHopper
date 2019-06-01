﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEntryScript : MonoBehaviour
{
    PlayerController playerController;

    private void Awake()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && gameObject.tag != "CollInactive")
        {
            playerController.inWater = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && gameObject.tag != "CollInactive")
        {
            playerController.inWater = true;
        }
        if(collision.gameObject.tag == "Player" && gameObject.tag == "CollInactive")
        {
            playerController.inWater = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && gameObject.tag != "CollInactive")
        {
            playerController.inWater = false;
        }
    }
}
