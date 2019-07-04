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
    PlayerController playerController;
    WorldSwitcher worldSwitcher;
    GameObject player;

    int hitTimer = 0;
    float durationTimer = 0;
    bool bodyFrozen = false;
    int worldNum;

    //contact filter stuff
    int g1;
    int g2;
    int g3;
    int g4;
    int finalMask;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerBody = player.GetComponent<Rigidbody2D>();
        worldSwitcher = player.GetComponentInChildren<WorldSwitcher>();
        worldNum = worldSwitcher.activeWorldNum;
        SetLayerMasks();
        //Debug.Log(playerBody.velocity.x + " " + playerBody.velocity.y);

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

        //Debug.Log(body.velocity.x + " " + body.velocity.y);
        StartCoroutine(FlashBomb());
    }

    private void Update()
    {

        RaycastHit2D hit = Physics2D.Raycast(new Vector3(boxCollider.transform.position.x, boxCollider.transform.position.y - (boxCollider.size.y / 2), boxCollider.transform.position.z),
            Vector2.down, 0.075f, finalMask);

        //Debug.DrawRay(new Vector3(boxCollider.transform.position.x, boxCollider.transform.position.y - (boxCollider.size.y / 2), boxCollider.transform.position.z), Vector2.down);

        //if (hit) { Debug.Log("bomb Hit"); }

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

        if (worldSwitcher.activeWorldNum != worldNum)
        {
            //Debug.Log("Unfreezing constraints");
            body.constraints &= ~RigidbodyConstraints2D.FreezePositionX;
            body.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
            worldNum = worldSwitcher.activeWorldNum;
            hitTimer = 0;
            bodyFrozen = false;
        }
        durationTimer += Time.deltaTime;

        if(durationTimer >= 3)
        {
            ExplosionDamage();
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

    private void ExplosionDamage()
    {
        Collider2D[] results = new Collider2D[10];
        ContactFilter2D cf = new ContactFilter2D();
        cf.SetLayerMask(LayerMask.GetMask("Enemy"));
        cf.SetLayerMask(LayerMask.GetMask("EnemyB"));
        Physics2D.OverlapCircle(transform.position, 1f, cf, results);

        for(int i = 0; i != results.Length; ++i)
        {
            if(results[i] == null)
            {
                break;
            }
            float offset = Vector2.SqrMagnitude(transform.position - results[i].transform.position);
            float damage = 100 / (offset * 3f);

            //Debug.Log(offset);
            //Debug.Log(damage);

            EnemyBase e = results[i].GetComponent<EnemyBase>();
            e.health -= (int)damage;
        }
    }

    private void SetLayerMasks()
    {
        g1 = 1 << LayerMask.NameToLayer("Ground1");
        g2 = 1 << LayerMask.NameToLayer("Ground2");
        g3 = 1 << LayerMask.NameToLayer("Ground3");
        g4 = 1 << LayerMask.NameToLayer("Ground4");
        finalMask = g1 | g2 | g3 | g4;

        //Debug.Log(finalMask);
    }
}
