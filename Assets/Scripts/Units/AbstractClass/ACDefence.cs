using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ACDefence : MonoBehaviour
{
    protected float health { get; set; }
    protected float maxHealth { get; set; }
    protected virtual void initialise(float maximumHealth)
    {
        maxHealth = maximumHealth;
        health = maxHealth;
    }
    public virtual void TakeDamage(int damageToTake){
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
