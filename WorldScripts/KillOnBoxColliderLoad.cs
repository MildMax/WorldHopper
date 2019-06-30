using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnBoxColliderLoad : MonoBehaviour
{
    Collider2D[] colliders;
    bool colliderEnabled = false;
    Vector2 size = new Vector2(0.7f, 0.7f);

    private void Awake()
    {
        colliders = GetComponents<Collider2D>();
    }

    private void Update()
    {
        KillOnColliderEnable();
        CheckCollidersOn();
        CheckCollidersOff();
    }

    private void CheckCollidersOn()
    {
        for (int i = 0; i != colliders.Length; ++i)
        {
            if (colliderEnabled == false && colliders[i].isActiveAndEnabled == true)
            {
                colliderEnabled = true;
            }
        }
    }

    private void CheckCollidersOff()
    {
        for (int i = 0; i != colliders.Length; ++i)
        {
            if (colliderEnabled == true && colliders[i].isActiveAndEnabled == false)
            {
                colliderEnabled = false;
            }
        }
    }

    private void KillOnColliderEnable()
    {
        for (int i = 0; i != colliders.Length; ++i)
        {
            if(colliderEnabled == false && colliders[i].isActiveAndEnabled == true)
            {
                Collider2D overlaps = Physics2D.OverlapBox(colliders[i].transform.position, size, 0f, LayerMask.GetMask("Player"));

                if (overlaps != null && overlaps.name == "Player")
                {
                    Destroy(overlaps.gameObject);
                }

                //Debug.Log("Set to destroy");
            }
        }
    }
}
