using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;public class PlayerController : MonoBehaviour
{
    public GameObject tileUI;
    [HideInInspector] public UnityEvent<Vector2, GameObject> OnUnitMove = new UnityEvent<Vector2, GameObject>();
    TurnManager turnManager;
    int LM;
    private UnitManager unitManager;
    private DistrictManager districtManager;
    private Building building;

    public GameObject selectedTile;

    bool pointerOverUI = false;

    //private variables
    private HexGrid hexGrid;
    void Start()
    {
        turnManager = FindAnyObjectByType<TurnManager>();
        unitManager = FindAnyObjectByType<UnitManager>();
        hexGrid = FindAnyObjectByType<HexGrid>();
        districtManager = FindAnyObjectByType<DistrictManager>();
        building = FindAnyObjectByType<Building>();

        LM = LayerMask.GetMask("Tile");
    }

    void Update()
    {
        pointerOverUI = EventSystem.current.IsPointerOverGameObject();
    }
    public void Clicked(InputAction.CallbackContext context){
        if (!context.performed){return;}
        
        //--------------------------------------------------------------------------------------------------
        //CHECKING IF PLAYER CLICKED UI

        // Initialize PointerEventData with current mouse position
        if(pointerOverUI){
            //Debug.Log("ponter is over UI");
            return;
        }

        //--------------------------------------------------------------------------------------------------
        //raycast

        tileUI.SetActive(false);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool hasHit = Physics.Raycast(ray, out hit, Mathf.Infinity, LM);
        

        if(!hasHit){return;} //return if its hit nothing

        //handles the movement if the tile is hidden
        if(hit.transform.gameObject.GetComponent<TileScript>().isFog){
            //Debug.Log("IS FOG");
            unitManager.unitController(hit);
            return;
        }
        
        // Debug.Log("reset tile");
        selectedTile = hit.transform.gameObject;
        tileUI.SetActive(true);

        //--------------------------------------------------------------------------------------------------
        //if waiting for placing barracks placement

        if(districtManager.waitingForClick){
            districtManager.BuildBarracks(hit);
            districtManager.waitingForClick = false;
            return;
        }
        
        //assigns the correct scriptable object for the district manager. used for checking placing defences in the correct city
        if(hit.transform.gameObject.GetComponent<TileScript>().districts == district.CityCentre){
            districtManager.citiesScriptableObject = hit.transform.gameObject.GetComponent<TileScript>().SO_Cities;
        }else{
            districtManager.citiesScriptableObject = null;
        }

        //--------------------------------------------------------------------------------------------------

        
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
