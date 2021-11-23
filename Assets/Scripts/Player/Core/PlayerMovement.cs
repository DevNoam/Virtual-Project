using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerMovement : NetworkBehaviour
{
    public NavMeshAgent navMeshController;
    public Camera cam;
    public Transform player;

    [SerializeField]
    private float rotationSpeed = 20;
    private bool CanRotate = true;
    private float heledLevel = 0;
    private bool isHeled = false;
    private bool isMoving = false;
    [SerializeField]
    private int pressSensive = 25;

    public int movementype = 1;

    [SerializeField]
    private PlayerManager playerManager;

    [ClientCallback]
    void Update()
    {
        if (!hasAuthority) { return; }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
#if UNITY_EDITOR
        if (Physics.Raycast(ray, out hit) && !EventSystem.current.IsPointerOverGameObject())
#elif UNITY_ANDROID || UNITY_IOS
            if (Physics.Raycast(ray, out hit) && !EventSystem.current.IsPointerOverGameObject(0))
#else
        if (Physics.Raycast(ray, out hit) && !EventSystem.current.IsPointerOverGameObject())
#endif
        {
            if (Input.GetMouseButton(0) && hit.transform.tag != "MouseHitCollider")
            {
                if (heledLevel < pressSensive && isHeled == false)
                {
                    if (navMeshController.angularSpeed > 500)
                        navMeshController.angularSpeed = 500;
                //navMeshController.SetDestination(hit.point);
                CmdScrPlayerSetDestination(hit.point);
                CanRotate = false;
                //heledLevel += Time.deltaTime * 100;
                }
                /*else if (heledLevel >= pressSensive)
                {
                    //Debug.Log("Heled");
                    CmdScrPlayerSetDestination(hit.point);
                    if (isHeled == false && CanRotate == false)
                    {
                        isHeled = true;
                        //CanRotate = true;
                        //Debug.Log("CanRotate TRUE");
                    }
                }*/
                isMoving = true;
            }
        }
        /*if (Input.GetMouseButtonUp(0))
        {
            if (isHeled == false)
            {
                //Debug.Log("Released regular Movement");
                heledLevel = 0;
            }
            else if (isHeled == true)
            {
                isHeled = false;
                //navMeshController.angularSpeed = 500;
                //Debug.Log("Released from hold");
                if (movementype == 1)
                {
                    //CmdScrPlayerSetDestination(this.transform.position);
                }
                else if (movementype == 2)
                {
                    CmdScrPlayerSetDestination(hit.point);
                    CanRotate = false;
                }
                heledLevel = 0;
            }
        }*/

        if (isMoving == true)
        {
            if (navMeshController.remainingDistance > navMeshController.stoppingDistance)
            {
                playerManager.animationsManager.AnimationWalk(0.5f);
            }
            else if (navMeshController.remainingDistance <= navMeshController.stoppingDistance && CanRotate == false)
            {
                playerManager.animationsManager.AnimationStopMoving();
                CanRotate = true;
                isMoving = false;
            }
            else if (navMeshController.remainingDistance <= navMeshController.stoppingDistance)
            {
                playerManager.animationsManager.AnimationStopMoving();
                isMoving = false;
            }
        }

#if UNITY_EDITOR
        if (CanRotate == true && (Input.GetAxis("Mouse X") != 0 || CanRotate == true && Input.GetAxis("Mouse Y") != 0)) // Rotation
#elif UNITY_ANDROID || UNITY_IOS
        if (CanRotate == true && (Input.GetAxis("Mouse X") != 0 || CanRotate == true && Input.GetAxis("Mouse Y") != 0) && !EventSystem.current.IsPointerOverGameObject(0)) // Rotation
#else
        if (CanRotate == true && (Input.GetAxis("Mouse X") != 0 || CanRotate == true && Input.GetAxis("Mouse Y") != 0)) // Rotation
#endif
        {
            var lookPos = hit.point - player.transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }

    [Command]
    public void CmdScrPlayerSetDestination(Vector3 argPosition)
    {
        navMeshController.SetDestination(argPosition);
        RpcScrPlayerSetDestination(argPosition);
    }

    [ClientRpc] //Only the caller of the CMD Command will receive this callback
    public void RpcScrPlayerSetDestination(Vector3 argPosition)
    {
        navMeshController.SetDestination(argPosition);
        CanRotate = false;
        isMoving = true;
    }

    [Server]
    public void Warp(Vector3 spawnLocation)
    {
        navMeshController.Warp(spawnLocation);
    }
    [Client]
    public void ClientWarp(Vector3 spawnLocation)
    {
        navMeshController.Warp(spawnLocation);
    }
}