using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int health;
    //[HideInInspector]
    public string hash;

    //used in shield script -- ONLY USED BY WALKING ENEMIES -- STATIONARY ENEMIES HAVE NO USE FOR THIS
    //IN RETROSPECT I PROBABLY SHOULD HAVE CREATED TWO MORE DERIVED CLASSES FROM THIS ONE FOR WALKING AND STATIONARY
    [HideInInspector]
    public bool changeDirection = false;

    //Finding layer based on hash
    protected string layerString;

    protected string FindLayer(string h)
    {
        string r = null;

        if(h.Contains("W1"))
        {
            r = "1";
        }
        else if (h.Contains("W2"))
        {
            r = "2";
        }
        else if (h.Contains("W3"))
        {
            r = "3";
        }
        else if (h.Contains("W4"))
        {
            r = "4";
        }

        if(r != null)
        {
            return r;
        }
        else
        {
            Debug.Log("Cannot detect layer based on hash for " + gameObject.name + " -- returned null");
            return null;
        }
    }

    protected void RetryHash()
    {
        if(layerString == null)
        {
            layerString = FindLayer(hash);
        }
    }
}
