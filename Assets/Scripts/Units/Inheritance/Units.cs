using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Units : ACDefence
{
    public float maxMovement = 1f; 
    [SerializeField] protected int MaxAttackDistance = 1;

    protected PlayerController playerController;
    protected unitTypes defenceType;
    [SerializeField] public int damage;
    [HideInInspector] public bool tookTurn;

    protected virtual void Start(){
        playerController = FindAnyObjectByType<PlayerController>();
    }
    protected UnityEvent<int> DamagedEvent  = new UnityEvent<int>();
}
public enum unitTypes{
    Melee,
    Ranged
}
