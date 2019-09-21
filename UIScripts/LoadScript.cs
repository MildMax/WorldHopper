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
        //once the main screen has a scene, load to that
    }
}
