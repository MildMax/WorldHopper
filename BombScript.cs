using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour {

    public Vector2 bombVelocity;
    public Sprite[] bombSprites;

    Rigidbody2D body;
    Rigidbody2D playerBody;
    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;
    CircleCollider2D circleCollider;
    PlayerController playerController;

    int hitTimer = 0;
    float durationTimer = 0;
    bool bodyFrozen = false;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        //circleCollider = GetComponent<CircleCollider2D>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        playerBody = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();

        Debug.Log(playerBody.velocity.x + " " + playerBody.velocity.y);

        if (playerController.direction > 0)
        {
            //body.AddForce(bombVelocity + playerBody.velocity);
            body.velocity = bombVelocity + (playerBody.velocity/ 1.5f);
        }
        else if(playerController.direction < 0)
        {
            //body.AddForce(new Vector2(-bombVelocity.x, bombVelocity.y) + playerBody.velocity);
            body.velocity = new Vector2(-bombVelocity.x, bombVelocity.y) + (playerBody.velocity / 1.5f);
        }

        Debug.Log(body.velocity.x + " " + body.velocity.y);
        StartCoroutine(FlashBomb());
    }

    private void Update()
    {
        //if(bodyFrozen)
        //{
        //    body.constraints = RigidbodyConstraints2D.None;
        //}

        RaycastHit2D hit = Physics2D.Raycast(new Vector3(boxCollider.transform.position.x, boxCollider.transform.position.y - Mathf.Abs(boxCollider.size.y / 2 + 0.051f), boxCollider.transform.position.z),
            Vector2.down, 0.075f, LayerMask.GetMask("Ground"));

        //RaycastHit2D hit = Physics2D.Raycast(new Vector3(circleCollider.transform.position.x, circleCollider.transform.position.y - Mathf.Abs(circleCollider.radius + 0.1f), 
        //    circleCollider.transform.position.z), Vector2.down, 0.05f);

        //Debug.DrawLine(new Vector3(boxCollider.transform.position.x, boxCollider.transform.position.y - Mathf.Abs(boxCollider.size.y / 2 + 0.051f), boxCollider.transform.position.z),
        //    new Vector3(boxCollider.transform.position.x, boxCollider.transform.position.y - Mathf.Abs(boxCollider.size.y / 2 + 0.126f), boxCollider.transform.position.z),
        //     Color.red);

        //Debug.Log(hit.collider.name);

        if (!hit)
        {
            hitTimer = 0;
        }
        else
        {
            hitTimer += 1;
        }

        if (hitTimer >= 35 && bodyFrozen == false)
        {
            body.constraints = RigidbodyConstraints2D.FreezeAll;
            bodyFrozen = true;
        }
        durationTimer += Time.deltaTime;

        if(durationTimer >= 3)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator FlashBomb()
    {
        float duration = 1f;
        int pos = 0;

        while(true)
        {
            spriteRenderer.sprite = bombSprites[pos];
            if(pos == 0)
            {
                pos = 1;
            }
            else if(pos == 1)
            {
                pos = 0;
            }

            duration /= 1.35f;
            yield return new WaitForSeconds(duration);
        }
    }
}
