using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneHolder : MonoBehaviour
{
    public int levelID;

    public void ChangeScene()
    {
        LoadScript ls = GameObject.FindGameObjectWithTag("GameController").GetComponent<LoadScript>();
        ls.LoadScene(levelID);
    }
}
