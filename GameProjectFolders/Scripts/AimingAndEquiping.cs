using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;



public class AimingAndEquiping : MonoBehaviour
{
    //[SerializeField] private float aimDuration = 0.3f;
    private PlayerController controller;
    [SerializeField] private Transform crossHairTarget;
    [SerializeField] private Transform[] weaponSlots;
    [SerializeField] private Transform leftGrip;
    [SerializeField] private Transform rightGrip;
    [SerializeField] public AmmoWidget ammo;
    
    private Player player;
    // private AnimatorOverrideController overrides;
    [SerializeField] private Rig aimLayer;

    [SerializeField] private Animator rigAnimator;
    private PlayerInput playerInput;
    private ReloadWeapon reload;

    public int primaryWeaponAmmo = 300;
    public int secondaryWeaponAmmo = 200;
    public int storedPrimaryAmmo;
    public int storedSecondaryAmmo;
    public int saveExtraPrimaryBullet;
    public int saveExtraSecondaryBullet;

    Weapon[] equippedWeapons = new Weapon[2];
    int activeWeaponIndex; 

    public bool switched;
    //bool test = true;



    // this is passed to the weapon script and assigned in the weapon prefab 
    //useful when we are trying to get weapon as we have already assigned each weapon an int based on if they are primary or secondary so all primary weapon are in slot 0 and secondary in slot 1
    public enum WeaponSlots
    {
        Primary = 0,
        Secondary = 1
    }

    public enum WeaponType
    {
        Primary,
        Secondary
    }
    // Start is called before the first frame update
    void Start()
    {
        //rigAnimator = GetComponent<Animator>();
        //overrides = animator.runtimeAnimatorController as AnimatorOverrideController;
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        reload = GetComponent<ReloadWeapon>();
        controller = GetComponent<PlayerController>();
        primaryWeaponAmmo = 300;
         Weapon existingWeapon = GetComponentInChildren<Weapon>();
        if (existingWeapon)
        {
            Equip(existingWeapon);
        }

     


    }

    //this is used to get the weapons equipped at the different index of the array of equipped wapons which has been made 2 and in the equip function we store the equip weapon in either 0 or 1 based on their assigned values in the weaponslot enum
    //so this returns if there is a weapon stored at the provided index
    public Weapon GetWeapon(int index)
    {
        if(index < 0 || index >= equippedWeapons.Length) return null;
        return equippedWeapons[index];
    }

    //activeWeaponIndex is the index of the present weapon in use 
    public Weapon GetActiveWeapon()
    {
        return GetWeapon(activeWeaponIndex);
    }

    // Update is called once per frame
    void Update()
    {

        /*if(Input.GetKeyDown(KeyCode.P))
        {
            test = true;
        }
        if(Input.GetKeyDown(KeyCode.O))
        {
            test = false;
        }*/

        //this control the aiming and hiding animations based on the right mouse button click which serves as the aiming 
        rigAnimator.SetBool("isAiming", (controller.isAiming() && !player.Crouch()));
        rigAnimator.SetBool("crouchAim", (player.Crouch() && controller.isAiming()));

        rigAnimator.SetBool("hideWeapon",!controller.isAiming());

        //all this functions are performed as long as the player is not dead
        if (!player.isDead)
        {
            SwitchWeapon();
            FireWeapon();
            hideWeapon();
        }

        //this sets the invisible weapon to visible when the aim action "right mouse button" is performed
        if (controller.isAiming())
        {
            Weapon weapon = GetActiveWeapon();
            if(weapon != null)
            {
                weapon.gameObject.SetActive(true);
            }
            
        }

        //Debug.Log(primaryWeaponAmmo + "primaryAmmo");
        //Debug.Log(secondaryWeaponAmmo + "Secondaryammo");
      //  Debug.Log(storedPrimaryAmmo + "storedPrimary");
       // Debug.Log(storedSecondaryAmmo + "StoredSecondary");
        
    }


    //this is called when the shoot action is performed
    private void FireWeapon()
    {
        var weapon = GetWeapon(activeWeaponIndex);
        if ((weapon != null) && controller.isAiming() && Shooting() && !reload.isReloading)
        {
          
            weapon.StartFiring();
           
        }
        else if (weapon && !Shooting())
        {
            weapon.StopFiring();
        }

        if (weapon != null)
        {
            weapon.UpdateWeapon(Time.deltaTime, crossHairTarget.position);
            
        }
       
    }

   
    private void SwitchWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            SelectWeapon();
            switched = true;
            
