using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHealth : Health
{

    public  bool isDead = false;
    


    // Ragdoll ragdoll;

    protected override void OnStart()
    {
        //ragdoll = GetComponent<Ragdoll>();
       
    }

    protected override void OnDamage(Vector3 direction)
    {
       
    }

    protected override void OnDeath(Vector3 direction)
    {
        isDead = true;
       
        
        // ragdoll.ActivateRagdoll();
        //direction.y = 0;
        //ragdoll.ApplyForce(direction * force);

    }

    protected override void OnHeal(float amount)
    {
       // takingDamage = false;
    }

    private void Dead(string name)
    {
        if(name == "death")
        {
            //ragdoll.OnDeath();
        }
    }

 
}
