using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using TMPro;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
public class PlayerManager : NetworkBehaviour
{
    /// Movement
    public NavMeshAgent navMeshController;
    public Camera cam;

    public Transform player;
    /// Player Name
    [SyncVar]
    public string playerName;
    public TextMeshPro playerNameMesh;
    public TextMeshPro PlayerCircle;
    [SyncVar]
    public int SkinColor = 1;

    public RoomManager roomManager;
    public GameObject canvas;
    public GameObject LoadingFrame; //

    public bool Moving;
    public float rotationSpeed = 20;
    public ChatSystem chatSystem;
    [HideInInspector]
    public bool isModderator = false;


    void Start()
    {
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
        if (!hasAuthority)
        {
            Destroy(cam.gameObject);
            Destroy(canvas);
            canvas = null;
            cam = null;
        }
        if (hasAuthority)
        {
            MakeBoldNameForLocalClient();
            LoadingFrame = GameObject.Find("LoadingCanvas/LoadingSplashScreen").gameObject;
            LoadingFrame.GetComponent<Animator>().SetTrigger("End");
        }
        if (isServer || isServerOnly)
        {
            GameObject followPlayer = this.transform.Find("MoveableComponents").gameObject;
            Destroy(followPlayer.gameObject);
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
        navMeshController.SetDestination(argPosition);
        RpcScrPlayerSetDestination(argPosition);
    }

    [ClientRpc] //Only the caller of the CMD Command will receive this callback
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
        CmdScrPlayerName(playername, playerSkin, player.transform.position);
    }



    [Command]
    public void CmdScrPlayerName(string playername, int playerSkin, Vector3 playerPosition)
    {
        playerNameMesh.text = playername;
        player.GetComponent<Renderer>().material = roomManager.skins[playerSkin];
        //SkinColor = playerSkin;

        chatSystem.playerName = playername;
        navMeshController.Warp(playerPosition);
        Debug.Log(playername + " Has pinged his location!");
        RpcScrPlayerName(playername, playerSkin, playerPosition);
    }

    [ClientRpc]
    public void RpcScrPlayerName(string playername, int playerSkin, Vector3 playerPosition)
    {
        playerNameMesh.text = playername;
        player.GetComponent<Renderer>().material = roomManager.skins[playerSkin];
        //SkinColor = playerSkin;

        navMeshController.Warp(playerPosition);

        chatSystem.playerName = playername;
    }
    #endregion

    //////////////////

    #region Room switching

    public static bool DoesSceneExist(string name)
    {
        if (string.IsNullOrEmpty(name))
            return false;

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            var lastSlash = scenePath.LastIndexOf("/");
            var sceneName = scenePath.Substring(lastSlash + 1, scenePath.LastIndexOf(".") - lastSlash - 1);

            if (string.Compare(name, sceneName, true) == 0)
                return true;
        }

        return false;
    }
    public void ChangeRoom(string RoomName, Vector3 spawnLocation)
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (isLocalPlayer && DoesSceneExist(RoomName) == true)
        {
            if (RoomName.ToUpper() != currentSceneName.ToUpper())
            {
                StartCoroutine(Arrived(1, currentSceneName));
                LoadingFrame.SetActive(true);
                LoadingFrame.GetComponent<Animator>().SetTrigger("Start");
            }
            CmdChangeRoom(RoomName, currentSceneName, spawnLocation);
        }
        else
        {
            Debug.Log("The scene: " + RoomName + " Is invaild!, Your request has been canceled.");
        }
    }


    [Command]
    void CmdChangeRoom(string RoomName, string currentSceneName, Vector3 spawnLocation)
    {
        //Checking if Scene already active on the Server. If now create one.
        if (SceneManager.GetSceneByName(RoomName).IsValid())
        {
            Debug.Log($"The Scene {RoomName}, is already running.");
        }
        else
        {
            SceneManager.LoadSceneAsync(RoomName, LoadSceneMode.Additive);
            Debug.Log($"The Scene {RoomName}, scene is now online!.");
        }

        if (RoomName.ToUpper() != currentSceneName.ToUpper()) //Check if the player wants to Teleport to other room or Respawn.
        {
            // Move player to the new Scene on the Server side.
            SceneManager.MoveGameObjectToScene(player.parent.gameObject, SceneManager.GetSceneByName(RoomName));
        }
        //Reposition player
        player.GetComponentInChildren<NavMeshAgent>().Warp(spawnLocation);

        if (RoomName.ToUpper() != currentSceneName.ToUpper()) //Check if the player wants to Teleport to other room or Respawn.
        {
            //Telling the client to Load the new Scene
            SceneMessage Load = new SceneMessage
            {
                sceneName = RoomName,
                sceneOperation = SceneOperation.LoadAdditive,
            };
            connectionToClient.Send(Load);
        }

        //Telling the client to reposition & move local player + components.
        RpcChangeRoom(RoomName, currentSceneName, spawnLocation);

        if (RoomName.ToUpper() != currentSceneName.ToUpper()) //Check if the player wants to Teleport to other room or Respawn.
        {
            //Telling the client to unload the previous scene.
            SceneMessage unLoad = new SceneMessage
            {
                sceneName = currentSceneName,
                sceneOperation = SceneOperation.UnloadAdditive
            };
            connectionToClient.Send(unLoad);
        }
    }
    [TargetRpc]
    void RpcChangeRoom(string RoomName, string currentSceneName, Vector3 spawnLocation)
    {
        //Move player to the new Scene
        if (RoomName.ToUpper() != currentSceneName.ToUpper()) //Check if the player wants to Teleport to other room or Respawn.
        {
            SceneManager.MoveGameObjectToScene(player.parent.gameObject, SceneManager.GetSceneByName(RoomName));
        }
        //Reset text bubble
        chatSystem.CmdDelayedFunction();

        //Position player
        //player.transform.position = spawnLocation;
        player.GetComponentInChildren<NavMeshAgent>().Warp(spawnLocation);
        Resources.UnloadUnusedAssets();
    }


    IEnumerator Arrived(float Seconds, string currentSceneName)
    {
        yield return new WaitForSeconds(Seconds);
        if (gameObject.scene.name != currentSceneName)
        {
            LoadingFrame.GetComponent<Animator>().SetTrigger("End");
        }
        else
        {
            StartCoroutine(Arrived(0.5f, currentSceneName));
        }
    }


    #endregion


    [Client]
    void MakeBoldNameForLocalClient()
    {
        playerNameMesh.fontStyle = FontStyles.Bold;
        PlayerCircle.gameObject.SetActive(true);

    }

}
