using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.SceneManagement;
public class RoomManager : MonoBehaviour
{
    public PlayerManager LocalPlayer;
    private string PlayerNameText;
    public PlayFabLogin playfabLogin;
    public int targetFrameRate = 60;
    private void Start()
    {
        Application.targetFrameRate = targetFrameRate;
    }

    void LateUpdate()
    {
        if (NetworkManager.singleton.isNetworkActive)
        {
            if (LocalPlayer == null)
            {
                if (ClientScene.localPlayer == null)
                    return;

                LocalPlayer = ClientScene.localPlayer.GetComponent<PlayerManager>();
                if (LocalPlayer != null)
                {
                    ThisPlayerJoined();
                    Invoke("ThisPlayerJoined", 0.1f);
                }
            }
            else
            {

            }
        }
        else
        {
        }
    }


    public void SendNameToClients()
    {
        LocalPlayer.SendName();
    }

    void ThisPlayerJoined()
    {
        GetPlayerProfileRequest request = new GetPlayerProfileRequest();
        PlayFabClientAPI.GetPlayerProfile(request, Successs, fail);
    }

    public void Successs(GetPlayerProfileResult result)
    {
        string getName = result.PlayerProfile.DisplayName;

        LocalPlayer.CmdReady(getName);
    }
    public void fail(PlayFabError error)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
