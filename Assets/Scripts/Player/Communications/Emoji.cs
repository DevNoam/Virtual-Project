using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
public class Emoji : MonoBehaviour
{

    public TMP_InputField inputFiled;
    public string spriteName;

    [Client]
    public void Click()
    {
        inputFiled.text = inputFiled.text + spriteName;
    }
}
