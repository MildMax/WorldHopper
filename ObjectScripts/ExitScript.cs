using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitScript : MonoBehaviour
{
    KeyScript keyManager;

    bool needsRed;
    bool needsBlue;
    bool needsGreen;
    bool needsYellow;

    [HideInInspector]
    public bool canExit = false;

    private void Awake()
    {
        keyManager = GameObject.FindGameObjectWithTag("KeyCanvas").GetComponentInChildren<KeyScript>();
        SetExitConditions();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && CheckExitConditions())
        {
            canExit = true;
        }
    }

    private bool CheckExitConditions()
    {
        if (needsRed && !keyManager.hasRedKey) return false;

        if (needsBlue && !keyManager.hasBlueKey) return false;

        if (needsGreen && !keyManager.hasGreenKey) return false;

        if (needsYellow && !keyManager.hasYellowKey) return false;

        return true;
    }

    private void SetExitConditions()
    {
        if (keyManager.redKey) needsRed = true;
        else needsRed = false;

        if (keyManager.blueKey) needsBlue = true;
        else needsBlue = false;

        if (keyManager.greenKey) needsGreen = true;
        else needsGreen = false;

        if (keyManager.yellowKey) needsYellow = true;
        else needsYellow = false;
    }
}
