using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchScript : MonoBehaviour
{
    public int channel;
    public bool isOn;
    public bool isPersistent;
    [HideInInspector]
    public int worldNum;

    WorldSwitcher wS;
    PlayerController playerController;
    GameObject player;
    SpriteRenderer rend;

    public Sprite[] onSwitches;
    public Sprite[] offSwitches;

    [HideInInspector]
    public int colorIndex;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        wS = player.GetComponentInChildren<WorldSwitcher>();
        playerController = player.GetComponent<PlayerController>();
        rend = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        SwitchSwitch();
    }

    private void SwitchSwitch()
    {
        if (isOn) rend.sprite = onSwitches[colorIndex];
        else rend.sprite = offSwitches[colorIndex];
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //Debug.Log("Colliding");
        if (collision.gameObject.tag == "Player" && (wS.activeWorldNum == worldNum || isPersistent))
        {
            if (playerController.isInteract)
            {
                isOn = !isOn;
                playerController.isInteract = false;
            }
        }
    }
}
