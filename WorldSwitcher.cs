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

    //GameObject[] hardWorlds = { null, null, null, null };
    //GameObject[] softWorlds = { null, null, null, null };

    GameObject activeWorld;
    public int activeWorldNum;

    //public float x;
    //public float y;

    private void Awake()
    {
        GetWorlds();
        RemoveBGRenderer();
        SetLevel();
        SetActiveWorld(activeWorldNum);
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
        }

        while (worlds.Count != 4)
        {
            worlds.Add(null);
        }
    }

    public void Switcher()
    {
        if (Mathf.Abs(Input.GetAxis("RJX")) > Mathf.Abs(Input.GetAxis("RJY")))
        {
            if (Input.GetAxisRaw("RJX") < 0 && activeWorld != worlds[0] && worlds[0] != null)
            {
                SetActiveWorld(0);
            }
            else if (Input.GetAxisRaw("RJX") > 0 && activeWorld != worlds[2] && worlds[2] != null)
            {
                SetActiveWorld(2);
            }
        }
        else
        {
            if (Input.GetAxisRaw("RJY") < 0 && activeWorld != worlds[1] && worlds[1] != null)
            {
                SetActiveWorld(1);
            }
            else if (Input.GetAxisRaw("RJY") > 0 && activeWorld != worlds[3] && worlds[3] != null)
            {
                SetActiveWorld(3);
            }
        }
    }

    private void SetActiveWorld(int i)
    {
        
        StartCoroutine(SwitchWorlds(worlds[i], worlds[activeWorldNum]));

        //Debug.Log(worldRenderers[activeWorldNum].Length);

        for (int k = 0; k != worldRenderers[activeWorldNum].Length; ++k)
        {
            if (worldRenderers[activeWorldNum][k] != null)
            {
                worldRenderers[activeWorldNum][k].enabled = false;
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

        for (int j = 0; j != worldColliders[i].Length; ++j)
        {
            worldColliders[i][j].enabled = true;
        }

        if (worlds[i] != worlds[activeWorldNum])
        {
            for (int j = 0; j != worldColliders[activeWorldNum].Length; ++j)
            {
                worldColliders[activeWorldNum][j].enabled = false;
            }
        }
        

        activeWorld = worlds[i];
        //activeWorld.SetActive(false);
        //activeWorld.SetActive(true);
        activeWorldNum = i;
        
    }

    private IEnumerator SwitchWorlds(GameObject set, GameObject unset)
    {
        if (set && unset)
        {
            if (set != unset)
            {
                SpriteRenderer[] next = set.GetComponentsInChildren<SpriteRenderer>();
                SpriteRenderer[] prev = unset.GetComponentsInChildren<SpriteRenderer>();

                for (int i = 0; i != next.Length; ++i)
                {
                    if (next[i].tag == "BGRenderer")
                    {
                        next[i].sortingOrder = -2;
                    }
                    else
                    {
                        next[i].sortingOrder = -1;
                    }
                }

                //set.SetActive(true);
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                //unset.SetActive(false);

                for (int i = 0; i != next.Length; ++i)
                {
                    if (next[i].tag == "BGRenderer")
                    {
                        next[i].sortingOrder = -1;
                    }
                    else
                    {
                        next[i].sortingOrder = 0;
                    }
                }
            }
            else set.SetActive(true); yield return null;
        }
        else yield return null;
    }

    public void Preview()
    {
        bool[] buttonsPressed = { false, false, false, false };

        if (Input.GetAxisRaw("DPadX") < 0 && worldRenderers[0] != null && activeWorld != worlds[0])
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
        if (Input.GetAxisRaw("DPadX") > 0 && worldRenderers[2] != null && activeWorld != worlds[2])
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
        if (Input.GetAxisRaw("DPadY") > 0 && worldRenderers[1] != null && activeWorld != worlds[1])
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
        if (Input.GetAxisRaw("DPadY") < 0 && worldRenderers[3] != null && activeWorld != worlds[3])
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
                    worldColliders[i][j].enabled = false;
                }
            }
        }

        //for(int i = 0; i != worlds.Count; ++i)
        //{
        //    if (worlds[i] != null)
        //    {
        //        worlds[i].SetActive(false);
        //    }
        //}
    }

    private void RemoveBGRenderer()
    {
        for(int i = 0; i != worldRenderers.Length; ++i)
        {
            if (worldRenderers[i] != null)
            {
                for (int j = 0; j != worldRenderers[i].Length; ++j)
                {
                    if (worldRenderers[i][j].gameObject.tag == "BGRenderer")
                    {
                        //Debug.Log("renderer removed: " + worldRenderers[i][j].gameObject.name);
                        worldRenderers[i][j] = null;
                    }
                }

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

        T[] newT = new T[len];
        int index = 0;

        for(int i = 0; i != t.Length; ++i)
        {
            if(t[i] != null)
            {
                newT[index] = t[i];
            }
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
