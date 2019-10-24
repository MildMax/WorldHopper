using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_CreateSpinners : TriggerEventBase
{
    public int worldNum;
    public GameObject spinner;

    WorldSwitcher wS;

    public Vector2[] spinnerSpawns;

    bool eventTriggered = false;

    private void Awake()
    {
        
    }

    private void Update()
    {
        if(triggerEvent && !eventTriggered)
        {
            CreateSpinners();
            eventTriggered = true;
        }
    }

    private void CreateSpinners()
    {
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();

        for (int i = 0; i != spinnerSpawns.Length; ++i)
        {
            GameObject g = Instantiate(spinner, spinnerSpawns[i],Quaternion.identity);
            g.transform.SetParent(GameObject.FindGameObjectWithTag("World2").transform);
            AddToWorldSwitcher(ref wS, ref g, worldNum);
        }
    }

    private void AddToWorldSwitcher(ref WorldSwitcher w, ref GameObject g, int worldNum)
    {
        SpriteRenderer[] newArr = new SpriteRenderer[w.enemyRenderers[worldNum].Length + 1];
        for (int i = 0; i != w.enemyRenderers[worldNum].Length; ++i)
        {
            newArr[i] = w.enemyRenderers[worldNum][i];
        }
        newArr[newArr.Length - 1] = g.GetComponent<SpriteRenderer>();
        w.enemyRenderers[worldNum] = newArr;
        w.enemyRenderers[worldNum][w.enemyRenderers[worldNum].Length - 1].enabled = true;

        string hash = "W" + worldNum + "-" + w.enemyColliders[worldNum].Count;
        g.GetComponent<EnemyBase>().hash = hash;
        w.enemyColliders[worldNum].Add(hash, g.GetComponent<Collider2D>());
        w.enemyColliders[worldNum][hash].enabled = true;
    }
}
