using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.EventSystems;public class PlayerController : MonoBehaviour
{
    public GameObject TileUI;
    [HideInInspector] public UnityEvent<Vector2, GameObject> OnUnitMove = new UnityEvent<Vector2, GameObject>();
    int _LM;
    private UnitManager _unitManager;
    private DistrictManager _districtManager;
    private Building _building;

    public GameObject SelectedTile;

    bool _pointerOverUI = false;

    //private variables
    private HexGrid _hexGrid;
    void Start()
    {
        _unitManager = FindAnyObjectByType<UnitManager>();
        _hexGrid = FindAnyObjectByType<HexGrid>();
        _districtManager = FindAnyObjectByType<DistrictManager>();
        _building = FindAnyObjectByType<Building>();

        _LM = LayerMask.GetMask("Tile");
    }

    void Update()
    {
        _pointerOverUI = EventSystem.current.IsPointerOverGameObject();
    }
    public void Clicked(InputAction.CallbackContext context){
        if (!context.performed){return;}
        
        //--------------------------------------------------------------------------------------------------
        //CHECKING IF PLAYER CLICKED UI

        // Initialize PointerEventData with current mouse position
        if(_pointerOverUI){
            //Debug.Log("ponter is over UI");
            return;
        }

        //--------------------------------------------------------------------------------------------------
        //raycast

        TileUI.SetActive(false);

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool hasHit = Physics.Raycast(ray, out hit, Mathf.Infinity, _LM);
        

        if(!hasHit){return;} //return if its hit nothing

        //handles the movement if the tile is hidden
        // if(hit.transform.gameObject.GetComponent<TileScript>().isFog){
        //     unitManager.unitController(hit);
        //     return;
        // }
        
        SelectedTile = hit.transform.gameObject;
        TileUI.SetActive(true);

        //if tile has a unit on it, select that unit.
        if(SelectedTile.GetComponent<TileScript>().OccupiedUnit != null){
            _unitManager.SelectUnit();
        }

        //--------------------------------------------------------------------------------------------------
        //if waiting for placing barracks placement

        if(_districtManager.WaitingForClick){
            _districtManager.BuildBarracks(hit);
            _districtManager.WaitingForClick = false;
            return;
        }
        
        //assigns the correct scriptable object for the district manager. used for checking placing defences in the correct city
        var tileScript = hit.transform.gameObject.GetComponent<TileScript>();
        if(tileScript.Districts == eDistrict.CityCentre){
            _districtManager.SelectedCitiesScriptableObject = tileScript.SO_Cities;
        }else{
            _districtManager.SelectedCitiesScriptableObject = null;
        }

        //--------------------------------------------------------------------------------------------------
        var interactable = hit.transform.gameObject.GetComponent<IInteractable>();
        if(interactable != null){ //if the hit object is clickable
            interactable.OnClick();
        }

        if(_unitManager.SelectedUnit != null && !_unitManager.SelectedUnit.GetComponent<Units>().TookTurn){
            _unitManager.unitController(hit);
        }
        //building.PlaceDown(hit);
        //cityCheck(hasHit, hit);
    }
    
    private void cityCheck(bool hasHit, RaycastHit hit){
        if(hit.transform.tag != "Tile"){return;}

        if(hit.transform.gameObject.GetComponent<TileScript>().IsCityCentre == true){
            Debug.Log("trueeeeee");
        }
    }
}
