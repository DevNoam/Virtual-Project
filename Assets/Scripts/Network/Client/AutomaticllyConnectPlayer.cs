﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AutomaticllyConnectPlayer : MonoBehaviour
{
    NetworkManager manager;
    AutomaticllyConnectPlayer thisScript;
    NetworkManagerHUD managerGUI;
    void Start()
    {
        thisScript = GameObject.Find("NetworkManager").GetComponent<AutomaticllyConnectPlayer>();
        manager = GetComponent<NetworkManager>();
        managerGUI = GameObject.Find("NetworkManager").GetComponent<NetworkManagerHUD>();
        if (NetworkClient.active)
            thisScript.enabled = false;

        if (!NetworkClient.active)
        {
            // Server Only
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                manager.StartClient();
                thisScript.enabled = false;
                managerGUI.showGUI = false;
            }
        }
    }
}
