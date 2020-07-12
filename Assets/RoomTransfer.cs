using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
public class RoomTransfer : MonoBehaviour
{
    public string SceneID;
    NetworkTransform LocalPlayer;
    // Start is called before the first frame update
    void Start()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NetworkManager.singleton.ServerChangeScene(SceneID);
        }

    }
}
