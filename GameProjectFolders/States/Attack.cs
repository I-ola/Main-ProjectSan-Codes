using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Attack : States
{
    float rotationSpeed = 10.0f;
    float inaccuracy = 0.4f;

    public Attack(GameObject _npc, NavMeshAgent _agent, Player _player, Processor processor)
        : base(_npc, _agent, _player , processor)
    {
        name = STATE.ATTACK;
        priority = 2;
        
    }

    public override void Enter()
    {
        agent.isStopped = false;
        agent.speed = 6.0f;
        agent.stoppingDistance = 6.0f;
        target = player.transform;
        weaponAim = weapon.bulletPathRaycast;
        StorePlayerLastPos();
        processor.SetAi();

        base.Enter();
          
    }

    public override void Update()
    {
        StorePlayerLastPos();
        agent.SetDestination(player.transform.position);
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.speed = 0.0f;

        }
        else
        {
            agent.speed = 6.0f;
        }

        if (weapon.ammoCount >= 1)
        {
            AttackPlayer();
        }


        if (LostPlayer() && attackingPlayer)
        {
            //Debug.Log(npc.gameObject.name + " is leaving attack to patrol");
            agent.speed = 2f;
            nextState = new Patrol(npc, agent, player, processor);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        attackingPlayer = false;
        agent.stoppingDistance = 2f;
        target = null;
        weaponAim = null;
        weaponIK.SetTargetTransform(target);
        weaponIK.SetAimTransform(weaponAim);
        base.Exit();
    }

    private void AttackPlayer()
    {
        Vector3 dir = player.transform.position - npc.transform.position;
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation, Quaternion.LookRotation(dir), rotationSpeed * Time.deltaTime);
        Vector3 position;
        weaponIK.SetTargetTransform(target);
        weaponIK.SetAimTransform(weaponAim);
        if (!player.isDead && weapon.ammoCount > 0)
        {
            attackingPlayer = true;
            SetFiring(true);

            position = target.position + weaponIK.aimOffset;

            position += Random.insideUnitSphere * inaccuracy;

            weapon.UpdateWeapon(Time.deltaTime, position);

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
}