            Invoke("MakeSwitchFalse", 5f);

        }
    }
     
    void MakeSwitchFalse()
    {
        switched = false; 
    }
    

    public bool Shooting()
    {
        return controller.isShooting;
    }

    // this function is used for equipping wepaon  it attaches the weapon based on the slot index this is already assigned on the weapon prefab and also sets the weapon parent to the player
    public void Equip(Weapon newWeapon)
    {
        StopCoroutine(DestroyDropWeapon(newWeapon));
        int weaponSlotIndex = (int)newWeapon.weaponSlots;
        var weapon = GetWeapon(weaponSlotIndex);
        if (weapon)
        {
            CheckAmmo(weapon);
            //DropWeapon(weapon);
        }
        weapon = newWeapon;
        weapon.transform.parent = weaponSlots[weaponSlotIndex];
        weapon.isDropped = false;
        weapon.GetComponent<WeaponRecoil>().enabled = true;
        weapon.recoil.playerAimCamera = player;
        weapon.recoil.rigController = rigAnimator;
        weapon.gameObject.transform.localPosition = Vector3.zero;
        weapon.gameObject.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
        CheckAmmo(weapon);

        rigAnimator.Play("Equip" + weapon.weaponName);
        
        equippedWeapons[weaponSlotIndex] = weapon;
        activeWeaponIndex = (int)weapon.weaponSlots;
        weapon.gameObject.SetActive(true);
        

        ammo.Refresh(weapon.ammoCount, weapon.magazineSize, weapon.availableAmmo);
       // overrides["EmptyAnimation"] = weapon.weaponAnimation;
    }

    // this checks the weapon type and adds available ammo from the weapon to the players own personal ammo
    public void CheckAmmo(Weapon weapon)
    {
        switch (weapon.weaponType)
        {
            case WeaponType.Primary:
                if(weapon.availableAmmo >= 0)
                {
                    storedPrimaryAmmo = weapon.availableAmmo;
                    primaryWeaponAmmo += storedPrimaryAmmo;
                    weapon.availableAmmo = primaryWeaponAmmo;
                    
                }           
                break;

            case WeaponType.Secondary:
                if(weapon.availableAmmo >= 0)
                {
                    storedSecondaryAmmo = weapon.availableAmmo;
                    secondaryWeaponAmmo += storedSecondaryAmmo;
                    weapon.availableAmmo = secondaryWeaponAmmo;
                   
                }                  
                break;
        }

    }
    // this deals in switching weapon based on pressing either the 1 or 2 key and also deals with the animation for both weapons
    public void SelectWeapon()
    {
        int selectedWeaponIndex = 0;
    
    
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeaponIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeaponIndex = 1;
        }
        var weapon = GetWeapon(selectedWeaponIndex);
        if (weapon != null)
        {
            rigAnimator.SetBool("hideWeapon", !controller.isAiming());
            rigAnimator.Play("Equip" + weapon.weaponName);
        }

        if(weapon == null)
        {
            return;
        }
        activeWeaponIndex =  selectedWeaponIndex;
        weapon.gameObject.SetActive(true);

    }

    // to automatically hide the players weapon after seconds of it not being in use
    void hideWeapon()
    {
        
        Weapon weapon = GetActiveWeapon();

        if(!controller.isAiming() && !switched && !reload.isReloading)
        {
            foreach (var equipped in equippedWeapons)
            {
                if (equipped != null)
                {
                    equipped.gameObject.SetActive(false);
                }
            }
        }
       
        
    }

    //a function that makes player drop the current active weapon
    public void DropWeapon(Weapon currentWeapon)
    {
       // var currentWeapon = GetActiveWeapon();
        if (currentWeapon)
        {
            ammo.DroppedWeapon(currentWeapon);
            currentWeapon.availableAmmo = 0;
            currentWeapon.isDropped = true;
            currentWeapon.transform.SetParent(null);
            currentWeapon.gameObject.SetActive(true);
            currentWeapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            if(currentWeapon.gameObject.GetComponent<Rigidbody>() == false)
            {
                currentWeapon.gameObject.AddComponent<Rigidbody>();
            }
            currentWeapon.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            equippedWeapons[activeWeaponIndex] = null;
            StartCoroutine(DestroyDropWeapon(currentWeapon));
        }
    }

    IEnumerator DestroyDropWeapon(Weapon weapon)
    {
        yield return new WaitForSeconds(4f);
        //Debug.Log("called on destroy");
        if (weapon.isDropped)
        {
            Destroy(weapon.gameObject);
        }
        
    }

    //The Below records the position of the below gameobjects and stores them in an animation clip
    //[ContextMenu("Save Weapon Pose")]
    /*void SaveWeaponPose()
    {
        GameObjectRecorder recorder = new GameObjectRecorder(gameObject);
        recorder.BindComponentsOfType<Transform>(weaponParent.gameObject, false);
        recorder.BindComponentsOfType<Transform>(leftGrip.gameObject, false);
        recorder.BindComponentsOfType<Transform>(rightGrip.gameObject, false);
        recorder.TakeSnapshot(0.0f);
        recorder.SaveToClip(weapon.weaponAnimation);
        UnityEditor.AssetDatabase.SaveAssets();
    }*/
}
