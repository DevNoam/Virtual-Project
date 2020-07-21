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

    Vector2 lastMousePosition;
    bool isMoving = false;

    void Start()
    {
        DontDestroyOnLoad(transform.gameObject);
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        player.GetComponent<Transform>();
        playerNameMesh = this.transform.Find("PlayerName").GetComponent<TextMeshPro>();
    }



    void Update()
    {
        if (isLocalPlayer)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                if (Physics.Raycast(ray, out hit))
                {
                    CmdScrPlayerSetDestination(hit.point, playerName);
                    var lookPos = hit.point - player.transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    player.transform.rotation = Quaternion.Slerp(player.transform.rotation, rotation, 20);
                    isMoving = true;
                    Invoke("checkIfStopped", 0.05f);
                }
            }

            if (Physics.Raycast(ray, out hit) && isMoving == false)
            {
                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                {
                    var lookPos = hit.point - player.transform.position;
                    lookPos.y = 0;
                    var rotation = Quaternion.LookRotation(lookPos);
                    player.transform.rotation = Quaternion.Slerp(player.transform.rotation, rotation, Time.deltaTime * navMeshController.angularSpeed);
                }
            }
        }
        else
        {
            return;
        }
    }

    [Command]
    public void CmdScrPlayerSetDestination(Vector3 argPosition, string playername)
    {//Step B, I do simple work, I not verifi a valid position in server, I only send to all clients
        RpcScrPlayerSetDestination(argPosition, playername);
    }

    [ClientRpc]
    public void RpcScrPlayerSetDestination(Vector3 argPosition, string playername)
    {//Step C, only the clients move
        navMeshController.SetDestination(argPosition);
    }
    void checkIfStopped()
    {
        float dist = navMeshController.remainingDistance;
        if (dist != Mathf.Infinity && navMeshController.pathStatus == NavMeshPathStatus.PathComplete && navMeshController.remainingDistance == 0)
        {
            isMoving = false;
        }
        else {
            Invoke("checkIfStopped", 0.05f);
        }
    }



    public void SendReadyToServer(string playername)
    {
        if (!isLocalPlayer)
            return;

        CmdReady(playername);
        SendName();
    }

    [Command]
    void CmdReady(string playername)
    {
        if (string.IsNullOrEmpty(playername))
        {
            playerName = "Guest: " + Random.Range(1, 99);
        }
        else
        {
            playerName = playername;
        }
    }

    void SendName()
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
        Invoke("SendName", 2.5f);
    }
}
