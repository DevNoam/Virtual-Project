using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ToolBarManager : MonoBehaviour
{

    public GameObject OpenEmoji;
    public GameObject OpenEmojiContainer;

    public void OpenEmojiBar()
    {
        if (!OpenEmojiContainer.activeInHierarchy)
        {
            OpenEmojiContainer.SetActive(true);
        }
        else if (OpenEmojiContainer.activeInHierarchy)
        {
            OpenEmojiContainer.SetActive(false);
        }
    }

    public void changeScene()
    {
        SceneManager.LoadScene(3);
    }
}
