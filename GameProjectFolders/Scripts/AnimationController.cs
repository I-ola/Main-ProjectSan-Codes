using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AnimationController : MonoBehaviour
{

    private Animator animator;
    private Player player;
    private AimingAndEquiping characterAiming;
   


   

    private PlayerController playerInput;
    int isWalkingForwardHash;
    int isRunningHash;
    int isCrouchingHash;
    int crouchWalkHash;
    int strafeRightHash;
    int strafeLeftHash;
    int walkBackwardHash;
    int idleHash;
    int isShootingHash;



    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
       


        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerController>();
        
       // animator.SetBool(idleHash, isIdle);
    }

    // Update is called once per frame
    void Update()
    {

        animator.SetFloat("Horizontal", player.Horizontal());
        animator.SetFloat("Vertical", player.Vertical());
        animator.SetBool("crouch", player.Crouch());
        animator.SetBool("isAiming", playerInput.isAiming());
        animator.SetBool("crouchAim", (player.Crouch() && playerInput.isAiming()));
    }

}
