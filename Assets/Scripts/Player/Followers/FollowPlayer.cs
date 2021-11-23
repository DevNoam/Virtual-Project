using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using Mirror;

public class FollowPlayer : NetworkBehaviour
{ 
    [SerializeField]
    private GameObject following;
        
    public Camera cam;
    [Range(0.0f, 150.0f)]
    [SerializeField]
    private float interested; 
    [SerializeField]
    private Vector3 followPosition;

    [ClientCallback]
    public void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    [ClientCallback]
    void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, following.transform.position + followPosition, interested);
        transform.LookAt(transform.position + cam.transform.forward);
    }

}
