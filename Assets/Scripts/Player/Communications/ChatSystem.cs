using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public void Start()
    {
        inputFiled = GameObject.Find("InputFieldChat").GetComponent<TMP_InputField>();
    }
    public void OnClick()
    {
        Send();
    }
    [Client]
    void Send()
    {
        string text = inputFiled.text;
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
            CmdSend(inputFiled.text);
            Invoke("CmdDelayedFunction", timetoClear);
        }
        else if (badword == true)
        {
            badword = false;
        }
        inputFiled.text = null;
        inputFiled.Select();
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