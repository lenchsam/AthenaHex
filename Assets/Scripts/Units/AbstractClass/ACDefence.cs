using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ACDefence : MonoBehaviour, IDamageable
{
    [BoxGroup("Health")]
    [SerializeField] protected float health;
    [BoxGroup("Health")]
    [SerializeField] protected float maxHealth;
    protected virtual void initialise(float maximumHealth)
    {
        maxHealth = maximumHealth;
        health = maxHealth;
    }
    public virtual void takeDamage(int damageToTake){
        health -= damageToTake;
        Debug.Log(gameObject.name + " health = " + health);

        if (health <= 0)
        {
            die();
        }
    }
    public virtual void die()
    {
        //current node stood on set back to walkable
        Debug.Log("UNIT DEADDDDD");
    }   
}
