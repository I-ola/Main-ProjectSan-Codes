using UnityEngine;
using UnityEngine.AI;

public class Patrol : States
{
    float time = 0.0f;
    public Patrol(GameObject _npc, NavMeshAgent _agent, Player _player, Processor processor)
        : base(_npc, _agent, _player, processor)
    {
        name = STATE.PATROL;
       
        index = -1;
        priority = 5;

    }

    public override void Enter()
    {
        target = null;
        weaponAim = null;
       
        agent.isStopped = false;

        CalculatePath();
        base.Enter(); 
    }

    public override void Update()
    {
        if(agent.remainingDistance < agent.stoppingDistance)
        {
            agent.speed = 1.0f;
            time += Time.deltaTime;
            if (time > 5.0f)
            {

                SetWay();
                time = 0.0f;

            }
        }

    }

    public override void Exit()
    {
        //Debug.Log($"Changing state to : {nextState} , from Patrol");
        base.Exit();
    }

    public void SetWay()
    {
        
        if (!agent.pathPending && agent.remainingDistance < agent.stoppingDistance)
        {
           

            if (index >= processor.pathObjects.Length - 1)
            {
                index = 0;
   
            }
            else
            {
                index++;
                
            }
           
            agent.SetDestination(processor.pathObjects[index].transform.position);
            agent.speed = 2.0f;
   
        }







    }

    public void CalculatePath()
    {
        float otherDist = Mathf.Infinity;
        for (int i = 0; i < processor.pathObjects.Length; i++)
        {
            GameObject wp = processor.pathObjects[i];
            float distance = Vector3.Distance(npc.transform.position, wp.transform.position);
            if (distance < otherDist)
            {
                index = i -1;
                otherDist = distance;
            }  
        }
       // int number = index + 1;
        //agent.SetDestination(processor.pathObjects[number].transform.position);
       
       
    }
}
