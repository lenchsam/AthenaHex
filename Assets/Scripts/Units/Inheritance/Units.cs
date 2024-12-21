using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class Units : MonoBehaviour
{
    public int MaxMovement = 1;
    [BoxGroup("Attacking")]
    [SerializeField] protected int _maxAttackDistance = 1;

    protected PlayerController _playerController;
    protected UnitManager _unitManager;
    protected unitTypes e_defenceType;
    [BoxGroup("Attacking")]
    [SerializeField] protected int _damage;
    [HideInInspector] public bool TookTurn;

    [SerializeField] protected List<GameObject> _walkableTiles = new List<GameObject>();

    protected virtual void Start(){
        _playerController = FindAnyObjectByType<PlayerController>();
        _unitManager = FindAnyObjectByType<UnitManager>();
    }
    protected UnityEvent<int> DamagedEvent  = new UnityEvent<int>();
}
public enum unitTypes{
    Melee,
    Ranged
}
