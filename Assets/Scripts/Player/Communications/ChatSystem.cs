using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class ChatSystem : NetworkBehaviour
{

    //Texting:

    public TMP_InputField inputFiled;

    public TMP_Text playerText;
    [SyncVar]
    string playermessage;

    public float timetoClear = 1.5f;
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
        if (EmojiCanvas == true)
        {
            CmdDelayedFunctionEmoji();
        }
        playermessage = inputFiled.text;
        //Check the database for bad words.
        CmdSend(playermessage);
        Invoke("CmdDelayedFunction", timetoClear);
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



    //Emojis:

    public Image playerEmojiBox;

    public GameObject EmojiCanvas;

    public void OnClickEmoji()
    {

        SendEmoji();
    }

    [Client]
    void SendEmoji()
    {
        if (ChatCanvas == true)
        { 
            CmdDelayedFunction();
        }

        CmdSendEmoji();
        Invoke("CmdDelayedFunctionEmoji", timetoClear);
    }

    [Command]
    void CmdSendEmoji()
    {
        //Check the database for bad words.
        RpcSendEmoji();
        EmojiCanvas.SetActive(true); //
    }

    [ClientRpc]
    void RpcSendEmoji()
    {
        EmojiCanvas.SetActive(true);
    }


    [Command]
    void CmdDelayedFunctionEmoji()
    {
        EmojiCanvas.SetActive(false);
        RpcDelayedFunctionEmoji();
    }
    [ClientRpc]
    void RpcDelayedFunctionEmoji()
    {
        EmojiCanvas.SetActive(false);
    }

}
