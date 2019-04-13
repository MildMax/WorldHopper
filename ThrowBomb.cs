using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBomb : MonoBehaviour {

    public GameObject bomb;

    ItemSwitcher itemSwitcher;

    bool buttonPressed = false;

    private void Awake()
    {
        itemSwitcher = GetComponentInParent<ItemSwitcher>();
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
        if(itemSwitcher.itemIndex == 3 && Input.GetButtonDown("UseItem"))
        {
            buttonPressed = true;
        }
    }

    public void Throw()
    {
        if (buttonPressed == true)
        {
            Instantiate(bomb, transform.position, Quaternion.identity);
            buttonPressed = false;
        }
    }
}
