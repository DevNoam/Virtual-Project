using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
public class RoomManager : MonoBehaviour
{
    public PlayerManager LocalPlayer;
    public TMP_InputField PlayerNameText;

    void Update()
    {
        if (NetworkManager.singleton.isNetworkActive)
        {
            if (LocalPlayer == null)
            {
                FindLocalPlayer();
            }
            else
            {

            }
        }
    }

    void FindLocalPlayer()
    {
        //Check to see if the player is loaded in yet
        if (ClientScene.localPlayer == null)
            return;

        LocalPlayer = ClientScene.localPlayer.GetComponent<PlayerManager>();
    }

    public void ReadyButtonHandler()
    {
        LocalPlayer.SendReadyToServer(PlayerNameText.text);
    }

}
