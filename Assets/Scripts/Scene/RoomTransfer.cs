using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;


public class RoomTransfer : NetworkBehaviour
{
    public string SceneID;
    [Tooltip("The room you want to go to, NetWork start position gameObject")]
    public GameObject desiredSpawnLocation;
    [Tooltip("The GameObject of the room you want to go to.")]
    public GameObject desiredRoomGameObject;

    public GameObject thisRoom;

    public Transform player;
    PlayerManager playerManager;



    // Start is called before the first frame update

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.GetComponentInParent<PlayerManager>().ChangeRoom(desiredRoomGameObject, thisRoom, desiredSpawnLocation);
        }
    }
}
