using System.Collections;
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
        Application.targetFrameRate = 60;

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
                thisScript.enabled = false;
                managerGUI.showGUI = false;
            }
        }
    }

    public void StartClient()
    {
        manager.StartClient();
    }
}
