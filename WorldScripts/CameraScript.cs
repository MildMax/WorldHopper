using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour {

    public float cameraVelocity;

    GameObject player;
    Vector3 offset;
   
    Vector3 moveVelocity;
    

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        offset = player.transform.position - transform.position;
        
        transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -offset.z);
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(player.transform.position.x, player.transform.position.y, -offset.z), ref moveVelocity, cameraVelocity * Time.deltaTime);
        }
    }

    /////note:: in fixedupdate previously:::::
    //transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -offset.z);

    //CheckFreeFall();

    //if (freeFall == false)
    //{
    //    transform.position = Vector3.SmoothDamp(transform.position, new Vector3(player.transform.position.x, player.transform.position.y, -offset.z), ref moveVelocity, currentCameraVelocity * Time.deltaTime);
    //}
    //else
    //{
    //    transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -freeFallOffset.z);
    //}

    //Vector3 boundary = new Vector3(4, 2, 0);

    //bool freeFall = false;

    //public float currentCameraVelocity;

    //Vector3 freeFallOffset;

    //currentCameraVelocity = cameraVelocity;

    //private void CheckFreeFall()
    //{
    //    if (player.transform.position.x >= transform.position.x + boundary.x ||
    //        player.transform.position.y >= transform.position.y + boundary.y || 
    //        player.transform.position.x <= transform.position.y - boundary.x ||
    //        player.transform.position.y <= transform.position.y - boundary.y)
    //    {
    //        freeFallOffset = player.transform.position - transform.position;
    //        freeFall = true;
    //    }
    //    else
    //    {
    //        //freeFall = false;
    //    }
    //}

    //private void CheckFreeFall()
    //{
    //    if (player.transform.position.x >= transform.position.x + boundary.x)
    //    {
    //        transform.position = new Vector3(player.transform.position.x + boundary.x, player.transform.position.y, -offset.z);
    //    }
    //    else if (player.transform.position.y >= transform.position.y + boundary.y)
    //    {
    //        transform.position = new Vector3(player.transform.position.x, player.transform.position.y + boundary.y, -offset.z);
    //    }
    //    else if(player.transform.position.x <= transform.position.y - boundary.x)
    //    {
    //        transform.position = new Vector3(player.transform.position.x - boundary.x, player.transform.position.y, -offset.z);
    //    }
    //    else if(player.transform.position.y <= transform.position.y - boundary.y)
    //    {
    //        transform.position = new Vector3(player.transform.position.x, player.transform.position.y - boundary.y, -offset.z);
    //    }
    //    else
    //    {
    //        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(player.transform.position.x, player.transform.position.y, -offset.z), ref moveVelocity, currentCameraVelocity * Time.deltaTime);
    //    }
    //}
}

