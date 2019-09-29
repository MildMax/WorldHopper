using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostTriggerScript : EnemyBase
{
    [HideInInspector]
    public bool inVicinity = false;

    [HideInInspector]
    public bool destroyThis = false;

    GhostScript gS;
    WorldSwitcher wS;

    private void Awake()
    {
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
        gS = GetComponentInChildren<GhostScript>();
    }

    private void Update()
    {
        if(destroyThis == true)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && wS.activeWorldNum == gS.worldNum)
        {
            inVicinity = true;
        }
    }
}
