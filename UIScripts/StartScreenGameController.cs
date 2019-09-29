using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenGameController : MonoBehaviour
{
    GameObject levelUI;
    Transform[] levelUIComponents;

    private void Awake()
    {
        levelUI = GameObject.FindGameObjectWithTag("LevelSelect");
        levelUIComponents = levelUI.GetComponentsInChildren<Transform>();
        DeactivateLevelUI();
    }

    public void ActivateLevelUI()
    {
        for(int i = 0; i != levelUIComponents.Length; ++i)
        {
            levelUIComponents[i].gameObject.SetActive(true);
        }
    }

    public void DeactivateLevelUI()
    {
        for (int i = 0; i != levelUIComponents.Length; ++i)
        {
            levelUIComponents[i].gameObject.SetActive(false);
        }
    }

}
