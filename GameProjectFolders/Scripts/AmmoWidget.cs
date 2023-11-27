using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AmmoWidget : MonoBehaviour
{
    public TMP_Text ammoText;
    public TMP_Text totalAmmo;
    public TMP_Text availableAmmo;
    public GameObject rifleIcon;
    public GameObject pistolIcon;
    public GameObject text1;
    public GameObject text2;
    public GameObject text3;

    public AimingAndEquiping activeWeapon;
    //this is function is used to show the weapon and ammo Ui
    public void Refresh(int ammoCount, int magazineSize, int ammoLeft)
    {
        ammoText.text = ammoCount.ToString();
        totalAmmo.text = magazineSize.ToString();
        availableAmmo.text = ammoLeft.ToString();

    }

    private void Update()
    {


        SwitchWeaponUI();

       
    }

    //this is used to set the different UI active and inactive based on the weapon in use
    void SwitchWeaponUI()
    {
        Weapon weapon = activeWeapon.GetActiveWeapon();
        if (weapon!= null)
        {
            text1.SetActive(true);
            text2.SetActive(true);
            text3.SetActive(true);

            if (weapon.weaponName == "Rifle")
            {
                rifleIcon.SetActive(true);
                pistolIcon.SetActive(false);
                Refresh(weapon.ammoCount, weapon.magazineSize, weapon.availableAmmo);
            }

            if (weapon.weaponName == "Pistol")
            {
                pistolIcon.SetActive(true);
                rifleIcon.SetActive(false);
                Refresh(weapon.ammoCount, weapon.magazineSize, weapon.availableAmmo);
            }
        }
    }

    public void DroppedWeapon(Weapon droppedWeapon)
    {
        if (droppedWeapon)
        {
            if (droppedWeapon.weaponName == "Rifle")
            {
                rifleIcon.SetActive(false);
                text1.SetActive(false);
                text2.SetActive(false);
                text3.SetActive(false);
            }

            if (droppedWeapon.weaponName == "Pistol")
            {
                pistolIcon.SetActive(false);
                text1.SetActive(false);
                text2.SetActive(false);
                text3.SetActive(false);
            }
        }
    }
}
