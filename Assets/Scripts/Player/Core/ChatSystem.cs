using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.EventSystems;

public class ChatSystem : NetworkBehaviour
{
    public TMP_InputField inputFiled;

    public TMP_Text playerText;

    public float timetoClear = 4f;
    public GameObject ChatCanvas;

    public string[] badWords;

    public GameObject chatLogInstantiate;
    public Transform chatLogContainer;
    public Transform chatLogParent;

    [HideInInspector]
    public string playerName;
    public PlayerManager playerManager;

    public void Start()
    {
        //inputFiled = GameObject.Find("InputFieldChat").GetComponent<TMP_InputField>();
        chatLogParent = GameObject.Find("SharedCanvasUI/ChatLog").GetComponent<Transform>();
        chatLogContainer = GameObject.Find("SharedCanvasUI/ChatLog/Panel/ChatLog Container/Viewport/ContentChatLog").GetComponent<Transform>();
    }

    [Client]
    public void Send()
    {
        string text = inputFiled.text;
        ChatMonitor(text, playerName);
        if (text.Trim().Length >= 1)
        {
            if (text.StartsWith("/"))
            {
                string command = inputFiled.text;
                //Clear the Inputfield.
                inputFiled.text = null;
                inputFiled.Select();

                playerManager.commandsManager.ReceivedCommand(command);
            }
            else
            {
                CallServerToCheckMessage(text, playerName);
            }
        }

        //Clear InputFiled
        inputFiled.text = null;
        EventSystem.current.SetSelectedGameObject(null);
        if (Input.GetKeyDown(KeyCode.Return))
        {
            inputFiled.Select();
        }
    }


    [Command]
    void CallServerToCheckMessage(string Message, string playerName)
    {
        ServerCheckMessage(Message, playerName);
    }
    [Command]
    void ChatMonitor(string Message, string playerName) //Sends the player input to the server terminal to be reviewd
    {
        Debug.Log($"{System.DateTime.Now}, {playerName}: {Message}");
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
                    //Debug.Log("Bad Word detected!");
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
                //Debug.Log($"{playerName}: {Message}"); //CLIENT MESSAGE WILL DEBUGGED TO THE CONSOLE.

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
        playerText.text = message;


        //----CHAT LOG----//
        GameObject ChatLogObject = Instantiate(chatLogInstantiate) as GameObject;

        ChatLogObject.GetComponentInChildren<TMP_Text>().text = "<b>" + playerName + ":</b> " + message;
        //Assign player profile URL when clicking the button

        ChatLogObject.transform.SetParent(chatLogContainer.transform, false);

        if (chatLogContainer.transform.childCount >= 50)
        {
            GameObject.Destroy(chatLogContainer.transform.GetChild(1).gameObject);
        }

        //Active the TextBubble above the client.
        ChatCanvas.SetActive(true);

    }


    void OnGUI()
    {
        if (inputFiled.isFocused && isLocalPlayer && inputFiled.text != "" && Input.GetButtonDown("Submit"))
        {
            Send();
        }
    }



    [ServerCallback]
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