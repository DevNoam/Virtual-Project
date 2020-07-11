using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FollowPlayer : MonoBehaviour
{

    public GameObject following;
    [Range(0.0f, 1.0f)]
    public float interested; 
    public Vector3 followPosition;
    Transform cam;

    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Transform>();
    }

    void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, following.transform.position + followPosition, interested);
        transform.LookAt(transform.position + cam.forward);
    }

}
