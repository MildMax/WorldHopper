using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseTriggerDetector : MonoBehaviour
{
    [HideInInspector]
    public bool inVicinity;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            inVicinity = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            inVicinity = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            inVicinity = false;
        }
    }
}
