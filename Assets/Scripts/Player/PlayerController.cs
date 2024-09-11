using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class PlayerController : MonoBehaviour
{
    [HideInInspector] public UnityEvent<Vector2, GameObject> OnUnitMove = new UnityEvent<Vector2, GameObject>();
    TurnManager turnManager;
    [SerializeField] Transform SelectedUnit;
    [SerializeField] bool unitSelected = false;
    [HideInInspector] public bool attackedThisTurn = false;
    [SerializeField] int LM;

    //private variables
    private HexGrid hexGrid;
    void Start()
    {
        turnManager = FindAnyObjectByType<TurnManager>();
        hexGrid = FindAnyObjectByType<HexGrid>();

        LM = LayerMask.GetMask("Tile");
    }
    public void Clicked(InputAction.CallbackContext context){
        if (!context.performed){return;}

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        bool hasHit = Physics.Raycast(ray, out hit, Mathf.Infinity, LM);
        

        if(!hasHit){return;} //return if its hit nothing

        unitController(hasHit, hit);
        cityCheck(hasHit, hit);
    }
    private void unitController(bool hasHit, RaycastHit hit){
        if (hit.transform.tag == "Tile" && unitSelected){ //if hit a tile and already have a unit selected
            Vector2 targetCords = new Vector2(hit.transform.GetComponent<TileScript>().transform.position.x, hit.transform.GetComponent<TileScript>().transform.position.z);
            Vector2 startCords = new Vector2(SelectedUnit.position.x, SelectedUnit.position.z);

            //pass the tile node the reference to the unit that is stood on it
            TileScript targetNode = hit.transform.gameObject.GetComponent<TileScript>();
            GameObject targetUnit = targetNode.occupiedUnit; 

            //if target tile is occupied by an enemy/check for attacked this turn. if true return
            if(targetUnit != null){
                // If the target tile is occupied by an enemy, handle the attack

                if(SelectedUnit == null){
                    //Debug.Log("no selected unit");
                    return;
                }
                //Debug.Log("tile occupied by an enemy");

                if (targetUnit.GetComponent<AssignTeam>().defenceTeam != SelectedUnit.GetComponent<AssignTeam>().defenceTeam) {
                    // Trigger the attack event
                    
                    //Debug.Log(SelectedUnit.GetComponent<IAttacking>());
                    SelectedUnit.GetComponent<IAttacking>().attack(targetUnit);

                    SelectedUnit = null;
                    unitSelected = false;
                    targetUnit = null;
                }

                // Prevent movement after attacking
                if (attackedThisTurn) {
                    attackedThisTurn = false;
                    return;  // Stop further movement
                }
            }else if (!targetNode.isWalkable) {
                //Debug.Log("Tile not walkable");
                // If the target tile is not walkable and doesn't have an enemy, return early


            return;
            }else{
                // If the tile is walkable and empty, move the unit
                //Debug.Log("MOVING");
                //need to change this to lerp rather than teleport...... maybe pathfinding
                SelectedUnit.transform.position = new Vector3(targetCords.x, SelectedUnit.position.y, targetCords.y);

                targetNode.occupiedUnit = SelectedUnit.gameObject; //set the node you moved to as occupied

                TileScript startNode = hexGrid.GetTileScript(startCords);

                startNode.occupiedUnit = null;
                
                hexGrid.BlockTile(targetCords);//set the tile that the unit will travel to as none walkable
                hexGrid.UnblockTile(startCords);//sets the current tile as walkable

                //reseting variables
                SelectedUnit = null;
                unitSelected = false;
            }

            //if attackedThisTurn == false then trigger the event, dont move

            //if tile is not occupied by an enemy, check if the tile is walkable

            //Debug.Log(SelectedUnit.GetComponent<AssignTeam>().defenceTeam + " selected unit");
            //Debug.Log(targetUnit.GetComponent<AssignTeam>().defenceTeam + " target unit");
        }else if(hit.transform.gameObject.GetComponent<TileScript>().occupiedUnit != null){ //if the tile has a unit on
            //-----------------------------------------------------------------------------------------------------------------------------------------------
            //select the unit clicked
            //if(hit.transform.gameObject.GetComponent<TileScript>().occupiedUnit == null){return;}//if the tile doesnt have a unit on it

            AssignTeam team = hit.transform.gameObject.GetComponent<TileScript>().occupiedUnit.GetComponent<AssignTeam>();

            if (team.defenceTeam != turnManager.playerTeam){return;}//if the defence is not on the players team then return

            SelectedUnit = team.gameObject.transform;
            //Debug.Log("UNIT SELECTED");
            unitSelected = true;
        }
    }
    private void cityCheck(bool hasHit, RaycastHit hit){
        if(hit.transform.tag != "Tile"){return;}

        if(hit.transform.gameObject.GetComponent<TileScript>().isCityCentre == true){
            Debug.Log("trueeeeee");
        }
    }
}
