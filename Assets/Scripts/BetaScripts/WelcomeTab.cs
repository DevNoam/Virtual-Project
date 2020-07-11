using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
public class WelcomeTab : NetworkBehaviour
{
    public InputField inputFiled;


    public void OnClick()
    {
        PlayerManager playerManager = NetworkClient.connection.identity.GetComponent<PlayerManager>();
        playerManager.playerName = inputFiled.text;
        //playerManager.Send();
    }

}
