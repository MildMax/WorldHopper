using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotScript : MonoBehaviour {

    public int speed;

    Rigidbody2D body;
    PlayerController playerController;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (playerController.direction > 0)
        {
            body.velocity = transform.right * speed;
        }
        else if (playerController.direction < 0)
        {
            body.velocity = -transform.right * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Boundary" && collision.tag != "Player" && collision.tag != "PlayerChild")
        {
            //Debug.Log("Collision enter");
            Destroy(gameObject);
        }
    }
}
