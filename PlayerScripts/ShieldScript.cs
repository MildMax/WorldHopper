using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldScript : MonoBehaviour {

    ItemSwitcher itemSwitcher;
    BoxCollider2D boxCollider;

    private void Awake()
    {
        itemSwitcher = GetComponentInParent<ItemSwitcher>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    //private void Update()
    //{
    //    EnableShield();
    //}

    public void EnableShield()
    {
        if(itemSwitcher.itemIndex == 4)
        {
            boxCollider.enabled = true;
        }
        else
        {
            boxCollider.enabled = false;
        }
    }

    //private void FlipCollider()
    //{
    //    if(playerController.direction > 0)
    //    {
    //        boxCollider.transform.position = new Vector2(transform.position.x + offset, transform.position.y);
    //    }
    //    else if(playerController.direction < 0)
    //    {
    //        boxCollider.transform.position = new Vector2(transform.position.x - offset, transform.position.y);
    //    }
    //}
}
