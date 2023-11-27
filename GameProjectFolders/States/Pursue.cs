using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Pursue :States
{
    
   public Pursue(GameObject _npc, NavMeshAgent _agent, Player _player, Processor processor)
        : base(_npc, _agent, _player, processor)
    {
        name = STATE.PURSUE;
        priority = 3;
        
    }

    public override void Enter()
    {
        crouching = false;
        agent.isStopped = false;
        agent.speed = 5.0f;
        target = null;
        weaponAim = null;
        StorePlayerLastPos();
        base.Enter();
       
    }

    public override void Update()
    {
        agent.SetDestination(player.transform.position);

        if (LostPlayer() && !agent.hasPath && agent.remainingDistance < agent.stoppingDistance)
        {
            //Debug.Log("I'm from Puruse");
            nextState = new Patrol(npc, agent, player, processor);
            stage = EVENT.EXIT;
        }
    }
    public override void Exit()
    {
        base.Exit();
    }
}
