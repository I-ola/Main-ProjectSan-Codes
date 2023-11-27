using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using static AimingAndEquiping;

public class Player : MonoBehaviour
{
    private PlayerController control;
    private ReloadWeapon reload;
  //  private PlayerSound sound;
    private SoundManager.SoundsCategory soundCategory;

    [SerializeField] private GameObject cameraTarget;

    [SerializeField] Transform orientation;
    private AimingAndEquiping weaponManager;
   
    public GameObject riflePickupWidget;
    public GameObject pistolPickupWidget;
 
    private float horizontal;
    private float vertical;
    Vector3 movDir;

    GameObject collidedWeapon;

    private Key key;
    private Door door;
    public Transform doorChecker;
    
    
    private float startYScale;
    private float startCenter;

    public float cameraTargetYaw;
    public float cameraTargetPitch;
    private float sensitivity;
    public bool isDead = false;
   
    private float speed;
    private float walkSpeed = 5f;
    private float runSpeed = 7f;
    private float crouchSpeed = 2f;

    private bool crouch = false;
    bool walking;
    bool running;
    bool crouchWalking;


    public bool hitPortal = false;
    CharacterController characterController;
    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching
    }
   
    void Start()
    {
        //rb = this.GetComponent<Rigidbody>();
        control = this.GetComponent<PlayerController>();
        weaponManager = GetComponent<AimingAndEquiping>();
        reload = GetComponent<ReloadWeapon>();
        characterController = GetComponent<CharacterController>();
       // sound = GetComponent<PlayerSound>();
        cameraTargetYaw = cameraTarget.transform.rotation.eulerAngles.y;

        startYScale = characterController.height;
        startCenter = characterController.center.y;
        
    }

    private void Update()
    {

       if(!isDead)
       {
            MovePlayer();
            CrouchManager();
            StateHandler();
            PickAndDropWeapon();
            MovementBools();
            OpenDoor();
            SoundBasedOnMovement();
           
       }



    }

    private void LateUpdate()
    {
        if(!isDead)
            CameraMovement();
    }


    private void CrouchManager()
    {
        float newHeight = startYScale / 2f;
        float anotherHeight = startYScale / 1.25f;
        //start crouch
        if (crouch && !walking)
        {

            characterController.height = newHeight;
            characterController.center = new Vector3(characterController.center.x, startCenter / 2f, characterController.center.z);
        }
        else if (crouchWalking)
        {

            characterController.height = anotherHeight;
            characterController.center = new Vector3(characterController.center.x, startCenter / 1.25f, characterController.center.z);

        }
        else
        {

            characterController.height = startYScale;
            characterController.center = new Vector3(characterController.center.x, startCenter, characterController.center.z);
        }
    }
    
    //this handles the speed of player movement based on the key pressed and player state
    private void StateHandler()
    {
        if(Input.GetKey(KeyCode.LeftShift) && !control.isAiming() && vertical == 1 && !crouch)
        {
            state = MovementState.sprinting;
            speed = runSpeed;
            vertical += 1f;
            running = true;
        }
        else
        {
            state = MovementState.walking;
            speed = walkSpeed;
            running = false;
           
        }

        if (Input.GetKey(KeyCode.C))
        {
            state = MovementState.crouching;
            speed = crouchSpeed;
            crouch = true;

        }
        else if(Input.GetKeyUp(KeyCode.C)) 
        {
            crouch = false;
        }
    }

    //this handles player movement and rotates and move the player based on the camera position
    private void MovePlayer()
    {
        Vector2 inputVector = control.MovementInput();

        movDir = new Vector3(inputVector.x, 0f, inputVector.y);

        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();
        Vector3 right = Vector3.Cross(Vector3.up, cameraForward);

        Vector3 move = cameraForward * movDir.z + right * movDir.x;

        //transform.position += move * speed * Time.deltaTime;
        characterController.Move(move * speed * Time.deltaTime);

        Quaternion targetRotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5f);

        

        horizontal = movDir.x;
        vertical = movDir.z;
     

    }
   
    //This controls the movement of the cinemachine camera based on the mouse action 
    private void CameraMovement()
    {
        Vector2 look = control.LookInput();
        cameraTargetYaw += look.x * 1.0f * sensitivity;
        cameraTargetPitch += look.y * -1.0f *sensitivity;

        cameraTargetYaw = Clamp(cameraTargetYaw, float.MinValue, float.MaxValue);
        cameraTargetPitch = Clamp(cameraTargetPitch, -30.0f, 70.0f);

        cameraTarget.transform.rotation = Quaternion.Euler(cameraTargetPitch, cameraTargetYaw, 0.0f);

    }

    // this is used to clamp the camera movement between 360 degrees so the camera won't move out of range
    private float Clamp(float lf, float min, float max)
    {
        if (lf > 360)
            lf -= 360;
        if (lf < -360)
            lf += 360;
        
        return Mathf.Clamp(lf, min, max);
    }
    public void SetSensitivity(float newValue)
    {
        sensitivity = newValue;
    }

    void MovementBools()
    {
        if ((vertical == -1 || vertical == 1) || (horizontal == -1 || horizontal == 1))
        {
            walking = true;
        }
        else
        {
            walking = false;
        }

        if(crouch && walking)
        {
            crouchWalking = true;
        }
        else
        {
            crouchWalking= false;
        }
    }
     
    public float Horizontal()
    {
        return horizontal;
    }
    public float Vertical()
    {
        return vertical;
    }

    public bool Crouch()
    {
        return crouch;
    }

    //this gets if the player has collided with a weapon and stores the weapon in an empty gameobject variable

    void CollidedWeaponIconEnabled()
    {
        Weapon weapon = collidedWeapon.GetComponent<Weapon>();
        if (weapon)
        {
            if (weapon.weaponName == "Rifle")
            {
                riflePickupWidget.SetActive(true);
                pistolPickupWidget.SetActive(false);
            }

            if (weapon.weaponName == "Pistol")
            {
                pistolPickupWidget.SetActive(true);
                riflePickupWidget.SetActive(false);
            }
        }
    }

    void CollidedWeaponDisabled(Weapon weapon)
    {
        if (weapon)
        {
            if (weapon.weaponName == "Rifle")
            {
                riflePickupWidget.SetActive(false);
            }

            if (weapon.weaponName == "Pistol")
            {
                pistolPickupWidget.SetActive(false);
            }
        }
    }

    IEnumerator DisableWeapon(GameObject collidedWeapon)
    {
        yield return new WaitForSeconds(4);
        Weapon weapon = collidedWeapon.GetComponent<Weapon>();
        if (weapon)
        {
            CollidedWeaponDisabled(weapon);
        } 
    }
    //This handles Weapon pickup and dropping 
    private void PickAndDropWeapon()
    {
        Vector3 dist;
        if (collidedWeapon != null && Input.GetKeyDown(KeyCode.E))
        {
            dist = this.transform.position - collidedWeapon.transform.position;

            if (dist.magnitude <= 3f)
            {
                
                PickWeapon(collidedWeapon);
                // Debug.Log("picked");

            }
            collidedWeapon = null;
        }
        
        else if(Input.GetKeyDown(KeyCode.E) && collidedWeapon == null && !reload.isReloading) 
        {
            DropEquippedWeapon();
            control.aiming = false;
            control.aimCamera.Priority = 9;
            //Debug.Log("droppedWeapon");
        }
    }


    // this helps to assign and store used ammo the weapon available ammo is assigned to the value of the ammo on player which can either be secondary or primary
    //the 2 different ammo for the different weapons primary and secondary and are assigne to the weapons based on the type assigned in the weapon prefab
    //when the weapon is fired we then assign the new amount of the weapon available ammo in an empty var and then equate the players own ammo to those values so when player picks or passes over a weapono of the same type it will just add its own personal values to the current player ammo
    private void CalculateAmmo(GameObject weaponObj)
    {
        Weapon weapon = weaponObj.GetComponent<Weapon>();
        int newWeaponIndex = (int)weapon.weaponSlots;
        Weapon oldWeapon = weaponManager.GetWeapon(newWeaponIndex);
        switch (weapon.weaponType)
        {
            case WeaponType.Primary:
                if(oldWeapon != null)
                {
            
                  
                    weaponManager.primaryWeaponAmmo += weapon.availableAmmo;
                    weapon.availableAmmo = 0;
                    oldWeapon.availableAmmo = weaponManager.primaryWeaponAmmo;
                    weaponManager.ammo.Refresh(oldWeapon.ammoCount, oldWeapon.magazineSize, oldWeapon.availableAmmo);
                }
                break;

            case WeaponType.Secondary:
                if(oldWeapon != null)
                {
            
                    weaponManager.secondaryWeaponAmmo += weapon.availableAmmo;
                    weapon.availableAmmo = 0;
                    oldWeapon.availableAmmo = weaponManager.primaryWeaponAmmo;
                    weaponManager.ammo.Refresh(oldWeapon.ammoCount, oldWeapon.magazineSize, oldWeapon.availableAmmo);
                }
                break;
        }
    }
    //this is the main function for picking up weapon using the Equip and Drop function in the AimingAndEquiping Script 
    private void PickWeapon(GameObject other)
    {
        Weapon newWeapon = other.GetComponent<Weapon>();
        if (newWeapon != null)
        {
            int newWeaponIndex = (int)newWeapon.weaponSlots;
            Weapon oldWeapon = weaponManager.GetWeapon(newWeaponIndex);
            if (oldWeapon == null)
            {
                newWeapon.GetComponent<Rigidbody>().isKinematic = true;
                newWeapon.GetComponent<BoxCollider>().enabled = false;
                weaponManager.Equip(newWeapon);
                //Debug.Log("1st pick");
            }
            else 
            {
               // Debug.Log("2nd pick");
                weaponManager.DropWeapon(oldWeapon);
                if (oldWeapon == null)
                {
                    weaponManager.Equip(newWeapon);
                }
                

            }
            CollidedWeaponDisabled(newWeapon);
        }
    }


    //the main function for dropping weapon this uses the DropWeapon function already defined in the AimingAndEquiping script
    private void DropEquippedWeapon()
    {
        
            Weapon weapon = weaponManager.GetActiveWeapon();
            if(weapon != null)
            {
                weaponManager.DropWeapon(weapon);
            //Debug.Log("dropped a" + weapon.weaponName);
            }
        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Key collidedKey = hit.gameObject.GetComponent<Key>();
        if(collidedKey != null)
        {
            collidedKey.gameObject.SetActive(false);
            collidedKey.transform.SetParent(this.transform);
            key = collidedKey;
        }

        if (hit.gameObject.GetComponent<Weapon>() != null)
        {
            collidedWeapon = hit.gameObject;
            CalculateAmmo(collidedWeapon);
            CollidedWeaponIconEnabled();
            StartCoroutine(DisableWeapon(collidedWeapon));
            //Debug.Log("collided with weapon");
        }

        if (hit.gameObject.CompareTag("Portal"))
        {
            hitPortal = true;
        }
    }

    void GetDoor()
    {
        RaycastHit hit;
        Physics.Raycast(doorChecker.transform.position, doorChecker.transform.forward, out hit, 10.0f);
        if(hit.collider != null && hit.collider.GetComponent<Door>() != null)
        {
            door = hit.collider.GetComponent<Door>();
            
        }
    }
   
    void OpenDoor()
    {
        GetDoor();
           
        if (Input.GetKeyDown(KeyCode.G))
        {
            if(door && key)
            {
                bool pos = Vector3.Dot(door.transform.forward , transform.forward) <= 0;
                //Debug.Log(pos);
                door.CheckKey(key, door, pos);
            }
        }
    }

    void SoundBasedOnMovement()
    {
        float intensity;
        Weapon weapon = weaponManager.GetActiveWeapon();
        if (walking && !crouch)
        {
            soundCategory = SoundManager.SoundsCategory.Walking;
            intensity = 0.5f;
            SoundManager.instance.SoundEmitted(this.transform.position, soundCategory, intensity);
        }
        if (running)
        {
            soundCategory = SoundManager.SoundsCategory.Running;
            intensity = 1.0f;
            SoundManager.instance.SoundEmitted(this.transform.position, soundCategory, intensity);
        }
        if (crouchWalking)
        {
            soundCategory = SoundManager.SoundsCategory.CrouchWalking;
            intensity = 0.2f;
            SoundManager.instance.SoundEmitted(this.transform.position, soundCategory, intensity);
        }
        if(weapon != null)
        {
            if (control.isShooting && weapon.weaponName == "Rifle")
            {
                soundCategory = SoundManager.SoundsCategory.Shooting;
                intensity = 3.0f;
                SoundManager.instance.SoundEmitted(this.transform.position, soundCategory, intensity);
            }

            if (control.isShooting && weapon.weaponName == "Pistol")
            {
                soundCategory = SoundManager.SoundsCategory.Shooting;
                intensity = 0.3f;
                SoundManager.instance.SoundEmitted(this.transform.position, soundCategory, intensity);
            }
        }
       
    }

    
}
 