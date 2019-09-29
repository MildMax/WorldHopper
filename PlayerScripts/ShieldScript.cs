using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour {

    ItemSwitcherAlt itemSwitcherAlt;
    BoxCollider2D[] boxCollider = new BoxCollider2D[2];
    PlayerController playerController;

    private void Awake()
    {
        itemSwitcherAlt = GetComponentInParent<ItemSwitcherAlt>();
        boxCollider = GetComponentsInChildren<BoxCollider2D>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        //Debug.Log(playerController == null);
    }

    //private void Update()
    //{
    //    EnableShield();
    //}

    public void EnableShield()
    {
        //Debug.Log("enable shield being called");
        if (itemSwitcherAlt.itemIndex == 4 && playerController.direction > 0)
        {
            //Debug.Log("Enabling shield right");
            boxCollider[1].enabled = false;
            boxCollider[0].enabled = true; 
        }
        else if(itemSwitcherAlt.itemIndex == 4 && playerController.direction < 0)
        {
            //Debug.Log("Enabling shield left");
            boxCollider[0].enabled = false;
            boxCollider[1].enabled = true;
        }
        else
        {
            for(int i = 0; i != boxCollider.Length; ++i)
            {
                boxCollider[i].enabled = false;
            }
        }
    }
}
