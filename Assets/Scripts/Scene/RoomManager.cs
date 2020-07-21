using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;
public class RoomManager : MonoBehaviour
{
    public PlayerManager LocalPlayer;
    private string PlayerNameText;
    public PlayFabLogin playfabLogin;


    void LateUpdate()
    {
        if (NetworkManager.singleton.isNetworkActive)
        {
            if (LocalPlayer == null)
            {
                if (ClientScene.localPlayer == null)
                    return;

                LocalPlayer = ClientScene.localPlayer.GetComponent<PlayerManager>();
            }
            else
            {
                
            }
        }
    }


    private string getName;

    public void Successs(GetPlayerProfileResult result)
    {
        getName = result.PlayerProfile.DisplayName;

        LocalPlayer.SendReadyToServer(getName);
        playfabLogin.loginPanel.SetActive(false);
    }
    public void fail(PlayFabError error)
    {

        Debug.LogError(error.GenerateErrorReport());
    }

}
