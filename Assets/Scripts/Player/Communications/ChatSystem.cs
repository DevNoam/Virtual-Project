using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

public class ChatSystem : NetworkBehaviour
{

    //Texting:

    public TMP_InputField inputFiled;

    public TMP_Text playerText;
    [SyncVar]
    string playermessage;

    public float timetoClear = 4f;
    public GameObject ChatCanvas;

    public void Start()
    {
        inputFiled = GameObject.Find("InputFieldChat").GetComponent<TMP_InputField>();
    }
    public void OnClick()
    {
        if (inputFiled.text != "")
        {
            Send();
        }
    }
    [Client]
    void Send()
    {
        if (IsInvoking("CmdDelayedFunction") == true)
        {       
            CancelInvoke("CmdDelayedFunction");
        }
        playermessage = inputFiled.text;
        //Check the database for bad words.
        CmdSend(playermessage);
        Invoke("CmdDelayedFunction", timetoClear);
        inputFiled.Select();
        inputFiled.text = "";
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
    }
    [Client]
    void OnGUI()
    {
        if (inputFiled.isFocused && isLocalPlayer && inputFiled.text != "" && Input.GetButtonDown("Submit"))
            {
                Send();
            }
    }
    [Command]
    void CmdDelayedFunction()
    {
        //playerText.text = "";
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
