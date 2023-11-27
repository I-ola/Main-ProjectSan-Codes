using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Dead : States
{
    public Dead(GameObject _npc, NavMeshAgent _agent, Player _player, Processor processor)
        : base(_npc, _agent, _player, processor)
    {
        name = STATE.DEATH;
        agent.isStopped = true;

        priority = 0;
        
        
    }

    public override void Enter()
    {
        target = null;
        weaponAim = null;
        DropWeapon();
        agent.isStopped = true;
        if(processor.key != null)
        {
            processor.key.gameObject.transform.SetParent(null);
            processor.key.gameObject.GetComponent<BoxCollider>().enabled = true;
            if (processor.key.gameObject.GetComponent<Rigidbody>() == null)
            {
                processor.key.gameObject.AddComponent<Rigidbody>();
            }

           
        }    

     
        base.Enter();
    }

    public override void Update()
    {
        nextState = null;
        stage = EVENT.EXIT;
    }
    public override void Exit()
    {
        processor.DestroyAgent();
        
    }

    public void DropWeapon()
    {
        var currentWeapon = weapon;
        if (currentWeapon)
        {
            currentWeapon.availableAmmo = 0;
            currentWeapon.isDropped = true;
            currentWeapon.transform.SetParent(null);
            currentWeapon.gameObject.SetActive(true);
            currentWeapon.gameObject.GetComponent<BoxCollider>().enabled = true;
            if (currentWeapon.gameObject.GetComponent<Rigidbody>() == false)
            {
                currentWeapon.gameObject.AddComponent<Rigidbody>();
            }
            currentWeapon.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}
