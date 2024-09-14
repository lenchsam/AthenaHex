using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Units : ACDefence
{
    public float movementSpeed = 1f; 

    protected PlayerController playerController;
    protected unitTypes defenceType;
    [SerializeField] public int damage;
    [HideInInspector] public bool tookTurn;

    protected virtual void Start(){
        playerController = FindObjectOfType<PlayerController>();
    }
    protected UnityEvent<int> DamagedEvent  = new UnityEvent<int>();
}
public enum unitTypes{
    Melee,
    Ranged
}
