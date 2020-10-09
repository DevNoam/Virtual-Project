using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;
using UnityEngine.Networking;


public class ChatSystem : NetworkBehaviour
{

    //Texting:

    public TMP_InputField inputFiled;

    public TMP_Text playerText;
    [SyncVar]
    string playermessage;

    public float timetoClear = 4f;
    public GameObject ChatCanvas;

    public string[] badWords;

    public GameObject chatLogInstantiate;
    public Transform chatLog;
    public string playerName;
    public CommandsManager commandsManager;

    public void Start()
    {
        inputFiled = GameObject.Find("InputFieldChat").GetComponent<TMP_InputField>();
        chatLog = GameObject.Find("ContentChatLog").GetComponent<Transform>();
    }

    [Client]
    public void Send()
    {
        string text = inputFiled.text;
        bool isCommand = false;
        if (text.Trim().Length >= 1)
        {
            if (text.StartsWith("/"))
            {
                commandsManager.ReceivedCommand(inputFiled.text);

                isCommand = true;
            }
            if (isCommand != true)
            {
                bool badword = false;

                for (int i = 0; i < badWords.Length; i++)
                {
                    if (text.ToLower().Contains(badWords[i]))
                    {
                        Debug.Log("Bad Word detected!");
                        badword = true;
                    }
                }
                if (badword == false)
                {
                    if (IsInvoking("CmdDelayedFunction") == true)
                    {
                        CancelInvoke("CmdDelayedFunction");
                    }
                    CmdSend(text.TrimStart());
                    Invoke("CmdDelayedFunction", timetoClear);
                }
                else if (badword == true)
                {
                    badword = false;
                }
            }
            //Clear InputFiled
            inputFiled.text = null;
            inputFiled.Select();
        }
    }


    [Command]
    void CmdSend(string message)
    {
        //Check the database for bad words.
        playerText.text = message;
        RpcSend(message);
        ChatCanvas.SetActive(true); //
    }

    [ClientRpc]
    void RpcSend(string message)
    {
        playerText.text = message;
        ChatCanvas.SetActive(true);
        
        //----CHAT LOG----//
        GameObject ChatLogObject = Instantiate(chatLogInstantiate) as GameObject;
        ChatLogObject.GetComponentInChildren<TMP_Text>().text = "<b>" + playerName + ":</b> " + message;
        //Assign player profile when clicking the button
        ChatLogObject.transform.SetParent(chatLog.transform, false);

        if (chatLog.transform.childCount >= 50)
        {
            GameObject.Destroy(chatLog.transform.GetChild(1).gameObject);
        }
    }

    void OnGUI()
    {
        if (inputFiled.isFocused && isLocalPlayer && inputFiled.text != "" && Input.GetButtonDown("Submit"))
            {
                Send();
            }
    }
    [Command]
    public void CmdDelayedFunction()
    {
        if (IsInvoking("CmdDelayedFunction") == true)
        {
            CancelInvoke("CmdDelayedFunction");
        }
        ChatCanvas.SetActive(false);
        RpcDelayedFunction();
    }
    [ClientRpc]
    void RpcDelayedFunction()
    {
        // playerText.text = "";
        ChatCanvas.SetActive(false);
    }
}