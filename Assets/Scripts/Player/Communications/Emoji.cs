using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
public class Emoji : NetworkBehaviour
{

    public TMP_InputField inputFiled;
    public string spriteName;

    [Client]
    public void Click()
    {
        inputFiled = GameObject.Find("InputFieldChat").GetComponent<TMP_InputField>();
        inputFiled.text = inputFiled.text + spriteName;
    }
}
