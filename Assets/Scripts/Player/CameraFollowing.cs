using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class CameraFollowing : MonoBehaviour
{
    public Transform following;
    [Range(0.0f, 500)]
    public float interested;
    public Vector3 followPosition;
    Transform cam;

    void Start()
    {
        cam = this.transform;
    }

    void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, following.position + followPosition, interested);
        transform.LookAt(transform.position + cam.forward);
    }

}