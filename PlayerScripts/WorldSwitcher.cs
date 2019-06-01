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
    Collider2D[][] enemyColliders = {null, null, null, null};

    GameObject activeWorld;
    public int activeWorldNum;

    InputManager IM;

    //public float x;
    //public float y;

    private void Awake()
    {
        GetWorlds();
        RemoveBGRenderer();
        SetLevel();
        SetActiveWorld(activeWorldNum);
        SetInitialBG();
        IM = GetComponentInParent<InputManager>();
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
        
        for(int i = 0; i != worlds.Count; ++i)
        {
            worldRenderers[i] = worlds[i].GetComponentsInChildren<SpriteRenderer>();
            worldColliders[i] = worlds[i].GetComponentsInChildren<Collider2D>();

            //Debug.Log(worlds[i].name);
        }

        //Debug.Log("worldColliders[0] length before removal: " + worldColliders[0].Length);
        //Debug.Log("worldRenderers[0] length before removal: " + worldRenderers[0].Length);

        for (int i = 0; i != worldRenderers.Length; ++i)
        {
            List<SpriteRenderer> enemyRend = new List<SpriteRenderer>();

            for (int j = 0; j != worldRenderers[i].Length; ++j)
            {
                if(worldRenderers[i][j].gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    enemyRend.Add(worldRenderers[i][j]);
                    worldRenderers[i][j] = null;
                }
            }

            enemyRenderers[i] = ConvertList(enemyRend);
            worldRenderers[i] = ResizeArray(worldRenderers[i]);
        }

        for (int i = 0; i != worldColliders.Length; ++i)
        {
            List<Collider2D> enemyRend = new List<Collider2D>();

            for (int j = 0; j != worldColliders[i].Length; ++j)
            {
                //Debug.Log("Layermask.GetMask(\"Enemy\"): " + (1 << 9) + "-- worldColliders[i][j].gameObject.layer: " + worldColliders[i][j].gameObject.layer);
                if (worldColliders[i][j].gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    enemyRend.Add(worldColliders[i][j]);
                    worldColliders[i][j] = null;
                }
            }

            enemyColliders[i] = ConvertList(enemyRend);
            worldColliders[i] = ResizeArray(worldColliders[i]);
        }

        //Debug.Log("worldColliders[0] length after removal: " + worldColliders[0].Length);
        //Debug.Log("enemyColliders[0] length: " + enemyColliders[0].Length);

        //Debug.Log("worldRenderers[0] length after removal: " + worldRenderers[0].Length);
        //Debug.Log("enemyRenderers[0] length: " + enemyRenderers[0].Length);

        //while (worlds.Count != 4)
        //{
        //    worlds.Add(null);
        //}
    }

    public void Switcher()
    {
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
            worldColliders[i][j].isTrigger = false;
            worldColliders[i][j].tag = "CollActive";
            worldColliders[i][j].gameObject.layer = LayerMask.NameToLayer("Ground");
        }

        for(int j = 0; j != enemyColliders[i].Length; ++j)
        {
            enemyColliders[i][j].enabled = true;
        }

        if (worlds[i] != worlds[activeWorldNum])
        {
            for (int j = 0; j != worldColliders[activeWorldNum].Length; ++j)
            {
                worldColliders[activeWorldNum][j].isTrigger = true;
                worldColliders[activeWorldNum][j].tag = "CollInactive";
                worldColliders[activeWorldNum][j].gameObject.layer = LayerMask.NameToLayer("GroundInactive");
            }

            for(int j = 0; j != enemyColliders[activeWorldNum].Length; ++j)
            {
                enemyColliders[activeWorldNum][j].enabled = false;
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
            next[i].sortingOrder = -1;
            next[i].enabled = true;
        }

        for (int i = 0; i != prev.Length; ++i)
        {
            prev[i].enabled = false;
            prev[i].sortingOrder = -2;
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
                    worldRenderers[0][i].sortingOrder = 1;
                    worldRenderers[0][i].enabled = true;
                    worldRenderers[0][i].color = opaque;
                    buttonsPressed[0] = true;
                }
            }
        }
        else if (Input.GetAxisRaw(IM.previewX) > 0 && worldRenderers[2] != null && activeWorld != worlds[2])
        {
            for (int i = 0; i != worldRenderers[2].Length; ++i)
            {
                if (worldRenderers[2][i] != null)
                {
                    worldRenderers[2][i].sortingOrder = 1;
                    worldRenderers[2][i].enabled = true;
                    worldRenderers[2][i].color = opaque;
                    buttonsPressed[2] = true;
                }
            }
        }
        else if (Input.GetAxisRaw(IM.previewY) > 0 && worldRenderers[1] != null && activeWorld != worlds[1])
        {
            for (int i = 0; i != worldRenderers[1].Length; ++i)
            {
                if (worldRenderers[1][i] != null)
                {
                    worldRenderers[1][i].sortingOrder = 1;
                    worldRenderers[1][i].enabled = true;
                    worldRenderers[1][i].color = opaque;
                    buttonsPressed[1] = true;
                }
            }
        }
        else if (Input.GetAxisRaw(IM.previewY) < 0 && worldRenderers[3] != null && activeWorld != worlds[3])
        {
            for (int i = 0; i != worldRenderers[3].Length; ++i)
            {
                if (worldRenderers[3][i] != null)
                {
                    worldRenderers[3][i].sortingOrder = 1;
                    worldRenderers[3][i].enabled = true;
                    worldRenderers[3][i].color = opaque;
                    buttonsPressed[3] = true;
                }
            }
        }

        for(int i = 0; i != buttonsPressed.Length; ++i)
        {
            //Debug.Log(worlds.Count);
            if(!buttonsPressed[i] && worlds[i] != activeWorld && worlds[i] != null)
            {
                for (int j = 0; j != worldRenderers[i].Length; ++j)
                {
                    if (worldRenderers[i][j] != null)
                    {
                        worldRenderers[i][j].sortingOrder = 0;
                        worldRenderers[i][j].color = solid;
                        worldRenderers[i][j].enabled = false;
                    }
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
                    worldColliders[i][j].isTrigger = true; //change made here
                    worldColliders[i][j].tag = "CollInactive";
                    worldColliders[i][j].gameObject.layer = LayerMask.NameToLayer("GroundInactive");
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
                for (int j = 0; j != enemyColliders[i].Length; ++j)
                {
                    if (enemyColliders[i][j] != null)
                    {
                        enemyColliders[i][j].enabled = false;
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
                        BGRenderers[activeWorldNum][j].sortingOrder = -1;
                        BGRenderers[activeWorldNum][j].enabled = true;
                    }
                }
                else
                {
                    for (int j = 0; j != BGRenderers[i].Length; ++j)
                    {
                        BGRenderers[i][j].sortingOrder = -2;
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
    

    //private void Awake()
    //{
    //    for (int i = 1; i != 5; ++i)
    //    {
    //        if (GameObject.FindGameObjectWithTag("World" + i))
    //        {
    //            worlds.Add(GameObject.FindGameObjectWithTag("World" + i));
    //        }
    //        else
    //        {
    //            break;
    //        }
    //    }

    //    for(int i = 0; i != worlds.Count; ++i)
    //    {
    //        hardWorlds[i] = worlds[i].transform.GetChild(0).gameObject;
    //        hardWorlds[i].SetActive(false);

    //        softWorlds[i] = worlds[i].transform.GetChild(1).gameObject;
    //        softWorlds[i].SetActive(false);
    //    }

    //    activeWorld = hardWorlds[0];

    //    activeWorld.SetActive(true);

    //    //Debug.Log(worlds.Count);
    //}

    //private void Update()
    //{
    //    //y = Input.GetAxisRaw("RJY");
    //    Switcher();
    //    Preview();
    //}

    //private void Switcher()
    //{
    //    if (Mathf.Abs(Input.GetAxis("RJX")) > Mathf.Abs(Input.GetAxis("RJY")))
    //    {
    //        if (Input.GetAxisRaw("RJX") < 0 && activeWorld != hardWorlds[0] && hardWorlds[0] != null)
    //        {
    //            SetActiveWorld(0);
    //        }
    //        else if (Input.GetAxisRaw("RJX") > 0 && activeWorld != hardWorlds[2] && hardWorlds[2] != null)
    //        {
    //            SetActiveWorld(2);
    //        }
    //    }
    //    else
    //    {
    //        if (Input.GetAxisRaw("RJY") < 0 && activeWorld != hardWorlds[1] && hardWorlds[1] != null)
    //        {
    //            SetActiveWorld(1);
    //        }
    //        else if (Input.GetAxisRaw("RJY") > 0 && activeWorld != hardWorlds[3] && hardWorlds[3] != null)
    //        {
    //            SetActiveWorld(3);
    //        }
    //    }
    //}

    //private void SetActiveWorld(int i)
    //{
    //    activeWorld.SetActive(false);
    //    activeWorld = hardWorlds[i];
    //    activeWorld.SetActive(true);
    //}

    //private void Preview()
    //{
    //    if(Input.GetAxisRaw("DPadX") < 0 && softWorlds[0] != null)
    //    {
    //        softWorlds[0].SetActive(true);
    //    }
    //    else if(Input.GetAxisRaw("DPadX") > 0 && softWorlds[2] != null)
    //    {
    //        softWorlds[2].SetActive(true);
    //    }
    //    else if(Input.GetAxisRaw("DPadY") > 0 && softWorlds[1] != null)
    //    {
    //        softWorlds[1].SetActive(true);
    //    }
    //    else if(Input.GetAxisRaw("DPadY") < 0 && softWorlds[3] != null)
    //    {
    //        softWorlds[3].SetActive(true);
    //    }
    //    else
    //    {
    //        for (int i = 0; i != softWorlds.Length; ++i)
    //        {
    //            if(softWorlds[i] != null && softWorlds[i].activeSelf == true)
    //            {
    //                softWorlds[i].SetActive(false);
    //            }
    //        }
    //    }
    //}
}
