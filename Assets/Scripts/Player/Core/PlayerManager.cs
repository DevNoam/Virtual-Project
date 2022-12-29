using UnityEngine;
using Mirror;
using TMPro;

public class PlayerManager : NetworkBehaviour
{
    public Transform player;
    /// Player Name

    [SyncVar(hook = nameof(updatePlayerName))]
    private string playerName = "Loading..";
    public string PlayerName {
        get { return playerName; }
    }

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
        NewNetworkManager.singleton.ConnectPlayer(playerName, this.GetComponent<NetworkIdentity>());
        Debug.Log(playername + " connected to the server.");
    }
    public override void OnStopClient()
    {
        base.OnStopClient();
    }

    private void updatePlayerName(string oldName, string newName)
    {
        playerNameMesh.text = newName;
        chatSystem.playerName = newName;
    }
    [TargetRpc]
    public void InformClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("Logged in from other client.");
        //NetworkClient.Disconnect();
        NewNetworkManager.singleton.StopClient();
    }
    #endregion
}
