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
                NavMeshHit navHit;
                NavMesh.SamplePosition(hit.point, out navHit, 1, -1);

                NavMeshPath navpath = new NavMeshPath();
                NavMesh.CalculatePath(hit.point, hit.point, -1, navpath);
                if (navpath.status == NavMeshPathStatus.PathComplete)
                {
                    navMeshController.SetDestination(navHit.position);
                }

                //CmdScrPlayerSetDestination(hit.point);
            }

            if (Input.GetAxis("Mouse X") != 0 && Moving == false || Input.GetAxis("Mouse Y") != 0 && Moving == false) // Rotation
            {
                    var lookPos = hit.point - player.transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    player.transform.rotation = Quaternion.Slerp(player.transform.rotation, rotation, Time.deltaTime * navMeshController.angularSpeed);
            }
            if (navMeshController.remainingDistance < navMeshController.stoppingDistance)
            {
                Moving = false;
            }
        }
    }
    [Command]
    public void CmdScrPlayerSetDestination(Vector3 argPosition)
    {//Step B, I do simple work, I not verifi a valid position in server, I only send to all clients
        RpcScrPlayerSetDestination(argPosition);
    }

    [ClientRpc]
    public void RpcScrPlayerSetDestination(Vector3 argPosition)
    {//Step C, only the clients move
        navMeshController.SetDestination(argPosition);
        Moving = true;
        var lookPos = argPosition - player.transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, rotation, 20);
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
        CmdScrPlayerName(playerName);
    }
    [Command]
    public void CmdScrPlayerName(string playername)
    {
        playerNameMesh.text = playername;
        RpcScrPlayerName(playername);
    }

    [ClientRpc]
    public void RpcScrPlayerName(string playername)
    {
        playerNameMesh.text = playername;
    }
    #endregion
}
