using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction lookValue;
    private InputAction aimAction;
    private InputAction shootAction;

    private AimingAndEquiping aimingAndEquiping;
    private Player player;

    public bool aiming;
    private int boostCameraAmount = 12;
    public CinemachineVirtualCamera aimCamera;

    public bool isShooting;
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        aimingAndEquiping = GetComponent<AimingAndEquiping>();

        moveAction = playerInput.actions["Move"];
        aimAction = playerInput.actions["Aim"];
        shootAction = playerInput.actions["Shoot"];
        lookValue = playerInput.actions["Look"];
    }
    private void Start()
    {
       
        player = GetComponent<Player>();
        

    }

    private void OnEnable()
    {
        Aiming();
        ShootAction();
    }
    public Vector2 MovementInput()
    {
        Vector2 inputVector = moveAction.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
      
        return inputVector;
    }

    public Vector2 LookInput() 
    {
        Vector2 look = lookValue.ReadValue<Vector2>();
        look = look.normalized;

        return look;
    }

    private void Aiming()
    {
        aimAction.performed += AimAction_performed;
        aimAction.canceled += AimAction_canceled;

    }

    private void AimAction_performed(InputAction.CallbackContext obj)
    {
       
        Weapon weapon = aimingAndEquiping.GetActiveWeapon();
        if (weapon != null && !player.isDead)
        {
            aiming = true;
            AImIn();
        }
    }

    public void AimAction_canceled(InputAction.CallbackContext obj)
    {
        if (aiming)
        {
            aiming = false;

            AimOut();
        }
         

    }

    void AImIn()
    { 
            aimCamera.Priority += boostCameraAmount;
    }

    void AimOut()
    {
        if (!player.isDead)
        {
           
            aimCamera.Priority -= boostCameraAmount;
        }

    }
    public bool isAiming()
    {
        return aiming;
    }

    private void ShootAction()
    {
        shootAction.performed += ShootAction_performed;
        shootAction.canceled += ShootAction_canceled;
    }

    private void ShootAction_canceled(InputAction.CallbackContext obj)
    {
        isShooting = false;
    }

    private void ShootAction_performed(InputAction.CallbackContext obj)
    {
        isShooting = true;

    }
    /* public void LegacyInput()
     {
       Vector2 inputVector = new Vector2(0, 0);
         if (Input.GetKey(KeyCode.W))
         {
             inputVector.y = +1;

         }


         if (Input.GetKey(KeyCode.A))
         {
             inputVector.x = -1;

         }


         if (Input.GetKey(KeyCode.D))
         {
             inputVector.x = +1;


         }

         if (Input.GetKey(KeyCode.S))
         {
             inputVector.y = -1;

         }

     }*/

}
