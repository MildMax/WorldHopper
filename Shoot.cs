using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour {

    public GameObject shot;

    ItemSwitcher itemSwitcher;
    PlayerController playerController;
    ShotScript shotScript;
    Transform parentTransform;
    bool isLeft = false;
    bool isShoot = false;

    Vector3[] positions =
    {
        new Vector2(-0.45f, 0),
        new Vector2(0.45f, 0)
    };

    private void Awake()
    {
        itemSwitcher = GetComponentInParent<ItemSwitcher>();
        playerController = GetComponentInParent<PlayerController>();
        parentTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
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
        if (Input.GetButtonDown("UseItem") && itemSwitcher.itemIndex == 2 && isShoot == false)
        {
            isShoot = true;
        }
    }

    public void MoveShootPoint()
    {
        if (playerController.direction < 0 && itemSwitcher.itemIndex == 2 && isLeft == false)
        {
            transform.position = transform.position + positions[0];
            isLeft = true;

            Debug.Log("moving shoot point");
        }
        else if (playerController.direction > 0 && itemSwitcher.itemIndex == 2 && isLeft == true)
        {
            transform.position = transform.position + positions[1];
            isLeft = false;

            Debug.Log("moving shoot point");
        }
    }

    public void ShootRaygun()
    {
        if (isShoot == true)
        {
            Instantiate(shot, transform.position, Quaternion.identity);
            isShoot = false;
        }
    }

    private void CorrectPosition()
    {
        if (playerController.direction < 0 && itemSwitcher.itemIndex == 2 && transform.position != parentTransform.position + positions[0])
        {
            Vector3.MoveTowards(transform.position, parentTransform.position + positions[0], 0.9f);
            //Debug.Log("correcting position");
        }
        else if (playerController.direction > 0 && itemSwitcher.itemIndex == 2 && transform.position != parentTransform.position + positions[1])
        {
            Vector3.MoveTowards(transform.position, parentTransform.position + positions[1], 0.9f);
            //Debug.Log("correcting position");
        }
    }
}
