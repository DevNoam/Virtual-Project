﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutomaticllyConnectToServer : MonoBehaviour
{
    NetworkManager manager;
    AutomaticllyConnectToServer thisScript;
    NetworkManagerHUD managerGUI;

    void Start()
    {
        thisScript = this;
        manager = GetComponent<NetworkManager>();
        managerGUI = this.GetComponent<NetworkManagerHUD>();
        if (NetworkClient.active)
            thisScript.enabled = false;

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            manager.StartClient();
            thisScript.enabled = false;
            managerGUI.showGUI = false;
        }
    }
}
