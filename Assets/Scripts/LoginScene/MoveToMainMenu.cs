using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
public class MoveToMainMenu : MonoBehaviour
{
    public VideoPlayer vid;


    void Start() {
        if (Application.isBatchMode)
        {
            SceneManager.LoadScene(1);
        }

        vid.loopPointReached += CheckOver; 
    
    }

    void CheckOver(UnityEngine.Video.VideoPlayer vp)
    {
        SceneManager.LoadScene(1);
    }

}
