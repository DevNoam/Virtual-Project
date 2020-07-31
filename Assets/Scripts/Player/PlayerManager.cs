using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;

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

    public RoomManager roomManager;

    public bool Moving;
    public float rotationSpeed = 20;


    void Start()
    {
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        DontDestroyOnLoad(transform.gameObject);
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        player.GetComponent<Transform>();
        playerNameMesh = this.transform.Find("PlayerName").GetComponent<TextMeshPro>();
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
        if (string.IsNullOrEmpty(playername))
        {
            playerName = "Guest: " + Random.Range(1, 99);
        }
        else
        {
            playerName = playername;
            RpcSendPlayerName();
        }
    }

    #region Player Name management

    [ClientRpc]
    void RpcSendPlayerName()
    {
        roomManager.SendNameToClients();
    }


    public void SendName()
    {
        CmdScrPlayerName(playerName, roomManager.playerSkin);

    }
    [Command]
    public void CmdScrPlayerName(string playername, int playerSkin)
    {

        playerNameMesh.text = playername;
        player.GetComponent<Renderer>().material = roomManager.skins[playerSkin];
        RpcScrPlayerName(playername, playerSkin);
    }

    [ClientRpc]
    public void RpcScrPlayerName(string playername, int playerSkin)
    {
        playerNameMesh.text = playername;
        player.GetComponent<Renderer>().material = roomManager.skins[playerSkin];
    }
    #endregion
}
