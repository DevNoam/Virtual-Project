using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class RoomsManager : NetworkBehaviour
{
    [SerializeField]
    private PlayerManager playerManager;

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
        playerManager.chatSystem.inputFiled.text = null;
        if (isLocalPlayer && DoesSceneExist(RoomName) == true)
        {
            if (RoomName.ToUpper() != currentSceneName.ToUpper())
            {
                StartCoroutine(Arrived(1, currentSceneName));
                playerManager.LoadingFrame.SetActive(true);
                playerManager.LoadingFrame.GetComponent<Animator>().SetTrigger("Start");
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
            SceneManager.MoveGameObjectToScene(playerManager.playerMovement.player.gameObject, SceneManager.GetSceneByName(RoomName));
        }
        //Reposition player
        playerManager.playerMovement.Warp(spawnLocation);

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

        playerManager.chatSystem.CmdDelayedFunction();

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
            SceneManager.MoveGameObjectToScene(playerManager.playerMovement.player.gameObject, SceneManager.GetSceneByName(RoomName));
        }
        //Reset text bubble
        playerManager.chatSystem.CmdDelayedFunction();

        //Position player
        //player.transform.position = spawnLocation;
        playerManager.playerMovement.ClientWarp(spawnLocation);
        Resources.UnloadUnusedAssets();
    }


    IEnumerator Arrived(float Seconds, string currentSceneName)
    {
        yield return new WaitForSeconds(Seconds);
        if (gameObject.scene.name != currentSceneName)
        {
            playerManager.LoadingFrame.GetComponent<Animator>().SetTrigger("End");
        }
        else
        {
            StartCoroutine(Arrived(0.5f, currentSceneName));
        }
    }
}
