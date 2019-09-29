using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillOnLoadComposite : KillOnLoad
{
    Collider2D[] colls;
    MinMax[] minMax;

    protected override void Awake()
    {
        GetColls();
        minMax = CreateInnerBox(colls);
        playerDims = GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider2D>().size;
    }

    protected override void FixedUpdate()
    {
        CheckEnabled();
    }

    private void GetColls()
    {
        colls = GetComponentsInChildren<Collider2D>();
    }

    protected MinMax[] CreateInnerBox(Collider2D[] co)
    {
        MinMax[] mm = new MinMax[co.Length];

        for(int i = 0; i != co.Length; ++i)
        {
            mm[i].maxVal = new Vector2(
            co[i].transform.position.x + co[i].bounds.extents.x - (playerDims.x / 4),
            co[i].transform.position.y + co[i].bounds.extents.y - (playerDims.y / 4));

            mm[i].minVal = new Vector2(
                co[i].transform.position.x - co[i].bounds.extents.x + (playerDims.x / 4),
                co[i].transform.position.y - co[i].bounds.extents.y + (playerDims.y / 4));
        }

        return mm;
    }

    private bool CheckAllMinMax()
    {
        bool doBreak = false;

        for(int i = 0; i != minMax.Length; ++i)
        {
            Collider2D overlaps = Physics2D.OverlapBox(colls[i].transform.position, colls[i].bounds.size, 0f, LayerMask.GetMask("Player"));

            //Debug.Log(overlaps == null);
            //Debug.Log(overlaps.name);

            if (overlaps != null)
            {
                doBreak = CheckPlayerProx(overlaps, i);
            }

            if (doBreak) break;
        }

        return doBreak;
    }

    private bool CheckPlayerProx(Collider2D o, int i)
    {
        bool isClose = false;

        if (
            o.transform.position.x < minMax[i].maxVal.x &&
            o.transform.position.x > minMax[i].minVal.x &&
            o.transform.position.y < minMax[i].maxVal.y &&
            o.transform.position.y > minMax[i].minVal.y
            )
        {
            isClose = true;
        }

        return isClose;
    }

    private void KillPlayer()
    {
        if (colls[0].isTrigger == false && isEnabled == false)
        {
            if (CheckAllMinMax())
            {
                PlayerController p = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
                p.boxCollider.enabled = false;
                p.health = 0;

            }
        }
    }
}

public class MinMax
{
    public Vector2 minVal;
    public Vector2 maxVal;
}
