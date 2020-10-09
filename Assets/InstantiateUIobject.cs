using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateUIobject : MonoBehaviour
{
    public GameObject theObject;
    public GameObject Canvas;
    private GameObject OpenedObject;


    public void Instantiate()
    {
        if (OpenedObject == null)
        {
            GameObject Canvas = Instantiate(theObject) as GameObject;
            OpenedObject = transform.Find(theObject.name.ToString() + "(Clone)").gameObject;
        }
        else if (OpenedObject != null)
        {
            Destroy(OpenedObject);
            Instantiate(theObject);
            GameObject Canvas = Instantiate(theObject) as GameObject;
            OpenedObject = transform.Find(theObject.name.ToString()).gameObject;
        }
    
    }
}
