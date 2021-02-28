using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using TMPro;

public class PlayerManager : NetworkBehaviour
{
    public Transform player;
    /// Player Name

    [SyncVar(hook = nameof(updatePlayerName))]
    private string playerName = "Loading..";

    [SerializeField]
    private TextMeshPro playerNameMesh;

    [SerializeField]
    private TextMeshPro PlayerCircle;

    [SerializeField]
    private GameObject canvas;
    public GameObject LoadingFrame;


    [HideInInspector]
    public bool isModderator = false;

    public int movementype = 1;

    public ChatSystem chatSystem;
    public PlayerMovement playerMovement;
    public RoomsManager roomsManager;
    public CommandsManager commandsManager;
    public AnimationsManager animationsManager;
    public PlayerPlayFab playerPlayFab;

    void Start()
    {
        if (!hasAuthority)
        {
            Destroy(playerMovement.cam.gameObject);
            Destroy(canvas);
            canvas = null;
            playerMovement.cam = null;
        }
        if (hasAuthority)
        {
            MakeBoldNameForLocalClient();
            LoadingFrame = GameObject.Find("LoadingCanvas/LoadingSplashScreen").gameObject;
            LoadingFrame.GetComponent<Animator>().SetTrigger("End");
        }
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        playerMovement.cam.gameObject.tag = "MainCamera";
    }

    [Client]
    void MakeBoldNameForLocalClient()
    {
        playerNameMesh.fontStyle = FontStyles.Bold;
        PlayerCircle.gameObject.SetActive(true);

    }

    #region Name Management
    [Command]
    public void CmdUpdatePlayerName(string playername)
    {
        playerName = playername;
    }

    private void updatePlayerName(string oldName, string newName)
    {
        playerNameMesh.text = newName;
        chatSystem.playerName = newName;
    }
    #endregion
}
