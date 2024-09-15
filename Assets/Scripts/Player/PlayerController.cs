using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEditor.Experimental.GraphView;
public class PlayerController : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Vector2, GameObject> OnUnitMove = new UnityEvent<Vector2, GameObject>();
    TurnManager turnManager;
    [SerializeField] int LM;
    private UnitManager unitManager;
    private DistrictManager districtManager;
    private Building building;

    //private variables
    private HexGrid hexGrid;
    void Start()
    {
        turnManager = FindAnyObjectByType<TurnManager>();
        unitManager = FindObjectOfType<UnitManager>();
        hexGrid = FindAnyObjectByType<HexGrid>();
        districtManager = FindAnyObjectByType<DistrictManager>();
        building = FindAnyObjectByType<Building>();

        LM = LayerMask.GetMask("Tile");
    }
    public void Clicked(InputAction.CallbackContext context){
        if (!context.performed){return;}

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool hasHit = Physics.Raycast(ray, out hit, Mathf.Infinity, LM);
        

        if(!hasHit){return;} //return if its hit nothing

        //--------------------------------------------------------------------------------------------------- For
        if(districtManager.waitingForClick){
            districtManager.BuildBarracks(hit);
            districtManager.waitingForClick = false;
            return;
        }

        //if waiting to build
        //  place
        //  return
        
        if(hit.transform.gameObject.GetComponent<IInteractable>() != null){ //if the hit object is clickable
            hit.transform.gameObject.GetComponent<IInteractable>().OnClick();
        }

        unitManager.unitController(hit);
        building.PlaceDown(hit);
        //cityCheck(hasHit, hit);
    }
    
    private void cityCheck(bool hasHit, RaycastHit hit){
        if(hit.transform.tag != "Tile"){return;}

        if(hit.transform.gameObject.GetComponent<TileScript>().isCityCentre == true){
            Debug.Log("trueeeeee");
        }
    }
}
