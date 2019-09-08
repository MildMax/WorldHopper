using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    ExitScript exit;

    private void Awake()
    {
        exit = GetComponentInChildren<ExitScript>();
    }

    private void Update()
    {
        if (exit.canExit) Debug.Log("Yay, you won!");
    }
}
