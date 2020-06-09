using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScript : MonoBehaviour
{
    GameObject loadScreen;

    private void Awake()
    {
        loadScreen = GameObject.FindGameObjectWithTag("LoadScreen");
        loadScreen.SetActive(false);
    }

    public void LoadCurrentScene()
    {
        Debug.Log("Loading current scene!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadMainScreen()
    {
        Load(0);
    }

    public void LoadScene(int levelID)
    {
        Load(levelID);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }

    private void Load(int sceneIndex)
    {
        AsyncOperation a = SceneManager.LoadSceneAsync(sceneIndex);
        loadScreen.SetActive(true);
    }
}
