using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerHealth : Health
{
  
    Player player;
    AimingAndEquiping activeWeapon;
    public VolumeProfile volumeProfile;
    private Vignette vignette;
    protected override void OnStart()
    {
        activeWeapon = GetComponent<AimingAndEquiping>();
        player = GetComponent<Player>();

        

        
    }

    protected override void OnDamage(Vector3 direction)
    {
        UpdateVignette();
    }

    protected override void OnHeal(float amount)
    {
        UpdateVignette();
    }

    private void UpdateVignette()
    {
        if (volumeProfile.TryGet(out vignette))
        {
            if (vignette != null)
            {
                vignette.active = true;
                float percent = 1.0f - (currentHealth / maxHealth);
                vignette.intensity.value = percent * 0.5f;
            }

        }
    }
    protected override void OnDeath(Vector3 direction)
    {
        
        player.isDead = true;
        var currentWeapon = activeWeapon.GetActiveWeapon();
        activeWeapon.DropWeapon(currentWeapon);
        if (volumeProfile.TryGet(out vignette))
        {
            if (vignette != null)
            {
                vignette.active = true;
                vignette.intensity.value = 0.0f;
            }

        }

    }

}


