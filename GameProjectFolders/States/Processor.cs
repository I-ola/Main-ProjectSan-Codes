using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class Processor : MonoBehaviour
{
    private NavMeshAgent agent;
    private Player player;
    private States currentState;
    private Animator movementAnimator;
    public AIWeaponIK activateWeapon;
    public AISoundManager sound;
    public Weapon weapon;
    public AIHealth health;
    public Rig handLayer;
    public SphereCollider coverCollider;
    public LayerMask hidableLayer;
    public LayerMask aiLayer;
    public Key key;

  
    Collider[] aiColliders = new Collider[6];

    private int aiAmmo = 10000;
    float hearingRange = 10f;
    float maxIntensity = 0.45f;

    [SerializeField] private Animator weaponAnimator;
    [SerializeField] public Transform rayEyeUpper;
    [SerializeField] public Transform rayEyeLower;
    [SerializeField] public GameObject[] pathObjects;

    Processor[] otherAI = new Processor[6];
    Processor ai;
    Processor myAi;

    public Vector3 soundLocation;
    public float soundIntensity;
    public bool canMove = false;

    private Dictionary<Func<bool>, States> StateTransitions = new Dictionary<Func<bool>, States>();
      
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<Player>();
        movementAnimator = GetComponent<Animator>();
        activateWeapon = GetComponent<AIWeaponIK>();
        weapon = GetComponentInChildren<Weapon>();
        health = GetComponent<AIHealth>();
        myAi = this.GetComponent<Processor>();
        sound = GetComponent<AISoundManager>();
        key = GetComponentInChildren<Key>();

        weapon.availableAmmo = aiAmmo;

        currentState = new Idle(this.gameObject, agent, player, myAi);
        // the lower the priority value the higher its priority
        StateTransitions.Add(() => currentState.canSeePlayer(), new Pursue(this.gameObject, agent, player, myAi) {priority = 3});
        StateTransitions.Add(() => currentState.canAttackPlayer(), new Attack(this.gameObject, agent, player, myAi) { priority = 2});
        StateTransitions.Add(() => currentState.RunToCover(), new GoToCover(this.gameObject, agent, player, myAi) { priority = 4});
        StateTransitions.Add(() => currentState.inCover, new CoverAttack(this.gameObject, agent, player, myAi) { priority = 3 });
        StateTransitions.Add(() => SuspectLocation(), new Suspect(this.gameObject, agent, player, myAi) { priority = 4 });
        StateTransitions.Add(() => currentState.Dead(), new Dead(this.gameObject, agent, player, myAi) { priority = 0 });
        StateTransitions.Add(() => true, new Patrol(this.gameObject, agent, player, myAi) { priority = 5 });
        StateTransitions.Add(() => player.isDead, new Patrol(this.gameObject, agent, player, myAi) { priority = 1 });

    }
    

    private void Update()
    {
    
        SetAnimations();

            foreach (var state in StateTransitions)
            {
                if (state.Key())
                {
                        if (state.Value.priority < currentState.priority)
                        {
                            currentState.nextState = state.Value;
                            currentState.stage = States.EVENT.EXIT;
                            //Debug.Log($"{this.gameObject.name}Changing state to : {currentState.nextState}");

                        }

                }
            }
        currentState = currentState.Process();

        GetNearestAI();
        KeepDistance();

       //Debug.Log($"{this.gameObject.name}Current state : {currentState.name}");
       

        if (weapon.ammoCount == 0)
        {
            handLayer.weight = 1f;
        }

        if(weapon.ammoCount >= 1)
        {
            handLayer.weight = 0f;
        }

        if (player.isDead)
        {
            health.healthBar.gameObject.SetActive(false);
        }

    }
   

    public void DestroyAgent()
    {
        Destroy(this.gameObject);
    }

    public bool SuspectLocation()
    {
        if (sound.hearSomething)
        {
            Vector3 direction = this.transform.position - soundLocation;
            if (direction.magnitude < hearingRange && soundIntensity > maxIntensity)
            {
                //Debug.Log("suspecting");
                return true;
            }
            return false;
        }
        return false;
    }
    public void SetAi()
    {
        for (int i = 0; i < otherAI.Length; i++)
        {
            if (otherAI[i] != null)
            {
                if (otherAI[i].currentState.name != States.STATE.ATTACK && otherAI[i].currentState.name != States.STATE.PURSUE && otherAI[i].currentState.name != States.STATE.COVERATTACK)
                {

                    //Debug.Log($"{this.gameObject.name} setting {otherAI[i].gameObject.name} to pursue");
                    otherAI[i].currentState.nextState = new Pursue(this.gameObject, agent, player, myAi);
                    otherAI[i].currentState.stage = States.EVENT.EXIT;

                }
            }
        }

    }
    public void GetNearestAI()
    {
        Collider[] aiCollided = Physics.OverlapSphere(agent.transform.position, coverCollider.radius, aiLayer);
       
        for (int i = 0; i < aiCollided.Length; i++)
        {
            ai = aiCollided[i].gameObject.GetComponent<Processor>();
            if (ai != null && ai.gameObject != this.gameObject)
            {
                otherAI[i] = ai;
            }
            
        }
    }

    private void KeepDistance()
    {
        for (int i = 0; i < otherAI.Length; i++)
        {
            if (otherAI[i] != null)
            {
                float dist = Vector3.Distance(transform.position, otherAI[i].transform.position);        
                if (dist < 4f)
                {
                    Vector3 movDir = (this.transform.position - otherAI[i].transform.position).normalized;
                    Vector3 newPos = transform.position + movDir * (4f - dist);
                    transform.position = newPos;
                }

            }
        }


    }

    private void SetAnimations()
    {

        movementAnimator.SetFloat("speed", agent.speed);
        movementAnimator.SetBool("crouch", currentState.crouching);
        weaponAnimator.SetFloat("WeaponPos", agent.speed);
        movementAnimator.SetBool("crouch", currentState.crouching);
        movementAnimator.SetFloat("CrouchToStand", currentState.crouchShoot);
    }



}
