using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TestTools;

public class GoToCover : States
{
   public GoToCover(GameObject _npc, NavMeshAgent _agent, Player _player, Processor processor)
        : base(_npc, _agent, _player, processor)
    {
        name = STATE.COVER;
        priority = 4;

        // covers = GameObject.FindGameObjectsWithTag("Cover");
    }

    public override void Enter()
    {
        
        agent.isStopped = false;
        agent.speed = 5;
        agent.stoppingDistance = 1f;
        
        base.Enter();
    }

    public override void Update()
    {

        TakeCover();
        if(!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.speed = 0;
            agent.isStopped = true;
            crouching = true;
            inCover = true;
            nextState = new CoverAttack(npc, agent, player, processor);
            //processor.EnqueueState(nextState);
            stage = EVENT.EXIT;
        }
         
    }

    public override void Exit()
    {
        //Debug.Log($"Changing from GoToCover");
        inCover = false;
        base.Exit(); 
    }

    void TakeCover()
    {
        Collider[] colliders = Physics.OverlapSphere(npc.transform.position, coverCollider.radius, hidableLayer);
        System.Array.Sort(colliders, ColliderArraySortCompare);

         Vector3 coverSpot = Vector3.zero;
         Vector3 coverDir = Vector3.zero;

        var nearestCover = colliders
            .Where(collider => collider.CompareTag("Cover"))
            .Where(collider => Vector3.Distance(npc.transform.position, collider.transform.position) <= 10.0f)
            .Where(collider => Vector3.Dot((npc.transform.position- collider.transform.position).normalized, player.transform.forward) <= -0.1f)
            .OrderBy(collider => Vector3.Distance(npc.transform.position, collider.transform.position))
            .FirstOrDefault();

        if(nearestCover != null && !agent.hasPath)
        {
            Vector3 hideDir = nearestCover.transform.position - npc.transform.position;
            Vector3 hidePosition = nearestCover.transform.position + hideDir.normalized * 10;

       
            Ray backray = new Ray(hidePosition, -hideDir.normalized);
            RaycastHit info;
            if (nearestCover.Raycast(backray, out info, coverCollider.radius))
            {
                agent.SetDestination(info.point + coverDir.normalized);
            }
  
        }
    }

    private int ColliderArraySortCompare(Collider A, Collider B)
    {
        if (A == null || B != null)
        {
            return 1;
        }else if (A != null || B == null)
        {
            return -1;
        }else if(A == null || B == null)
        {
            return 0;
        }
        else
        {
            return Vector3.Distance(agent.transform.position, A.transform.position).CompareTo(Vector3.Distance(agent.transform.position, B.transform.position));
        }   
        
    }

}
