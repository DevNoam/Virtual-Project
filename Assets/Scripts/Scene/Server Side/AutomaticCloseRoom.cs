using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutomaticCloseRoom : MonoBehaviour
{
    // Start is called before the first frame update


    private string roomName;
    public float closingTimer;
    public int PlayersInScene;
    public NetworkManager networkManager;


    [Server]
    void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        roomName = gameObject.scene.name;
        Debug.Log(roomName);

        Invoke("CloseRoom", closingTimer);

    }

    [Server]
    void CloseRoom()
    {
        GameObject[] _RootSceneObjects = SceneManager.GetSceneByName(roomName).GetRootGameObjects();


        bool HasActivePlayers = false;
        for (int i = 3; i < _RootSceneObjects.Length; i++)
        {
            if (_RootSceneObjects[i].name.Contains(networkManager.playerPrefab.name + "(Clone)"))
            {
                HasActivePlayers = true;
                break;
            }
        }
        if (HasActivePlayers == false)
        {
            Debug.Log($"The room {roomName} has been closed!");
            SceneManager.UnloadSceneAsync(roomName);

        }
        else if (HasActivePlayers == true)
        {
            Debug.Log($"The room: {roomName}, has active players inside.");
            Invoke("CloseRoom", closingTimer);
        }
    }
}
