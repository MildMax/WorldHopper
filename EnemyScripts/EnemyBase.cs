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
}
