using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior : Melee, IAttacking
{
    [SerializeField] protected float maximumHealth = 50.0f;
    protected override void Awake()
    {
        base.Awake();
        initialise(maximumHealth);
    }

    // Update is called once per frame
    protected override void Start()
    {
        base.Start();
    }
    public void attack(GameObject thingToAttack){
        //if thing is x tiles away then attack
        thingToAttack.GetComponent<IDamageable>().takeDamage(damage);
    }
}
