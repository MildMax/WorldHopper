﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotScript : MonoBehaviour {

    public int speed;

    [HideInInspector]
    public int damage = 25;

    Rigidbody2D body;
    PlayerController playerController;
    WorldSwitcher wS;

    string[] tags = { "Player", "PlayerChild", "Shot", "WaterCollider" };

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        if (playerController.direction > 0)
        {
            body.velocity = transform.right * speed;
        }
        else if (playerController.direction < 0)
        {
            body.velocity = -transform.right * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Collision layer: " + collision.gameObject.layer + "\nWater Layer: " + (LayerMask.GetMask("Water") >> 2));
        if (CheckObject(collision))
        {
            //Debug.Log("Collision enter");
            Destroy(gameObject);
        }
    }

    private bool CheckObject(Collider2D coll)
    {
        bool val = true;

        for(int i = 0; i != tags.Length; ++i)
        {
            if(coll.tag == tags[i])
            {
                val = false;
            }
        }

        if (coll.gameObject.layer == LayerMask.GetMask("Water") >> 2
            || coll.gameObject.layer == LayerMask.NameToLayer("Ground" + wS.activeWorldNum)
            || coll.gameObject.layer == LayerMask.NameToLayer("Wall" + wS.activeWorldNum))
        {
            val = false;
        }

        return val;
    }
}
