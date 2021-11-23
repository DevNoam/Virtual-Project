using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AnimationsManager : NetworkBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private NetworkAnimator networkAnimator;

    [SerializeField]
    private PlayerManager playerManager;

    [ClientCallback]
    void Update() //Call Animations
    {
        if (Input.GetKeyDown("w") && hasAuthority && !playerManager.chatSystem.inputFiled.isFocused && (playerManager.playerMovement.navMeshController.remainingDistance <= playerManager.playerMovement.navMeshController.stoppingDistance))
        {
            playerManager.animationsManager.AnimationWaving();
        }
    }

    /////Animations
    public void AnimationWalk(float speed)
    {
        animator.SetFloat("Moving", speed);
    }
    public void AnimationStopMoving()
    {
        animator.SetFloat("Moving", 0f);
    }
    public void AnimationWaving()
    {
        animator.SetTrigger("Wave");
        networkAnimator.SetTrigger("Wave");
    }
}
