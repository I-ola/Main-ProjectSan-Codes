using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    [HideInInspector]
    public float currentHealth;
    protected float force = 10f ;

    Hitbox hitbox;
    Ragdoll ragdoll;
    public UIHealthBar healthBar;
    public GameObject head;
    public GameObject torso;
    public GameObject hips;
    public GameObject[] hands;
    public GameObject[] legs;
    private bool dead = false;
    public bool takingDamage = false;


    private float minHealth = 10.0f;
    private float recoveryAmount = 5.0f;

    protected Dictionary<GameObject, float> damageMultiplier = new Dictionary<GameObject, float>();
    // Start is called before the first frame update
    void Start()
    {
        ragdoll = GetComponent<Ragdoll>();
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<UIHealthBar>();
        OnStart();

        var rigidBodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rigidBody in rigidBodies)
        {
            hitbox = rigidBody.gameObject.AddComponent<Hitbox>();
            hitbox.health = this;
            if(hitbox.gameObject != gameObject)
            {
                hitbox.gameObject.layer = LayerMask.NameToLayer("Hitbox");
            }
        }

        damageMultiplier[head] = 5.0f;
        damageMultiplier[torso] = 1.0f;
        damageMultiplier[hips] = 1.0f;

        foreach (GameObject hand in hands)
        {
            damageMultiplier[hand] = 0.8f;
        }

        foreach (GameObject leg in legs)
        {
            damageMultiplier[leg] = 0.8f;
        }

       
    }
    private void Update()
    {
        if (!takingDamage && currentHealth < maxHealth && !dead)
        {
            Heal();
        }
        
    }

    private void Heal()
    {
        currentHealth += recoveryAmount * Time.deltaTime;
        currentHealth = Mathf.Clamp(currentHealth, minHealth ,maxHealth);
        if (healthBar != null)
        {
            healthBar.SetHealthBar(currentHealth / maxHealth);
        }
        
        OnHeal(currentHealth);

    }

    public void TakeDamage(float damage,GameObject bodyPart ,Vector3 direction)
    {
        
        GameObject bodyPartHit = DetermineBodyPart(bodyPart);
        if (damageMultiplier.ContainsKey(bodyPartHit))
        {
            OnDamage(direction);
            float damageMultiplied = damageMultiplier[bodyPart];
            float newDamage = damageMultiplied * damage;
            currentHealth -= newDamage;
            if(healthBar !=null)
            {
              
                healthBar.SetHealthBar(currentHealth / maxHealth);
            }
 
        }

        StartCoroutine(StartHealing());

        if (currentHealth <=0 )
        {
            Die(direction);
           
        }
    }

    IEnumerator StartHealing()
    {
        yield return new WaitForSeconds(5.0f);
        takingDamage = false;

    }
    private void Die(Vector3 direction)
    {
        ragdoll.ActivateRagdoll();
        direction.y = 0;
       // ragdoll.ApplyForce(direction * force);
        OnDeath(direction);
        dead = true;
        if(healthBar != null)
        {
            healthBar.gameObject.SetActive(false);
        }
        

    }

    private GameObject DetermineBodyPart(GameObject objectHit)
    {
       
            if ( head == objectHit)
            {
            // Debug.Log("head");
                takingDamage = true;
                 return head;
            }

            else if (torso == objectHit)
            {
            //  Debug.Log("body");
                takingDamage = true;
                return torso;
            }

            else if(hips == objectHit)
            {
            //  Debug.Log("hips");
                takingDamage = true;
                return hips;
            }

            else if (Array.Exists(hands, hand => hand == objectHit))
            {
            // Debug.Log("hands");
                takingDamage = true;
                return objectHit;
            }

            else if (Array.Exists(legs, leg => leg == objectHit))
            {
            // Debug.Log("legs");
                takingDamage = true;
                return objectHit;
            }

        return null;
    }

    protected virtual void OnStart()
    {

    }

    protected virtual void OnDamage(Vector3 direction)
    {

    }

    protected virtual void OnDeath(Vector3 direction)
    {

    }

    protected virtual void OnHeal(float amount)
    {

    }
}
