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
    [SerializeField] [Tooltip("Attach MovableComponenets")]
    private Transform transformComponents;

    [ClientCallback]
    public void Start()
    {
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    [ClientCallback]
    void LateUpdate()
    {
        transformComponents.position = Vector3.MoveTowards(transformComponents.position, following.transform.position + followPosition, interested);
        transformComponents.LookAt(transformComponents.position + cam.transform.forward);
    }

}
