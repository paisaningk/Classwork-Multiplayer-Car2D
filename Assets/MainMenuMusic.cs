using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMusic : MonoBehaviour
{
    public static MainMenuMusic instance;
    private void Start()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "Map")
        {
           DestroyImmediate(this);
        }
        else if(SceneManager.GetActiveScene().name == "Multimap_2")
        {
            DestroyImmediate(this);
        }
        else if(SceneManager.GetActiveScene().name == "Multimap_3")
        {
            DestroyImmediate(this);
        }
    }
}
