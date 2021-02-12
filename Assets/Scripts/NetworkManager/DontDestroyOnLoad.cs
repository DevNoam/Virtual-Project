using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DontDestroyOnLoad : MonoBehaviour
{
    // Start is called before the first frame update

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
