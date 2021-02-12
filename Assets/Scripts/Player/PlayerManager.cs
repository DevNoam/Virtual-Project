﻿using System.Collections;
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

    [SerializeField]
    private GameObject playerObject;
    /// Player Name

    [SyncVar(hook = nameof(updatePlayerName))]
    public string playerName = "Loading..";

    [SyncVar(hook = nameof(updatePlayerColor))]
    [SerializeField]
    private int SkinColor = 1;

    [SerializeField]
    private TextMeshPro playerNameMesh;

    [SerializeField]
    private TextMeshPro PlayerCircle;

    public RoomManager roomManager;
    [SerializeField]
    private GameObject canvas;
    public GameObject LoadingFrame;

    [SerializeField]
    private float rotationSpeed = 20;
    public ChatSystem chatSystem;

    [HideInInspector]
    public bool isModderator = false;

    [Tooltip("MovementType is how the local player will move: True = Hold to move. False = Click to move to destination")]
    private bool CanRotate = true;
    private float heledLevel = 0;
    private bool isHeled = false;
    [SerializeField]
    private int pressSensive = 25;

    public int movementype = 1;


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
            /*GameObject followPlayer = this.transform.Find("MoveableComponents").gameObject;
            Destroy(followPlayer.gameObject);*/
        }
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        cam.gameObject.tag = "MainCamera";
    }
    #region Movement

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority) { return; }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButton(0) && hit.transform.tag != "MouseHitCollider")
            {
                //CmdScrPlayerSetDestination(hit.point);
                if (heledLevel < pressSensive && isHeled == false)
                {
                    if (navMeshController.angularSpeed > 500)
                        navMeshController.angularSpeed = 500;
                    CmdScrPlayerSetDestination(hit.point);
                    CanRotate = false;
                    heledLevel += Time.deltaTime * 100;
                }
                else if (heledLevel >= pressSensive)
                {
                    Debug.Log("Heled");
                    CmdScrPlayerSetDestination(hit.point);
                    if (isHeled == false && CanRotate == false)
                    {
                        //navMeshController.angularSpeed = 0;
                        isHeled = true;
                        CanRotate = true;
                        Debug.Log("CanRotate TRUE");
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (isHeled == false)
            {
                Debug.Log("Released regular Movement");
                heledLevel = 0;
            }
            else if (isHeled == true)
            {
                isHeled = false;
                //navMeshController.angularSpeed = 500;
                Debug.Log("Released from hold");
                if (movementype == 1)
                {
                    navMeshController.ResetPath();

                }
                else if (movementype == 2)
                {
                    navMeshController.SetDestination(player.transform.position);
                }
                else if (movementype == 3)
                {
                    navMeshController.SetDestination(hit.point);
                    CanRotate = false;
                }
                //CanRotate = false;
                heledLevel = 0;
            }
        }
        if (CanRotate == true && Input.GetAxis("Mouse X") != 0 || CanRotate == true && Input.GetAxis("Mouse Y") != 0) // Rotation
        {
            var lookPos = hit.point - player.transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
        if (navMeshController.remainingDistance < navMeshController.stoppingDistance && CanRotate == false)
        {
            CanRotate = true;
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
        navMeshController.SetDestination(argPosition);

    }
    #endregion

    #region Player Name management

    [Command] 
    public void CmdUpdatePlayerName(string playername, int color) 
    {
        playerName = playername;
        SkinColor = color;
    }

    private void updatePlayerName(string oldName, string newName)
    {
        playerName = newName;
        playerNameMesh.text = playerName;
        chatSystem.playerName = playerName;
        Debug.Log("updatePlayerName!");
    }
    private void updatePlayerColor(int oldColor, int newColor)
    {
        SkinColor = newColor;
        playerObject.GetComponent<Renderer>().material = roomManager.skins[SkinColor];
    }


    [Command]
    public void CmdScrPlayerName(Vector3 playerPosition)
    {
        navMeshController.Warp(playerPosition);
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
        chatSystem.inputFiled.text = null;
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
            SceneManager.MoveGameObjectToScene(player.gameObject, SceneManager.GetSceneByName(RoomName));
        }
        //Reposition player
        navMeshController.Warp(spawnLocation);

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

        chatSystem.CmdDelayedFunction();

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
            SceneManager.MoveGameObjectToScene(player.gameObject, SceneManager.GetSceneByName(RoomName));
        }
        //Reset text bubble
        chatSystem.CmdDelayedFunction();

        //Position player
        //player.transform.position = spawnLocation;
        navMeshController.Warp(spawnLocation);
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
