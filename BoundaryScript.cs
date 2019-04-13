using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundaryScript : MonoBehaviour {

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Shot")
        {
            //Debug.Log("exit boundary");
            Destroy(collision.gameObject);
        }
    }
}
