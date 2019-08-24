using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventListener : MonoBehaviour
{
    public int channel;
    public bool worldSpecific;

    TriggerEventBase t;
    WorldSwitcher wS;

    bool channelSet = false;
    [HideInInspector]
    public int worldNum;

    private void Awake()
    {
        GameObject[] temp = GameObject.FindGameObjectsWithTag("TriggerEvent");
        Debug.Log(temp.Length);
        for(int i = 0; i != temp.Length; ++i)
        {
            if(temp[i].GetComponent<TriggerEventBase>().channel == channel)
            {
                if (!channelSet)
                {
                    t = temp[i].GetComponent<TriggerEventBase>();
                    channelSet = true;
                }
                else
                {
                    Debug.Log("There is more than one trigger game object linked to this channel");
                }
            }
        }
        wS = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WorldSwitcher>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!worldSpecific)
        {
            if (collision.gameObject.tag == "Player") t.triggerEvent = true;
        }
        else
        {
            if (collision.gameObject.tag == "Player" && worldNum == wS.activeWorldNum)
            { t.triggerEvent = true; }
        }

    }
}
