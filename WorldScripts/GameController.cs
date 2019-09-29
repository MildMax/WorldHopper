using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    ExitScript exit;
    GameObject gameOverlay;
    Transform[] gameOverlayComponents;
    InputManager IM;
    Text pauseText;
    PlayerController playerController;

    bool isPaused = false;

    private void Awake()
    {
        exit = GetComponentInChildren<ExitScript>();
        IM = GetComponent<InputManager>();
        pauseText = GameObject.FindGameObjectWithTag("PauseText").GetComponent<Text>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        gameOverlay = GameObject.FindGameObjectWithTag("LevelSelect");
        gameOverlayComponents = gameOverlay.GetComponentsInChildren<Transform>();
        DeactivateOverlay();
    }

    private void Update()
    {
        if (exit.canExit || playerController == null) ActivateOverlay();
        DetectPause();
    }


    private void DetectPause()
    {
        if (Input.GetButtonDown(IM.pause) && !isPaused) ActivateOverlay();
        else if (Input.GetButtonDown(IM.pause) && isPaused) DeactivateOverlay();
    }

    private void ActivateOverlay()
    {
        for(int i = 0; i != gameOverlayComponents.Length; ++i)
        {
            gameOverlayComponents[i].gameObject.SetActive(true);
        }
        SetPauseText();
        isPaused = true;
    }

    private void DeactivateOverlay()
    {
        for(int i = 0; i != gameOverlayComponents.Length; ++i)
        {
            gameOverlayComponents[i].gameObject.SetActive(false);
        }
        isPaused = false;
    }

    private void SetPauseText()
    {
        if (playerController == null) pauseText.text = "You died!!!";
        else if (!exit.canExit) pauseText.text = "Pause!";
        else if (exit.canExit) pauseText.text = "Fini!!!";
    }
}
