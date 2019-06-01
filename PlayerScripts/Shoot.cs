using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {

    public GameObject shot;

    //ItemSwitcher itemSwitcher;
    ItemSwitcherAlt itemSwitcherAlt;
    PlayerController playerController;
    ShotScript shotScript;
    Transform parentTransform;
    InputManager IM;
    bool isShoot = false;
    Transform[] shootPoints;

    //bool isLeft = false;

    //Vector3[] positions =
    //{
    //    new Vector2(-0.45f, 0),
    //    new Vector2(0.45f, 0)
    //};

    private void Awake()
    {
        //itemSwitcher = GetComponentInParent<ItemSwitcher>();
        itemSwitcherAlt = GetComponentInParent<ItemSwitcherAlt>();
        playerController = GetComponentInParent<PlayerController>();
        parentTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        IM = GetComponentInParent<InputManager>();
        shootPoints = GetShootPoints();
    }

    //private void Update()
    //{
    //    GetButtonPress();
    //}

    //private void FixedUpdate()
    //{
    //    MoveShootPoint();
    //    ShootRaygun();
        
    //}

    private void LateUpdate()
    {
        //CorrectPosition();
    }

    public void GetButtonPress()
    {
        if (Input.GetButtonDown(IM.useItem) && itemSwitcherAlt.itemIndex == 2 && isShoot == false)
        {
            isShoot = true;
        }
    }

    //public void MoveShootPoint()
    //{
    //    if (playerController.direction < 0 && itemSwitcherAlt.itemIndex == 2 && isLeft == false)
    //    {
    //        transform.position = transform.position + positions[0];
    //        isLeft = true;

    //        //Debug.Log("moving shoot point");
    //    }
    //    else if (playerController.direction > 0 && itemSwitcherAlt.itemIndex == 2 && isLeft == true)
    //    {
    //        transform.position = transform.position + positions[1];
    //        isLeft = false;

    //        //Debug.Log("moving shoot point");
    //    }
    //}

    public void ShootRaygun()
    {
        if (isShoot == true)
        {
            if(playerController.direction > 0 && playerController.grounded == true)
            {
                Instantiate(shot, shootPoints[0].position, Quaternion.identity);
            }
            else if (playerController.direction < 0 && playerController.grounded == true)
            {
                Instantiate(shot, shootPoints[1].position, Quaternion.identity);
            }
            else if (playerController.direction > 0 && playerController.grounded == false)
            {
                Instantiate(shot, shootPoints[2].position, Quaternion.identity);
            }
            else if (playerController.direction < 0 && playerController.grounded == false)
            {
                Instantiate(shot, shootPoints[3].position, Quaternion.identity);
            }
            //Instantiate(shot, transform.position, Quaternion.identity);
            isShoot = false;
        }
    }

    private Transform[] GetShootPoints()
    {
        List<Transform> l = new List<Transform>();
        string[] n = { "A", "B", "C", "D" };
        Transform[] tot = GetComponentsInChildren<Transform>();

        for(int i = 0; i != tot.Length; ++i)
        {
            for (int j = 0; j != n.Length; ++j)
            {
                if (tot[i].name == "Shoot Point " + n[j])
                {
                    l.Add(tot[i]);
                }
            }
        }

        Transform[] p = new Transform[l.Count];

        for(int i = 0; i != l.Count; ++i)
        {
            p[i] = l[i];
            //Debug.Log(p[i]);
        }

        //Debug.Log("P length: " + p.Length);

        return p;
    }
}
