using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using TMPro;

public class PlayerManager : NetworkBehaviour
{
    /// <summary>
    /// Movement
    /// </summary>
    public NavMeshAgent navMeshController;
    public Camera cam;
    public Transform player;

    /// <summary>
    /// Player Name
    /// </summary>
    [SyncVar]
    public string playerName;

    public TextMeshPro playerNameMesh;
    

    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        player.GetComponent<Transform>();
        playerNameMesh = this.transform.Find("PlayerName").GetComponent<TextMeshPro>();
    }



    void Update()
    {
        playerNameMesh.text = playerName;

        if (isLocalPlayer)
        {
            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    navMeshController.SetDestination(hit.point);
                }
            }
        }
        else { return; }
    }

    public void SendReadyToServer(string playername)
    {
        if (!isLocalPlayer)
            return;

        CmdReady(playername);

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
}
