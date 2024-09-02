using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Settler : Units
{
    [SerializeField] private InputActionAsset controls;
    private InputAction foundCity;
    private InputActionMap _inputActionMap;
    private CitiesManager citiesManager;
    private HexGrid gridManager;

    protected override void Start(){
        base.Start();

        citiesManager = FindObjectOfType<CitiesManager>();
        gridManager = FindObjectOfType<HexGrid>();

        _inputActionMap = controls.FindActionMap("Player");
        foundCity = _inputActionMap.FindAction("Ability");
        foundCity.performed += startCity;
    }
    private void startCity(InputAction.CallbackContext obj){
        
        //Debug.Log("CITY IS FOUNDED");

        //get tile unit is stood on

        //check if its a city already

        //if no, check tiles in a 1 tile radius to see if theyre cities;
        
        //if no, found the city
        var SO = ScriptableObject.CreateInstance<CitiesScriptableObject>();
        Vector2 tileCords = gridManager.GetCoordinatesFromPosition(transform.position);
        citiesManager.initialiseCity(SO, gridManager.GetTile(new Vector2(tileCords.x, tileCords.y)));
    }
}
