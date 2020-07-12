﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ButtonOnClick : MonoBehaviour
{
    // Start is called before the first frame update    

    public void Click()
    {
        ChatSystem player = NetworkClient.connection.identity.GetComponent<ChatSystem>();
        player.OnClick();
    }
}
