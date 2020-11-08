using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using UnityEngine.Networking;


public class ChatSystem : NetworkBehaviour
{

    public TMP_InputField inputFiled;

    public TMP_Text playerText;
    [SyncVar]
    string playermessage;

    public float timetoClear = 4f;
    public GameObject ChatCanvas;

    public string[] badWords;

    public GameObject chatLogInstantiate;
    public Transform chatLogContainer;
    public Transform chatLogParent;
    [HideInInspector]
    public string playerName;
    public CommandsManager commandsManager;

    public void Start()
    {
        //inputFiled = GameObject.Find("InputFieldChat").GetComponent<TMP_InputField>();
        //chatLog = GameObject.Find("ContentChatLog").GetComponent<Transform>();
    }

    [Client]
    public void Send()
    {
        string text = inputFiled.text;
        if (text.Trim().Length >= 1)
        {
            if (text.StartsWith("/"))
            {
                string command = inputFiled.text;
                //Clear the Inputfield.
                inputFiled.text = null;
                inputFiled.Select();

                commandsManager.ReceivedCommand(command);
            }
            else
            {
                CallServerToCheckMessage(text, playerName);
            }
        }

        //Clear InputFiled
        inputFiled.text = null;
        inputFiled.Select();
    }

    [Command]
    void CallServerToCheckMessage(string Message, string playerName)
    {
        ServerCheckMessage(Message, playerName);
    }


    [Server]
    void ServerCheckMessage(string Message, string playerName)
    {
        string text = Message;
        if (text.Trim().Length >= 1)
        {
            bool badword = false;

            for (int i = 0; i < badWords.Length; i++)
            {
                if (text.ToLower().Contains(badWords[i]))
                {
                    Debug.Log("Bad Word detected!");
                    //Send warning to Client, If warning extended the limit, Mute the Client.
                    //This field is for Playfab.
                    badword = true;
                }
            }
            if (badword == false)
            {
                if (IsInvoking("CmdDelayedFunction") == true)
                {
                    CancelInvoke("CmdDelayedFunction");
                }
                playerText.text = Message;
                RpcSendGlobal(playerName, text.TrimStart());
                Debug.Log($"{playerName}: {Message}"); //CLIENT MESSAGE WILL BE DEBUGGED ON THE CONSOLE TO MONITOR THE CHAT.

                Invoke("CmdDelayedFunction", timetoClear);
            }
            else if (badword == true)
            {
                badword = false;
            }
        }
    }

    [ClientRpc]
    void RpcSendGlobal(string playerName, string message)
    {
        Debug.Log("Message arrived to all clients");

        playerText.text = message;
        ChatCanvas.SetActive(true);


        //----CHAT LOG----//
        GameObject ChatLogObject = Instantiate(chatLogInstantiate) as GameObject;
        ChatLogObject.GetComponentInChildren<TMP_Text>().text = "<b>" + playerName + ":</b> " + message;
        //Assign player profile URL when clicking the button
        ChatLogObject.transform.SetParent(chatLogContainer.transform, false);

        if (chatLogContainer.transform.childCount >= 50)
        {
            GameObject.Destroy(chatLogContainer.transform.GetChild(1).gameObject);
        }
    }

    void OnGUI()
    {
        if (inputFiled.isFocused && isLocalPlayer && inputFiled.text != "" && Input.GetButtonDown("Submit"))
            {
                Send();
            }
    }



    [Server]
    public void CmdDelayedFunction()
    {
        if (IsInvoking("CmdDelayedFunction") == true)
        {
            CancelInvoke("CmdDelayedFunction");
        }
        //ChatCanvas.SetActive(false); //DISABLED BECAUSE THE SERVER DOES NOT NEED TO RECEIVE TEXT'S FROM THE CLIENTS ANYMORE.
        RpcDelayedFunction();
    }
    [ClientRpc]
    void RpcDelayedFunction()
    {
        // playerText.text = "";
        ChatCanvas.SetActive(false);
    }
}