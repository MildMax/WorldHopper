using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLightScript : MonoBehaviour {

    [HideInInspector]
    public GameObject worldLight;

    [HideInInspector]
    public Light torchLight;

    [HideInInspector]
    public ItemSwitcherAlt itemSwitcherAlt;

    private void Awake()
    {
        worldLight = GameObject.FindGameObjectWithTag("WorldLight");
        torchLight = GetComponentInChildren<Light>();
        itemSwitcherAlt = GetComponentInChildren<ItemSwitcherAlt>();
    }

    //private void Update()
    //{
    //    if(itemSwitcher.itemIndex == 5 && worldLight.activeSelf == false)
    //    {
    //        torchLight.enabled = true;
    //    }
    //    else
    //    {
    //        torchLight.enabled = false;
    //    }
    //}


}
