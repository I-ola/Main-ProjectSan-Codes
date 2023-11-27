using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Suspect : States
{
    public Suspect(GameObject _npc, NavMeshAgent _agent, Player _player, Processor processor)
        : base(_npc, _agent, _player,   processor)
    {
        name = STATE.SUSPECT;
        priority = 4;
    }

    public override void Enter()
    {
        agent.speed = 2;
        agent.isStopped = false;
       
        
        base.Enter();
    }

    public override void Update()
    {
        if (processor.soundLocation != null)
        {
            agent.SetDestination(processor.soundLocation);
        }

    }

    public override void Exit() 
    { 
        base.Exit();
    }
}
