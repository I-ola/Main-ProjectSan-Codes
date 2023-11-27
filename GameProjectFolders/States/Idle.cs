using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Idle :States
{
    public Idle(GameObject _npc, NavMeshAgent _agent, Player _player,  Processor processor)
        : base(_npc, _agent, _player, processor)
    {
        name = STATE.IDLE;
        agent.isStopped = false;
        agent.speed = 1f;
        priority = 6;
    }

    public override void Enter()
    {
        target = null;
        weaponAim = null;
        //Debug.Log("in idle state");
        base.Enter(); 
    }

    public override void Update()
    {
       /* if (Random.Range(0,100) < 10)
        {
           
            nextState = new Patrol(npc, agent, player, processor);
            stage = EVENT.EXIT;

           // processor.EnqueueState(nextState);
        } */
    }

    public override void Exit()
    {
        //Debug.Log($"Changing state to : {nextState} , from Idle");
        base.Exit();
    }
}
