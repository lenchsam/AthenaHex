using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Settler : Units, IAttacking
{
    //input variables
    [SerializeField] private InputActionAsset _controls;
    private InputAction _foundCity;
    private InputActionMap _inputActionMap;

    private CitiesManager _citiesManager;

    protected override void Start(){
        base.Start();

        _citiesManager = FindAnyObjectByType<CitiesManager>();

        _inputActionMap = _controls.FindActionMap("Player");
        _foundCity = _inputActionMap.FindAction("Ability");
        _foundCity.performed += startCity;
    }
    private void startCity(InputAction.CallbackContext obj){
        if(_unitManager.SelectedUnit != gameObject.transform){return;} //if they havent selected this settler return
        //if this tile is already part of a city

        _citiesManager.MakeNewCity(transform.position);
    }
    public void attack(GameObject thingToAttack){
        Debug.Log("This unit cannot attack");
    }
}
