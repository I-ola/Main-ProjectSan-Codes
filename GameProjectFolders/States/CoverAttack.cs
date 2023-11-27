using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class CoverAttack : States
{
    float inaccuracy = 0.4f;
    float time = 0.0f;
    float maxTime = 10.0f;
    bool hasAttacked = false;
    public CoverAttack(GameObject _npc, NavMeshAgent _agent, Player _player, Processor processor)
       : base(_npc, _agent, _player, processor)
    {
        name = STATE.COVERATTACK;
        priority = 3;
    }

    public override void Enter()
    {
        agent.speed = 0f;
        agent.isStopped = true;
        crouching = true;
        target = player.transform;
        weaponAim = weapon.bulletPathRaycast;
        StorePlayerLastPos();
        processor.SetAi();

        base.Enter();
    }

    public override void Update()
    {

        //Debug.Log($"the time is already greater than max time:{time > maxTime}");
        // Debug.Log($"can crouch attack{crouchAttack()}");
        if (!crouchAttack())
        {
            time += Time.deltaTime;
            if(time > maxTime)
            {
       
                nextState = new Patrol(npc, agent, player, processor);
             
                stage = EVENT.EXIT;
            }else if (!LostPlayer() && hasAttacked)
            {
                nextState = new Attack(npc, agent, player, processor);

                stage = EVENT.EXIT;
            }
           
        }
         if (crouchAttack())
         {
            Vector3 dir = player.transform.position - npc.transform.position;
            npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, Quaternion.LookRotation(dir), 10f);
            hasAttacked = true;
            CrouchAndShootingSwitch();
            if (canAttackPlayer())
            {
            
                nextState = new Attack(npc, agent, player, processor);
              
                stage = EVENT.EXIT;
            }
         }
    }

    public override void Exit()
    {
        //Debug.Log($"Changing state from CoverAttack");
        crouching = false;
        crouchShoot = 0.0f;
        target = null;
        weaponAim = null;
        weaponIK.SetTargetTransform(target);
        weaponIK.SetAimTransform(weaponAim);

        base.Exit(); 
    }

    public void CrouchAndShootingSwitch()
    {
        if (Time.time % 10 < 1)
        {
            crouchShoot = 0.0f;

        }
        else
        {
            crouchShoot += 1.0f;
            AttackPlayer();
        }

        if (weapon.ammoCount == 0)
        {
            crouchShoot = 0.0f;
        }
    }
    public void SetFiring(bool enabled)
    {
        if (enabled)
        {
            weapon.StartFiring();
        }
        else
        {
            weapon.StopFiring();
        }
    }

    private void AttackPlayer()
    {

        Vector3 position;
        weaponIK.SetTargetTransform(target);
        weaponIK.SetAimTransform(weaponAim);
        if (!player.isDead && weapon.ammoCount > 0)
        {
            SetFiring(true);

            position = target.position + weaponIK.aimOffset;

            position += Random.insideUnitSphere * inaccuracy;

            weapon.UpdateWeapon(Time.deltaTime, position);

        }
    }
}
