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
        DontDestroyOnLoad(transform.gameObject);
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
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
                    navMeshController.SetDestination(hit.point);
                }
            }

            if (Physics.Raycast(ray, out hit))
            {
                var lookPos = hit.point - player.transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                player.transform.rotation = Quaternion.Slerp(player.transform.rotation, rotation, Time.deltaTime * navMeshController.angularSpeed);
            }
        }
        else { return; }
    }
    private void LateUpdate()
    {
        playerNameMesh.text = playerName;
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
