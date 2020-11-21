using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyGameObject : MonoBehaviour
{
    public int DestoryIn = 0;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("autoDestroy", DestoryIn);
    }

    void autoDestroy()
    {
        Destroy(this.gameObject);
    }
}
