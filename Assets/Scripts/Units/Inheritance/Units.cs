using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class Units : ACDefence
{
    public int maxMovement = 1;
    [BoxGroup("Attacking")]
    [SerializeField] protected int MaxAttackDistance = 1;

    protected PlayerController playerController;
    protected UnitManager unitManager;
    protected unitTypes defenceType;
    [BoxGroup("Attacking")]
    [SerializeField] public int damage;
    [HideInInspector] public bool tookTurn;

    [SerializeField] protected List<GameObject> walkableTiles = new List<GameObject>();

    protected virtual void Start(){
        playerController = FindAnyObjectByType<PlayerController>();
        unitManager = FindAnyObjectByType<UnitManager>();
    }
    protected UnityEvent<int> DamagedEvent  = new UnityEvent<int>();
}
public enum unitTypes{
    Melee,
    Ranged
}
