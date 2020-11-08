using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutomaticCloseRoom : MonoBehaviour
{
    // Start is called before the first frame update


    private string roomName;
    public NetworkManager networkManager;
    public float closingTimer;
    [Server]
    void Start()
    {
        roomName = gameObject.scene.name;
        Debug.Log(roomName);

        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        Invoke("CloseRoom", closingTimer);

    }

    [Server]
    void CloseRoom()
    {
        if (GameObject.FindGameObjectsWithTag("Player") != null) // SEARCH FOR PLAYER INSIDE SPECIFIC SCENE!!!.
        {
            Invoke("CloseRoom", closingTimer);
            Debug.Log($"Found players on {roomName} closing has been aborted.");
        }
        else
        {
            Debug.Log($"The room {roomName} closed!");
            SceneManager.UnloadSceneAsync(roomName);
        }
    }
}
