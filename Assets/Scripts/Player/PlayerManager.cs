using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public class PlayerManager : NetworkBehaviour
{
    /// <summary>
    /// Movement
    /// </summary>
    /// 
    public NavMeshAgent navMeshController;
    public Camera cam;

    public Transform player;
    /// <summary>
    /// Player Name
    /// </summary>
    [SyncVar]
    public string playerName;
    public TextMeshPro playerNameMesh;
    public TextMeshPro PlayerCircle;
    [SyncVar]
    public int SkinColor = 1;

    public RoomManager roomManager;

    public bool Moving;
    public float rotationSpeed = 20;
    public ChatSystem chatSystem;

    [SerializeField]
    public bool isNewPlayer = true;


    void Start()
    {
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        if (!hasAuthority)
            Destroy(cam);
        if (hasAuthority)
        {
            MakeBoldNameForLocalClient();
        }

    }

    #region Movement

    void Update()
    {
        if (!hasAuthority)
        {
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {

            if (Input.GetMouseButtonUp(0) && hit.transform.tag != "MouseHitCollider") // Movement
            {
                CmdScrPlayerSetDestination(hit.point);
            }

            if (Moving == false && Input.GetAxis("Mouse X") != 0 || Moving == false && Input.GetAxis("Mouse Y") != 0) // Rotation
            {
                var lookPos = hit.point - player.transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }
            if (navMeshController.remainingDistance < navMeshController.stoppingDistance && Moving == true)
            {
                Moving = false;
            }
        }
    }
    [Command]
    public void CmdScrPlayerSetDestination(Vector3 argPosition)
    {
        RpcScrPlayerSetDestination(argPosition);
    }

    [ClientRpc]
    public void RpcScrPlayerSetDestination(Vector3 argPosition)
    {
        Moving = true;
        navMeshController.SetDestination(argPosition);
    }

    #endregion


    [Command]
    public void CmdReady(string playername)
    {
        playerName = playername;
        RpcSendPlayerName();
    }


    #region Player Name management

    [ClientRpc]
    void RpcSendPlayerName()
    {
        roomManager.SendNameToClients();
    }


    public void SendName(string playername, int playerSkin)
    {
        CmdScrPlayerName(playername, playerSkin);
    }



    [Command]
    public void CmdScrPlayerName(string playername, int playerSkin)
    {
        playerNameMesh.text = playername;
        player.GetComponent<Renderer>().material = roomManager.skins[playerSkin];
        //SkinColor = playerSkin;

        chatSystem.playerName = playername;
        RpcScrPlayerName(playername, playerSkin);
    }

    [ClientRpc]
    public void RpcScrPlayerName(string playername, int playerSkin)
    {
        playerNameMesh.text = playername;
        player.GetComponent<Renderer>().material = roomManager.skins[playerSkin];
        //SkinColor = playerSkin;

        chatSystem.playerName = playername;
    }
    #endregion


    [Client]
    public void ChangeRoom(GameObject desiredRoomGameObject, GameObject thisRoom, GameObject desiredSpawnLocation)
    {
        if (isLocalPlayer)
        {
            desiredRoomGameObject.SetActive(true);
            CmdChangePosition(desiredSpawnLocation.transform.position, playerName);
            thisRoom.SetActive(false);
            roomManager.LoadingGUI.SetActive(true);
            cam.transform.GetComponent<AudioListener>().enabled = false;
            Invoke("Arrived", 1f);

            //Reset player:
            chatSystem.CmdDelayedFunction();
        }
    }

    [Command]
    public void CmdChangePosition(Vector3 argPosition, string playerName)
    {
        player.GetComponentInChildren<NavMeshAgent>().Warp(argPosition);
        //this.transform.position = argPosition;

        RpcChangePosition(argPosition, playerName);
    }

    [ClientRpc]
    public void RpcChangePosition(Vector3 argPosition, string name)
    {
        player.GetComponentInChildren<NavMeshAgent>().Warp(argPosition);
        //this.transform.position = argPosition;

    }

    [Client]
    void Arrived()
    {
        if (player.GetComponentInChildren<NavMeshAgent>().remainingDistance < player.GetComponentInChildren<NavMeshAgent>().stoppingDistance)
        {
            cam.transform.GetComponent<AudioListener>().enabled = true;
            roomManager.LoadingGUI.SetActive(false);
        }
        else
        {
            Arrived();
        }
    }

    [Client]
    void MakeBoldNameForLocalClient()
    {
        playerNameMesh.fontStyle = FontStyles.Bold;
        PlayerCircle.gameObject.SetActive(true);

    }

}
