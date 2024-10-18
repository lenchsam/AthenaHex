using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Settler : Units, IAttacking
{
    //input variables
    [SerializeField] private InputActionAsset controls;
    private InputAction foundCity;
    private InputActionMap _inputActionMap;

    private CitiesManager citiesManager;
    private UnitManager unitManager;

    protected override void Start(){
        base.Start();

        citiesManager = FindAnyObjectByType<CitiesManager>();
        unitManager = FindAnyObjectByType<UnitManager>();

        _inputActionMap = controls.FindActionMap("Player");
        foundCity = _inputActionMap.FindAction("Ability");
        foundCity.performed += startCity;
    }
    private void startCity(InputAction.CallbackContext obj){
        if(unitManager.SelectedUnit != gameObject.transform){return;} //if they havent selected this settler return
        //if this tile is already part of a city

        citiesManager.MakeNewCity(transform.position);
    }
    public void attack(GameObject thingToAttack){
        Debug.Log("This unit cannot attack");
    }
}
