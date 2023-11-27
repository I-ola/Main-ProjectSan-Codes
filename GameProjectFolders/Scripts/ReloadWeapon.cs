using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ReloadWeapon : MonoBehaviour
{
    [SerializeField] private Animator rigAnimator;
    [SerializeField] private WeaponAnimationEvents AnimationEvents;
    private AimingAndEquiping playerWeapon;
    private Processor ai;
    [SerializeField] private Transform leftHand;
    [SerializeField] private AmmoWidget playerAmmo;
    private PlayerController controller;

    GameObject magazineOnHand;
    GameObject droppedMagazine;
    private Weapon weapon;

    public bool isReloading = false;
    // Start is called before the first frame update
    void Start()
    {
        AnimationEvents.weaponAnimationEvent.AddListener(OnAnimationEvent);
        playerWeapon = GetComponent<AimingAndEquiping>();
        controller = GetComponent<PlayerController>();
        ai = GetComponent<Processor>();
    }

    // Update is called once per frame
    void Update()
    {
        
        
        if(playerWeapon!= null)
        {
            weapon = playerWeapon.GetActiveWeapon();
            if (weapon != null)
            {
                
              
                 //Check();   
                if (((Input.GetKeyDown(KeyCode.R) && weapon.ammoCount < weapon.magazineSize) || (weapon.ammoCount == 0 && !isReloading)) && controller.isAiming())
                {
                    Reload(weapon);
                       
                }
                else if (isReloading)
                {
                    Invoke("ResetReloading", 5);
                }

                if (weapon.isFiring)
                {
                        playerAmmo.Refresh(weapon.ammoCount, weapon.magazineSize, weapon.availableAmmo);

                }

            }

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                Invoke("ResetReloading", 1.0f);

            }
        }
        
        if( ai != null)
        {
            //Debug.Log("in AI");
            weapon = ai.weapon;
            if (weapon)
            {
                if(weapon.ammoCount <= 0)
                {
                   
                    Reload(weapon);
                }
            }
        }
      
       

    }

    void OnAnimationEvent(string eventName)
    {
       // Debug.Log(eventName);
       switch(eventName)
        {
          
            case "detach_magazine":
                DetachMagazine();
                break;
            case "drop_magazine":
                DropMagazine();
                break;
            case "refill_magazine":
                RefillMagazine();
                break;
            case "attach_magazine":
                AttachMagazine();
                break;
        }
    }
    
    void DetachMagazine()
    {
        if (playerWeapon != null)
        {
            weapon = playerWeapon.GetActiveWeapon();
        } else if (ai != null)
        {
            weapon = ai.weapon;
        }
        
        magazineOnHand = Instantiate(weapon.magazine, leftHand, true);
        weapon.magazine.SetActive(false);
    }
    
    void DropMagazine()
    {
        droppedMagazine = Instantiate(magazineOnHand, magazineOnHand.transform.position, magazineOnHand.transform.rotation);
        droppedMagazine.AddComponent<Rigidbody>();
        droppedMagazine.AddComponent<BoxCollider>();
        StartCoroutine(DestroyDroppedMagazine(droppedMagazine));
        magazineOnHand.SetActive(false);
        
    }

    void RefillMagazine()
    {
        magazineOnHand.SetActive(true);
    }

    void AttachMagazine()
    {
        if(playerWeapon != null)
        {
            weapon = playerWeapon.GetActiveWeapon();
            weapon.magazine.SetActive(true);
            Destroy(magazineOnHand);
            weapon.ammoCount += weapon.reloadAmount;
            playerAmmo.Refresh(weapon.ammoCount, weapon.magazineSize, weapon.availableAmmo);
            isReloading = false;
            playerWeapon.switched = false;
            rigAnimator.SetBool("reload", false);
        }
        
        else if(ai != null)
        {
            weapon = ai.weapon;
            weapon.magazine.SetActive(true);
            Destroy(magazineOnHand);
            weapon.ammoCount += weapon.reloadAmount;
            isReloading = false;
            rigAnimator.SetBool("reload", false);
        }
    }

    // when called checks for the different parameters  to know the reload amount such as if ammoAvailable is less than the required ammo needed for reload or if there is no longer any availableAmmo to use in reloading the weapon
    //the isReloading bool helps in different ways such as if the player is in the middle of reloading and stops aiming or switches the weapon on aiming or switching back to the weapon the reload starts
    public void Reload(Weapon weapon)
    {
      
        if (weapon)
        {
            if(weapon.availableAmmo > 0 && !isReloading)
            {        
                    isReloading = true;
 
                if (weapon.bulletUsed <= weapon.availableAmmo)
                    {
                        weapon.reloadAmount = weapon.bulletUsed;
                    }
                    else if(weapon.bulletUsed >= weapon.availableAmmo)
                    {
                        weapon.reloadAmount = weapon.availableAmmo;
                    }

                weapon.availableAmmo -= weapon.reloadAmount;
                rigAnimator.SetBool("reload", true);
            }
            
        }
    }

    IEnumerator DestroyDroppedMagazine(GameObject droppedMagazine)
    {

        yield return new WaitForSeconds(5.0f);
       
        Destroy(droppedMagazine);
    }

   
    void ResetReloading()
    {
        isReloading = false;

    }
}
