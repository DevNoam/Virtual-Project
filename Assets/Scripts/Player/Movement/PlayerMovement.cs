using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

public class PlayerMovement : NetworkBehaviour
{
    public NavMeshAgent navMeshController;

    public Camera cam;
    public Transform player;

    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;
    public bool followMouse = false;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        player.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {

            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    navMeshController.SetDestination(hit.point);
                }
            }
            Vector3 mousePos = Input.mousePosition;
            Vector3 playerPos = cam.WorldToScreenPoint(transform.position);

            Vector3 dir = mousePos - playerPos;

            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            player.transform.rotation = Quaternion.AngleAxis(-angle, Vector3.up);
        }
        else { return; }
    }
}
