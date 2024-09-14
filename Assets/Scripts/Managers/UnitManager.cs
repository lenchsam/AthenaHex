using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    [SerializeField] Transform SelectedUnit;
    [SerializeField] bool unitSelected = false;
    [HideInInspector] public bool attackedThisTurn = false;
    private HexGrid hexGrid;
    TurnManager turnManager;
    PathFinding pathFinding;
    void Start()
    {
        turnManager = FindAnyObjectByType<TurnManager>();
        hexGrid = FindAnyObjectByType<HexGrid>();
        pathFinding = FindAnyObjectByType<PathFinding>();
    }
    public void unitController(bool hasHit, RaycastHit hit){
        if (hit.transform.tag == "Tile" && unitSelected){ //if hit a tile and already have a unit selected
            Vector2 targetCords = new Vector2(hit.transform.GetComponent<TileScript>().transform.position.x, hit.transform.GetComponent<TileScript>().transform.position.z);
            Vector2 startCords = new Vector2(SelectedUnit.position.x, SelectedUnit.position.z);
            
            //get the path to the target position
            Vector2Int startCoords = hexGrid.GetIntCordsFromPosition(startCords);
            Vector2Int targetCoords = hexGrid.GetIntCordsFromPosition(targetCords);
            List<GameObject> path = pathFinding.FindPath(startCoords, targetCoords);

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
                StartCoroutine(lerpToPosition(SelectedUnit.transform.position, path, pathFinding.unitMovementSpeed, SelectedUnit.gameObject));
                //SelectedUnit.transform.position = new Vector3(targetCords.x, SelectedUnit.position.y, targetCords.y);

                targetNode.occupiedUnit = SelectedUnit.gameObject; //set the node you moved to as occupied

                TileScript startNode = hexGrid.GetTileScript(startCords);

                startNode.occupiedUnit = null;
                
                hexGrid.BlockTile(targetCords);//set the tile that the unit will travel to as none walkable
                hexGrid.UnblockTile(startCords);//sets the current tile as walkable

                //reseting variables
                SelectedUnit = null;
                unitSelected = false;
            }
        }else if(hit.transform.gameObject.GetComponent<TileScript>().occupiedUnit != null){ //if the tile has a unit on
            //-----------------------------------------------------------------------------------------------------------------------------------------------
            //select the unit clicked
            //if(hit.transform.gameObject.GetComponent<TileScript>().occupiedUnit == null){return;}//if the tile doesnt have a unit on it

            AssignTeam team = hit.transform.gameObject.GetComponent<TileScript>().occupiedUnit.GetComponent<AssignTeam>();

            if(team.gameObject.GetComponent<Units>().tookTurn){
                Debug.Log("unit has moved this turn");
                return;
            }

            if (team.defenceTeam != turnManager.playerTeam){return;}//if the defence is not on the players team then return

            SelectedUnit = team.gameObject.transform;
            //Debug.Log("UNIT SELECTED");
            unitSelected = true;
        }
    }
    private IEnumerator lerpToPosition(Vector3 startPosition, List<GameObject> targetPositions, float unitMovementSpeed, GameObject gameObjectToMove){
        // Set initial position
        gameObjectToMove.transform.position = startPosition;

        // Iterate through each target position in the list
        foreach (GameObject targetPosition in targetPositions)
        {
            float distance = Vector3.Distance(gameObjectToMove.transform.position, targetPosition.transform.position);
            float duration = distance / unitMovementSpeed;  // Calculate duration based on speed

            float timeElapsed = 0f;

            // Move towards the current target position
            while (timeElapsed < duration)
            {
                gameObjectToMove.transform.position = Vector3.Lerp(gameObjectToMove.transform.position, targetPosition.transform.position, timeElapsed / duration);

                timeElapsed += Time.deltaTime;

                yield return null;  // Wait until the next frame
            }

            // Ensure the object reaches the target position exactly
            gameObjectToMove.transform.position = targetPosition.transform.position;
        }
    }
}
