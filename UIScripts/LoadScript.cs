using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScript : MonoBehaviour
{

    public void LoadCurrentScene()
    {
        Debug.Log("Loading current scene!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainScreen()
    {
        SceneManager.LoadScene(0);
    }

    public void LoadScene(int levelID)
    {
        SceneManager.LoadScene(levelID);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }
}
