using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventBase : MonoBehaviour
{
    public int channel;

    [HideInInspector]
    public bool triggerEvent = false;
}
