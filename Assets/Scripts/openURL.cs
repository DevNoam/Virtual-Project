using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openURL : MonoBehaviour
{
    public void OpenURL(string link)
    {
        Application.OpenURL(link);
    }
}
