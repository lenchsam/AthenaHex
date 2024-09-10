using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class Settler : Units
{
    //input variables
    [SerializeField] private InputActionAsset controls;
    private InputAction foundCity;
    private InputActionMap _inputActionMap;

    private CitiesManager citiesManager;

    protected override void Start(){
        base.Start();

        citiesManager = FindObjectOfType<CitiesManager>();

        _inputActionMap = controls.FindActionMap("Player");
        foundCity = _inputActionMap.FindAction("Ability");
        foundCity.performed += startCity;
    }
    private void startCity(InputAction.CallbackContext obj){
        citiesManager.MakeNewCity(transform.position);
    }
}
