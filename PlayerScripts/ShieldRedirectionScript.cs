using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldRedirectionScript : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision with shield collider");
        Debug.Log(collision.gameObject.layer.ToString());
        if(collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyBase>().changeDirection = true;
        }
    }
}
