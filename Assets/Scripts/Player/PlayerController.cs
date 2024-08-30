using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour
{
    public UnityEvent<Vector2, GameObject> OnUnitMove = new UnityEvent<Vector2, GameObject>();
    TurnManager turnManager;
    Transform SelectedUnit;
    bool unitSelected = false;
    [HideInInspector] public bool attackedThisTurn = false;

    //private variables
    private HexGrid hexGrid;
    void Start()
    {
        turnManager = FindAnyObjectByType<TurnManager>();
        hexGrid = FindAnyObjectByType<HexGrid>();
    }
    public void Clicked(InputAction.CallbackContext context){
        if (!context.performed){return;}
        
        //-----------------------------------------------------------------------------------------------------------------------------------------------
        //raycast

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        
        bool hasHit = Physics.Raycast(ray, out hit);
        
        if(!hasHit){return;} //return if its hit nothing
        //-----------------------------------------------------------------------------------------------------------------------------------------------

        if (hit.transform.tag == "Tile" && unitSelected){ //if hit a tile and already have a unit selected
            
            Vector2 targetCords = new Vector2(hit.transform.GetComponent<TileScript>().coords.x, hit.transform.GetComponent<TileScript>().coords.y);
            Vector2 startCords = new Vector2(SelectedUnit.position.x, SelectedUnit.position.z);

            //pass the tile node the reference to the unit that is stood on it
            TileScript targetNode = hit.transform.gameObject.GetComponent<TileScript>();
            GameObject targetUnit = targetNode.occupiedUnit; 

            //if target tile is occupied by an enemy/check for attacked this turn. if true return
            if(targetUnit != null){
                // If the target tile is occupied by an enemy, handle the attack
                //Debug.Log("tile occupied by an enemy");

                if (targetUnit.GetComponent<AssignTeam>().defenceTeam != SelectedUnit.GetComponent<AssignTeam>().defenceTeam) {
                    // Trigger the attack event
                    OnUnitMove.Invoke(targetCords, SelectedUnit.gameObject);
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
                
                hexGrid.blockTile(targetCords);//set the tile that the unit will travel to as none walkable
                hexGrid.unblockTile(startCords);//sets the current tile as walkable

                //reseting variables
                SelectedUnit = null;
                unitSelected = false;
            }

            //if attackedThisTurn == false then trigger the event, dont move

            //if tile is not occupied by an enemy, check if the tile is walkable

            //Debug.Log(SelectedUnit.GetComponent<AssignTeam>().defenceTeam + " selected unit");
            //Debug.Log(targetUnit.GetComponent<AssignTeam>().defenceTeam + " target unit");
        }else if(hit.transform.tag == "Unit"){
            //-----------------------------------------------------------------------------------------------------------------------------------------------
            //select the unit clicked
            AssignTeam team = hit.transform.gameObject.GetComponent<AssignTeam>();

            if (team.defenceTeam != turnManager.playerTeam){return;}//if the defence is not on the players team

            SelectedUnit = hit.transform;
            //Debug.Log("UNIT SELECTED");
            unitSelected = true;
        }
    }
}
