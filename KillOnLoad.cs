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

    Transform[] worldBoxPos;
    Vector2 size = new Vector2(0.7f, 0.7f);

    private void Awake()
    {
        worldBoxPos = GetComponentsInChildren<Transform>();
    }

    private void OnEnable()
    {

        foreach (Transform i in worldBoxPos)
        {
            Collider2D overlaps = Physics2D.OverlapBox(i.position, size, 0f, LayerMask.GetMask("Default"));

            if (overlaps != null && overlaps.name == "Player")
            {
                Destroy(overlaps.gameObject);
            }
        }
    }

}
