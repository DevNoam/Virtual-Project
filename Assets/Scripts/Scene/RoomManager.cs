using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.GroupsModels;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public Material[] skins;
    public int playerSkin;

    public int targetFrameRate = 60;

    public GameObject ChatLog;
    public GameObject LoadingGUI;

    private void Start()
    {
        Application.targetFrameRate = targetFrameRate;
    }


    public void fail(PlayFabError error)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}