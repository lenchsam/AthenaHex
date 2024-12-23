using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class Units : MonoBehaviour
{
    public int MaxMovement = 1;
    [HideInInspector] public bool TookTurn;
    [BoxGroup("Attacking")]
    [SerializeField] protected int _maxAttackDistance = 1;
    protected TurnManager _turnManager;
    protected PlayerController _playerController;
    protected UnitManager _unitManager;
    protected unitTypes e_defenceType;
    [BoxGroup("Attacking")]
    [SerializeField] protected int _damage;

    [SerializeField] protected List<GameObject> _walkableTiles = new List<GameObject>();
    protected virtual void Start(){
        _turnManager = FindAnyObjectByType<TurnManager>();
        _playerController = FindAnyObjectByType<PlayerController>();
        _unitManager = FindAnyObjectByType<UnitManager>();
        _turnManager.NextTurn.AddListener(nextTurn);
    }
    protected UnityEvent<int> DamagedEvent  = new UnityEvent<int>();
    void nextTurn(){
        TookTurn = false;
    }
}
public enum unitTypes{
    Melee,
    Ranged
}
