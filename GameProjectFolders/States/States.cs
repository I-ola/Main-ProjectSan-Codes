using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class States
{
    public enum STATE
    {
        IDLE,
        PATROL,
        SUSPECT,
        PURSUE,
        ATTACK,
        DEATH,
        COVER,
        COVERATTACK
    };

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public STATE name;
    public EVENT stage;
    protected GameObject npc;
    protected Player player; //Transform player;
    public States nextState;
    protected NavMeshAgent agent;
    protected Animator animator;
    protected Transform rayEyeUpper;
    protected Transform rayEyeLower;
    protected Weapon weapon;
    protected AIHealth health;
    protected AIWeaponIK weaponIK;
    protected SphereCollider coverCollider;
    protected LayerMask hidableLayer;
    protected Processor processor;
    protected AISoundManager sound;

    private LayerMask aimMask = LayerMask.NameToLayer("Hitbox");
    float visDis = 10.0f;
    float visAngle = 70.0f;
    float caughtPlayerDistance = 4f;

    public bool inCover = false;
    public bool isDead = false;
    public bool aiming;
    public bool crouching;
    public bool canMove = false;
    public bool attackingPlayer = false;
    public bool reset;
    public int index;
    public float crouchShoot;
    private Vector3 playerLastKnownPosition;
    public int priority { get; set; }

    public Transform target = null;
    public Transform weaponAim = null;
    public States(GameObject _npc, NavMeshAgent _agent, Player _player, Processor processor)
    {
        npc = _npc;
        agent = _agent;
        stage = EVENT.ENTER;
        player = _player;
        rayEyeUpper = processor.rayEyeUpper;
        rayEyeLower = processor.rayEyeLower;
        weapon = processor.weapon;
        this.health = processor.health;
        this.weaponIK = processor.activateWeapon;
        this.coverCollider = processor.coverCollider;
        this.hidableLayer = processor.hidableLayer;
        this.processor = processor;
        sound = processor.sound;
    }

    public virtual void Enter() { stage = EVENT.UPDATE; }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }

    public States Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState;
        }
        return this;
    }
    public bool Dead()
    {
        if (health.isDead)
        {
            return true;
        }
        return false;
    }
    public bool canSeePlayer()
    {
        if (!player.isDead && !health.isDead)
        {
            Vector3 direction = player.transform.position - npc.transform.position;
            float angle = Vector3.Angle(direction, npc.transform.forward);
            if ((direction.magnitude < visDis && angle < visAngle) && (ObastacleCheckLower() || ObastacleCheckUpper()))
            {
                //Debug.Log($"Can See Player");
                return true;
            }
            return false;
        }

        return false;
    }

    public bool canAttackPlayer()
    {
        if (!player.isDead && !health.isDead)
        {
            Vector3 direction = player.transform.position - npc.transform.position;
            if ((direction.magnitude <= caughtPlayerDistance) && (ObastacleCheckLower() || ObastacleCheckUpper()))
            {
                // Debug.Log($"Can attack Player");
                return true;
            }
            return false;
        }

        return false;
    }

    public bool ObastacleCheckUpper()
    {
        Vector3 direction = player.transform.position - npc.transform.position;
        RaycastHit hitUpper;
        if (Physics.Raycast(rayEyeUpper.transform.position, direction, out hitUpper, visDis))
        {
            if (hitUpper.collider.CompareTag("Player"))
            {
                //Debug.Log("from Upper ray");
                return true;
            }
        }
        return false;
    }
    public bool ObastacleCheckLower()
    {
        Vector3 direction = player.transform.position - npc.transform.position;
        RaycastHit hitLower;
        if (Physics.Raycast(rayEyeLower.transform.position, direction, out hitLower, visDis))
        {
            if (hitLower.collider.CompareTag("Player"))
            {
                // Debug.Log("from lower ray" );
                return true;
            }
        }
        return false;
    }

    public bool crouchAttack()
    {
        if (!player.isDead && !health.isDead)
        {
            Vector3 direction = player.transform.position - npc.transform.position;
            if ((direction.magnitude <= 15f) && (ObastacleCheckLower() || ObastacleCheckUpper()))
            {
                //Debug.Log($"Crouch ");
                return true;
            }
            return false;
        }

        return false;
    }
    public bool LostPlayer()
    {
        if (name == STATE.PURSUE || name == STATE.ATTACK || name == STATE.COVERATTACK)
        {
            float dist = Vector3.Distance(npc.transform.position, playerLastKnownPosition);
            if(dist > 15f)
            {
                //Debug.Log(npc.gameObject.name + "has lost player");
                return true;
            }
            return false;
        }
        return false;
    }

    public void StorePlayerLastPos()
    {
        if(!player.isDead || !Dead())
        {

            playerLastKnownPosition = player.transform.position;

        }

    }
    public bool RunToCover()
    {
        if (!player.isDead && !health.isDead)
        {
            if ((health.takingDamage) && (!canSeePlayer() || !canAttackPlayer()))
            {
               // Debug.Log($"Need to go to cover");
                return true;
            }
            return false;
        }
       
        return false;
    }

}
