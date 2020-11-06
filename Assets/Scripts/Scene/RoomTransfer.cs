using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;


public class RoomTransfer : NetworkBehaviour
{
    public string SceneID;
    public Vector3 SpawnLocation;


    // Start is called before the first frame update

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.GetComponentInParent<PlayerManager>().ChangeRoom(SceneID, SpawnLocation);
        }
    }
}
