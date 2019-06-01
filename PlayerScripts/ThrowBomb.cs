using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBomb : MonoBehaviour {

    public GameObject bomb;

    //ItemSwitcher itemSwitcher;
    ItemSwitcherAlt itemSwitcherAlt;
    InputManager IM;
    bool buttonPressed = false;
    Transform[] throwPoints;
    PlayerController playerController;

    private void Awake()
    {
        //itemSwitcher = GetComponentInParent<ItemSwitcher>();
        itemSwitcherAlt = GetComponentInParent<ItemSwitcherAlt>();
        IM = GetComponentInParent<InputManager>();
        playerController = GetComponentInParent<PlayerController>();
        throwPoints = GetThrowPoints();
    }

    //private void Update()
    //{
    //    GetButtonPress();
    //}

    //private void FixedUpdate()
    //{
    //    Throw();
    //}

    public void GetButtonPress()
    {
        if(itemSwitcherAlt.itemIndex == 3 && Input.GetButtonDown(IM.useItem))
        {
            buttonPressed = true;
        }
    }

    public void Throw()
    {
        if (buttonPressed == true)
        {
            if(playerController.direction > 0)
            {
                Instantiate(bomb, throwPoints[0].position, Quaternion.identity);
            }
            else if(playerController.direction < 0)
            {
                Instantiate(bomb, throwPoints[1].position, Quaternion.identity);
            }
            //Instantiate(bomb, transform.position, Quaternion.identity);
            buttonPressed = false;
        }
    }

    private Transform[] GetThrowPoints()
    {
        List<Transform> l = new List<Transform>();
        string[] n = { "A", "B" };
        Transform[] tot = GetComponentsInChildren<Transform>();

        for (int i = 0; i != tot.Length; ++i)
        {
            for (int j = 0; j != n.Length; ++j)
            {
                if (tot[i].name == "Throw Point " + n[j])
                {
                    l.Add(tot[i]);
                }
            }
        }

        Transform[] p = new Transform[l.Count];

        for (int i = 0; i != l.Count; ++i)
        {
            p[i] = l[i];
            //Debug.Log(p[i]);
        }

        //Debug.Log("P length: " + p.Length);

        return p;
    }
}
