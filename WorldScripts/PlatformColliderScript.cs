 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformColliderScript : MonoBehaviour
{
    GameObject player;
    BoxCollider2D coll;
    Collider2D playerColl;
    PlayerController playerController;

    [HideInInspector]
    public bool madeTrigger = false;

    public bool debugTrigger = false;

    float yPos;
    float playerYPos;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject;
        playerColl = player.GetComponent<Collider2D>();
        playerController = player.GetComponent<PlayerController>();
        coll = GetComponent<BoxCollider2D>();
        yPos = coll.transform.position.y;
        
    }

    private void Update()
    {
        if (debugTrigger)
        {
            MakeTrigger();
        }

        //TurnTriggerBackOn();
    }

    //public void MakeTrigger()
    //{
    //    playerYPos = player.transform.position.y;
    //    coll.enabled = false;
    //}

    //private void TurnTriggerBackOn()
    //{
    //    if (coll.enabled == false)
    //    {
    //        if (player.transform.position.y > playerYPos ||
    //            player.transform.position.y < yPos - playerColl.bounds.extents.y) coll.enabled = true;

    //    }
    //}

    public void MakeTrigger()
    {
        madeTrigger = true;
        coll.isTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (madeTrigger && collision.tag == "Player")
        {
            coll.isTrigger = false;
            madeTrigger = false;
            playerController.isInPlatform = false;

            debugTrigger = false;
        }
    }

}
