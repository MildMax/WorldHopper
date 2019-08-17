using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSwitcher : MonoBehaviour {

    List<GameObject> worlds = new List<GameObject>();

    //for Switcher() and SetActiveWorld()
    Collider2D[][] worldColliders = { null, null, null, null };

    //for Preview()
    SpriteRenderer[][] worldRenderers = { null, null, null, null };
    Color opaque = new Color(1, 1, 1, 0.5f);
    Color solid = new Color(1, 1, 1, 1); 

    SpriteRenderer[][] BGRenderers = { null, null, null, null };

    //GameObject[] hardWorlds = { null, null, null, null };
    //GameObject[] softWorlds = { null, null, null, null };

    SpriteRenderer[][] enemyRenderers = { null, null, null, null };
    //Collider2D[][] enemyColliders = { null, null, null, null };

    Dictionary<string, Collider2D>[] enemyColliders =
    {
        new Dictionary<string, Collider2D>(),
        new Dictionary<string, Collider2D>(),
        new Dictionary<string, Collider2D>(),
        new Dictionary<string, Collider2D>()
    };

    GameObject activeWorld;
    public int activeWorldNum;

    InputManager IM;

    //for reseting colliders after destroy enemy and world switch
    [HideInInspector]
    public bool enemyDestroyed = false;

    //for resetting preview buttons
    bool p1 = false;
    bool p2 = false;
    bool p3 = false;
    bool p4 = false;

    //public float x;
    //public float y;

    private void Awake()
    {
        GetWorlds();
        RemoveBGRenderer();
        SetLevel();
        SetActiveWorld(activeWorldNum);
        //SetInitialBG();
        IM = GameObject.FindGameObjectWithTag("GameController").GetComponent<InputManager>();
    }

    //private void Update()
    //{
    //    Switcher();
    //    Preview();
    //}

    private void GetWorlds()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("Worlds");
        foreach(Transform child in temp.transform)
        {
            worlds.Add(child.gameObject);
        }

        if(worlds.Count != 4)
        {
            for(int i = worlds.Count; i != 4; ++i)
            {
                worlds.Add(null);
            }
        }
        
        for(int i = 0; i != worlds.Count; ++i)
        {

            if (worlds[i] != null)
            {
                worldRenderers[i] = worlds[i].GetComponentsInChildren<SpriteRenderer>();
                worldColliders[i] = worlds[i].GetComponentsInChildren<Collider2D>();
            }

            //Debug.Log(worlds[i].name);
        }

        //Debug.Log("worldColliders[0] length before removal: " + worldColliders[0].Length);
        //Debug.Log("worldRenderers[0] length before removal: " + worldRenderers[0].Length);

        for (int i = 0; i != worldRenderers.Length; ++i)
        {
            List<SpriteRenderer> enemyRend = new List<SpriteRenderer>();

            if (worldRenderers[i] != null)
            {
                for (int j = 0; j != worldRenderers[i].Length; ++j)
                {
                    if (worldRenderers[i][j].gameObject.layer == LayerMask.NameToLayer("Enemy") || worldRenderers[i][j].gameObject.layer == LayerMask.NameToLayer("EnemyB"))
                    {
                        if (worldRenderers[i][j].gameObject.tag != "Ghost")
                        {
                            enemyRend.Add(worldRenderers[i][j]);
                        }
                        worldRenderers[i][j] = null;
                    }
                }

                enemyRenderers[i] = ConvertList(enemyRend);
                worldRenderers[i] = ResizeArray(worldRenderers[i]);
            }
        }

        for (int i = 0; i != worldColliders.Length; ++i)
        {
            if (worldColliders[i] != null)
            {
                for (int j = 0; j != worldColliders[i].Length; ++j)
                {
                    //Debug.Log("Layermask.GetMask(\"Enemy\"): " + (1 << 9) + "-- worldColliders[i][j].gameObject.layer: " + worldColliders[i][j].gameObject.layer);
                    if (worldColliders[i][j].gameObject.layer == LayerMask.NameToLayer("Enemy") || worldColliders[i][j].gameObject.layer == LayerMask.NameToLayer("EnemyB"))
                    {
                        if (worldColliders[i][j].gameObject.tag != "Ghost")
                        {
                            string t = "W" + (i + 1) + "-" + j;
                            enemyColliders[i].Add(t, worldColliders[i][j]);
                            worldColliders[i][j].gameObject.GetComponent<EnemyBase>().hash = t;
                        }
                        worldColliders[i][j] = null;
                    }
                    else if(worldColliders[i][j].gameObject.tag == "Collectable")
                    {
                        worldColliders[i][j].gameObject.GetComponent<CollectableBase>().worldNum = i;
                        worldColliders[i][j] = null;
                    }
                    else if (worldColliders[i][j].tag == "WaterCollider")
                    {
                        worldColliders[i][j] = null;
                    }
                }

                //Debug.Log(enemyColliders[i].Count);
                worldColliders[i] = ResizeArray(worldColliders[i]);
            }
        }

        //Debug.Log("worldColliders[0] length after removal: " + worldColliders[0].Length);
        //Debug.Log("enemyColliders[0] length: " + enemyColliders[0].Length);

        //Debug.Log("worldRenderers[0] length after removal: " + worldRenderers[0].Length);
        //Debug.Log("enemyRenderers[0] length: " + enemyRenderers[0].Length);
    }

    public void Switcher()
    {
        //ResizeEnemyArray();

        if (Mathf.Abs(Input.GetAxis(IM.changeX)) > Mathf.Abs(Input.GetAxis(IM.changeY)))
        {
            if (Input.GetAxisRaw(IM.changeX) < 0 && activeWorld != worlds[0] && worlds[0] != null)
            {
                SetActiveWorld(0);
            }
            else if (Input.GetAxisRaw(IM.changeX) > 0 && activeWorld != worlds[2] && worlds[2] != null)
            {
                SetActiveWorld(2);
            }
        }
        else
        {
            if (Input.GetAxisRaw(IM.changeY) > 0 && activeWorld != worlds[1] && worlds[1] != null)
            {
                SetActiveWorld(1);
            }
            else if (Input.GetAxisRaw(IM.changeY) < 0 && activeWorld != worlds[3] && worlds[3] != null)
            {
                SetActiveWorld(3);
            }
        }
    }

    private void SetActiveWorld(int i)
    {
        if (i != activeWorldNum)
        {
            SwitchBG(BGRenderers[i], BGRenderers[activeWorldNum]);
        }

        for (int j = 0; j != worldColliders[i].Length; ++j)
        {
            
            //worldColliders[i][j].tag = "CollActive";
            if (worldColliders[i][j].gameObject.layer != LayerMask.NameToLayer("Water"))
            {
                worldColliders[i][j].isTrigger = false;
            }
        }

        foreach(KeyValuePair<string, Collider2D> j in enemyColliders[i])
        {
            j.Value.enabled = true;
        }

        if (worlds[i] != worlds[activeWorldNum])
        {
            for (int j = 0; j != worldColliders[activeWorldNum].Length; ++j)
            {
                if (worldColliders[activeWorldNum][j] != null)
                {
                    //worldColliders[activeWorldNum][j].tag = "CollInactive";
                    if (worldColliders[activeWorldNum][j].gameObject.layer != LayerMask.NameToLayer("Water"))
                    {
                        worldColliders[activeWorldNum][j].isTrigger = true;
                    }
                }
            }

            foreach(KeyValuePair<string, Collider2D> j in enemyColliders[activeWorldNum])
            {
                j.Value.enabled = false;
                //Debug.Log(j.Value.gameObject.name);
                //Debug.Log(j.Value.enabled == true);
            }
        }


        for (int k = 0; k != worldRenderers[activeWorldNum].Length; ++k)
        {
            if (worldRenderers[activeWorldNum][k] != null)
            {
                worldRenderers[activeWorldNum][k].enabled = false;
            }
        }

        for(int k = 0; k != enemyRenderers[activeWorldNum].Length; ++k)
        {
            if(enemyRenderers[activeWorldNum][k] != null)
            {
                enemyRenderers[activeWorldNum][k].enabled = false;
            }
        }

        for (int k = 0; k != worldRenderers[i].Length; ++k)
        {
            if (worldRenderers[i][k] != null)
            {
                worldRenderers[i][k].enabled = true;
                worldRenderers[i][k].color = solid;
            }
        }

        for (int k = 0; k != enemyRenderers[i].Length; ++k)
        {
            if (enemyRenderers[i][k] != null)
            {
                enemyRenderers[i][k].enabled = true;
                enemyRenderers[i][k].color = solid;
            }
        }

        activeWorld = worlds[i];
        activeWorldNum = i;
    }

    private void SwitchBG(SpriteRenderer[] next, SpriteRenderer[] prev)
    {
        for (int i = 0; i != next.Length; ++i)
        {
            //next[i].sortingOrder = -1;
            next[i].enabled = true;
        }

        for (int i = 0; i != prev.Length; ++i)
        {
            prev[i].enabled = false;
            //prev[i].sortingOrder = -2;
        }
    }

    public void Preview()
    {
        bool[] buttonsPressed = { false, false, false, false };

        if (Input.GetAxisRaw(IM.previewX) < 0 && worldRenderers[0] != null && activeWorld != worlds[0])
        {
            for (int i = 0; i != worldRenderers[0].Length; ++i)
            {
                if (worldRenderers[0][i] != null)
                {
                    if (p1 == false)
                    {
                        worldRenderers[0][i].sortingOrder += 1;
                        worldRenderers[0][i].enabled = true;
                        worldRenderers[0][i].color = opaque;
                    }
                    buttonsPressed[0] = true;
                }
            }
            if(p1 == false) p1 = true;
        }
        else if (Input.GetAxisRaw(IM.previewX) > 0 && worldRenderers[2] != null && activeWorld != worlds[2])
        {
            for (int i = 0; i != worldRenderers[2].Length; ++i)
            {
                if (worldRenderers[2][i] != null)
                {
                    if (p3 == false)
                    {
                        worldRenderers[2][i].sortingOrder += 1;
                        worldRenderers[2][i].enabled = true;
                        worldRenderers[2][i].color = opaque;
                    }
                    buttonsPressed[2] = true;
                }
            }
            if(p3 == false) p3 = true;
        }
        else if (Input.GetAxisRaw(IM.previewY) > 0 && worldRenderers[1] != null && activeWorld != worlds[1])
        {
            for (int i = 0; i != worldRenderers[1].Length; ++i)
            {
                if (worldRenderers[1][i] != null)
                {
                    if (p2 == false)
                    {
                        //Debug.Log("Done bein' pressed");
                        worldRenderers[1][i].sortingOrder += 1;
                        //Debug.Log(worldRenderers[1][i].sortingOrder);
                        worldRenderers[1][i].enabled = true;
                        worldRenderers[1][i].color = opaque;
                    }
                    buttonsPressed[1] = true;
                }
            }
            if (p2 == false) p2 = true;
        }
        else if (Input.GetAxisRaw(IM.previewY) < 0 && worldRenderers[3] != null && activeWorld != worlds[3])
        {
            for (int i = 0; i != worldRenderers[3].Length; ++i)
            {
                if (worldRenderers[3][i] != null)
                {
                    if (p4 == false)
                    {
                        worldRenderers[3][i].sortingOrder += 1;
                        worldRenderers[3][i].enabled = true;
                        worldRenderers[3][i].color = opaque;
                    }
                    buttonsPressed[3] = true;
                }
            }

            if(p4 == false) p4 = true;
        }

        for(int i = 0; i != buttonsPressed.Length; ++i)
        {
            if (p1 == false && i == 0) continue;
            else if (p2 == false && i == 1) continue;
            else if (p3 == false && i == 2) continue;
            else if (p4 == false && i == 3) continue;

            //Debug.Log(worlds.Count);
            if(worlds[i] != null)
            {
                if (!buttonsPressed[i] && worlds[i] != activeWorld)
                {
                    for (int j = 0; j != worldRenderers[i].Length; ++j)
                    {
                        if (worldRenderers[i][j] != null)
                        {
                            worldRenderers[i][j].sortingOrder -= 1;
                            worldRenderers[i][j].color = solid;
                            worldRenderers[i][j].enabled = false;
                        }
                    }

                    //Debug.Log("Dun bein unpressed");

                    if (p1 == true) p1 = false;
                    if (p2 == true) p2 = false;
                    if (p3 == true) p3 = false;
                    if (p4 == true) p4 = false;
                }
            }
        }
    }

    private void SetLevel()
    {
        for(int i = 0; i != worldRenderers.Length; ++i)
        {
            if (worldRenderers[i] != null)
            {
                for (int j = 0; j != worldRenderers[i].Length; ++j)
                {
                    if (worldRenderers[i][j] != null)
                    {
                        worldRenderers[i][j].enabled = false;
                    }
                }
            }
        }

        for(int i = 0; i != worldColliders.Length; ++i)
        {
            if (worldRenderers[i] != null)
            {
                for (int j = 0; j != worldColliders[i].Length; ++j)
                {
                    
                    //worldColliders[i][j].tag = "CollInactive";
                    if (worldColliders[i][j].gameObject.layer != LayerMask.NameToLayer("Water"))
                    {
                        worldColliders[i][j].isTrigger = true; //change made here
                        if (worldColliders[i][j].gameObject.tag != "Wall")
                        {
                            string temp = "Ground" + (i + 1);
                            worldColliders[i][j].gameObject.layer = LayerMask.NameToLayer(temp);
                        }
                        else if(worldColliders[i][j].gameObject.tag == "Wall")
                        {
                            string temp = "Wall" + (i + 1);
                            worldColliders[i][j].gameObject.layer = LayerMask.NameToLayer(temp);
                        }
                    }
                }
            }
        }

        for(int i = 0; i != enemyRenderers.Length; ++i)
        {
            if(enemyRenderers[i] != null)
            {
                for(int j = 0; j != enemyRenderers[i].Length; ++j)
                {
                    if(enemyRenderers[i][j] != null)
                    {
                        enemyRenderers[i][j].enabled = false;
                    }
                }
            }
        }

        for (int i = 0; i != enemyColliders.Length; ++i)
        {
            if (enemyColliders[i] != null)
            {
                for (int j = 0; j != enemyColliders[i].Count; ++j)
                {
                    //if (enemyColliders[i][j] != null)
                    {
                        //enemyColliders[i][j].enabled = false;
                    }
                }

                foreach(KeyValuePair<string, Collider2D> j in enemyColliders[i])
                {
                    if(j.Value != null)
                    {
                        j.Value.enabled = false;
                    }
                }
            }
        }
    }

    private void SetInitialBG()
    {
        for(int i = 0; i != BGRenderers.Length; ++i)
        {
            if (BGRenderers[i] != null)
            {
                if (BGRenderers[i] == BGRenderers[activeWorldNum])
                {
                    for (int j = 0; j != BGRenderers[i].Length; ++j)
                    {
                        //BGRenderers[activeWorldNum][j].sortingOrder = -1;
                        BGRenderers[activeWorldNum][j].enabled = true;
                    }
                }
                else
                {
                    for (int j = 0; j != BGRenderers[i].Length; ++j)
                    {
                        //BGRenderers[i][j].sortingOrder = -2;
                        BGRenderers[i][j].enabled = false;
                    }
                }
            }
        }

        
    }

    private void RemoveBGRenderer()
    {
        for(int i = 0; i != worldRenderers.Length; ++i)
        {
            if (worldRenderers[i] != null)
            {

                List<SpriteRenderer> tempList = new List<SpriteRenderer>();

                for (int j = 0; j != worldRenderers[i].Length; ++j)
                {
                    if (worldRenderers[i][j].gameObject.tag == "BGRenderer")
                    {
                        tempList.Add(worldRenderers[i][j]);
                        //Debug.Log("renderer removed: " + worldRenderers[i][j].gameObject.name);
                        worldRenderers[i][j] = null;
                    }
                }

                BGRenderers[i] = ConvertList(tempList);
                worldRenderers[i] = ResizeArray(worldRenderers[i]);
            }
        }
    }

    public void DestroyEnemyValue(string v)
    {
        if(v.Contains("W1"))
        {
            enemyColliders[0].Remove(v);
            //Debug.Log(enemyColliders[0].Count);
        }
        else if (v.Contains("W2"))
        {
            enemyColliders[1].Remove(v);
        }
        else if (v.Contains("W3"))
        {
            enemyColliders[2].Remove(v);
        }
        else if (v.Contains("W4"))
        {
            enemyColliders[3].Remove(v);
        }
    }

    private T[] ResizeArray<T>(T[] t)
    {
        int len = 0;
        for(int i = 0; i != t.Length; ++i)
        {
            if(t[i] != null)
            {
                len += 1;
            }
        }

        //Debug.Log(len);

        T[] newT = new T[len];
        int index = 0;

        for(int i = 0; i != t.Length; ++i)
        {
            if(t[i] != null)
            {
                newT[index] = t[i];
                ++index;
            }
            
        }

        return newT;
    }

    private T[] ConvertList<T>(List<T> list)
    {
        int len = list.Count;
        T[] newT = new T[len];
        for(int i = 0; i != list.Count; ++i)
        {
            newT[i] = list[i];
        }
        return newT;
    }
    
}
