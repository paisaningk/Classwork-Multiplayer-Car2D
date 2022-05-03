using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuMusic : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(this);
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
