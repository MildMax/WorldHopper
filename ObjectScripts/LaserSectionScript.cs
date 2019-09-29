using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSectionScript : MonoBehaviour
{
    [HideInInspector]
    public int worldNum;

    [HideInInspector]
    public WorldSwitcher wS;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && worldNum == wS.activeWorldNum)
        {
            collision.gameObject.GetComponent<PlayerController>().GetHurt(transform.position);
        }
    }
}
