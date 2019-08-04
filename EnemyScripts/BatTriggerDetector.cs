using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatTriggerDetector : EnemyBase
{
    [HideInInspector]
    public bool inVicinity = false;

    [HideInInspector]
    public bool destroyThis = false;

    private void Update()
    {
        if (destroyThis == true)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inVicinity = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inVicinity = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            inVicinity = false;
        }
    }
}
