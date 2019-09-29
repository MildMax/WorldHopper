using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnLoad : MonoBehaviour {

    //this script will kill the player if they load a world where they overlap with one of the cells
    //
    //there are 2 main issues to solve here:
    // 1)   the size parameter passed to overlapBox will change based on the type of block, so find a way
    //      to determine the size and rotation of a block from its transform or something, so it stays consistent
    // 2)   OverlapBox uses a raycast which can be costly, try building a couple worlds and switching between them,
    //      see how much overhead it takes up. Two options: load and unload world based on whether its not in camera view
    //      to reduce the number of objects that are being called each time the world switches, or just find a less costly
    //      method of checking the players position in relation to the world. Depends on the tests. Lag bad. No want lag.

    protected bool isEnabled;

    int layer;

    Transform[] worldBoxPos;
    Transform pos;
    Vector2 size;
    Collider2D coll;

    Vector2 maxVal;
    Vector2 minVal;
    protected Vector2 playerDims;

    protected virtual void Awake()
    {
        coll = GetComponent<Collider2D>();
        size = (Vector2)coll.bounds.size;
        playerDims = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().size;
        pos = coll.transform;
        CreateInnerBox();
    }

    protected virtual void FixedUpdate()
    {
        DestroyOnColliderEnabled();
        CheckEnabled();
    }

    protected virtual void DestroyOnColliderEnabled()
    {
        if(coll.isTrigger == false && isEnabled == false)
        {     
            Collider2D overlaps = Physics2D.OverlapBox(pos.position, size, 0f, LayerMask.GetMask("Player"));

            //Debug.Log(overlaps == null);
            //Debug.Log(overlaps.name);

            bool isClose = false;

            if (overlaps != null)
            {
                isClose = CheckPlayerProx(overlaps);
            }

            if (overlaps != null && overlaps.name == "Player" && isClose == true)
            {
                PlayerController p = overlaps.gameObject.GetComponent<PlayerController>();
                p.boxCollider.enabled = false;
                p.health = 0;
                //Debug.Log("Killed by " + this.gameObject.transform.parent.name);
            }
            
        }
    }

    protected bool CheckPlayerProx(Collider2D o)
    {
        bool isClose = false;

        if(
            o.transform.position.x < maxVal.x &&
            o.transform.position.x > minVal.x &&
            o.transform.position.y < maxVal.y &&
            o.transform.position.y > minVal.y
            )
        {
            isClose = true;
        }

        return isClose;
    }

    protected void CheckEnabled()
    {
        if(isEnabled == false && coll.isTrigger == false)
        {
            isEnabled = true;
        }
        else if(isEnabled == true && coll.isTrigger == true)
        {
            isEnabled = false;
        }
    }

    protected void CreateInnerBox()
    {
        maxVal = new Vector2(
            coll.transform.position.x + coll.bounds.extents.x - (playerDims.x / 4),
            coll.transform.position.y + coll.bounds.extents.y - (playerDims.y / 4));

        minVal = new Vector2(
            coll.transform.position.x - coll.bounds.extents.x + (playerDims.x / 4),
            coll.transform.position.y - coll.bounds.extents.y + (playerDims.y / 4));
    }

}
