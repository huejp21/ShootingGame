﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth;
    public float health { get; protected set; }
    protected bool dead;

    public event System.Action OnDeath;


    // Use this for initialization
    protected virtual void Start()
    {
        health = startingHealth;
    }

    //// Update is called once per frame
    //void Update()
    //{

    //}

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 
         && dead == false)
        {
            Die();
        }
    }

    [ContextMenu("Self Destruct")]
    public virtual void Die()
    {
        dead = true;
        if (OnDeath != null)
        {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }




}
