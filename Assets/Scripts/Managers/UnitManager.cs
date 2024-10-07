using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] GameObject StartUnit;
    [HideInInspector] public Transform SelectedUnit;
    bool unitSelected = false;
    [HideInInspector] public bool attackedThisTurn = false;
    private HexGrid hexGrid;
    TurnManager turnManager;
    PathFinding pathFinding;
    [HideInInspector] public List<PlayerScriptableObject> SO_Players = new List<PlayerScriptableObject>();
    [SerializeField] PlayerScriptableObject playerSO;

    public Vector2Int[] startPositions;
    void Start()
    {
        turnManager = FindAnyObjectByType<TurnManager>();
        hexGrid = FindAnyObjectByType<HexGrid>();
        pathFinding = FindAnyObjectByType<PathFinding>();
        playerController = FindAnyObjectByType<PlayerController>();
        hexGrid.OnMapGenerated.AddListener(startUnits);
        //SetupStartUnits(StartUnit, startPositions);
    }
    public void unitController(RaycastHit hit){
        if (hit.transform.tag == "Tile" && unitSelected){ //if hit a tile and already have a unit selected
            Vector2 targetCords = new Vector2(hit.transform.GetComponent<TileScript>().transform.position.x, hit.transform.GetComponent<TileScript>().transform.position.z);
            Vector2 startCords = new Vector2(SelectedUnit.position.x, SelectedUnit.position.z);
            //get the path to the target position
            Vector2Int startCoords = hexGrid.GetIntCordsFromPosition(startCords);
            Debug.Log(startCoords + " start Cords");
            Vector2Int targetCoords = hexGrid.GetIntCordsFromPosition(targetCords);
            Debug.Log(targetCoords + " target Cords");
            List<GameObject> path = pathFinding.FindPath(startCoords, targetCoords);

            //pass the tile node the reference to the unit that is stood on it
            TileScript targetNode = hit.transform.gameObject.GetComponent<TileScript>();
            targetNode = hexGrid.GetTileFromIntCoords(targetNode.intCoords).GetComponent<TileScript>();
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
                if(hexGrid.DistanceBetweenTiles(startCoords, targetCoords) > SelectedUnit.GetComponent<Units>().maxMovement){return;}
                //reveales the tile where the unit moved

                //reveal tiles
                if(hexGrid.showFOW){hexGrid.RevealTile(hexGrid.GetTileFromIntCoords(new Vector2(targetCoords.x, targetCoords.y)).GetComponent<TileScript>());}

                //pathfinding
                StartCoroutine(lerpToPosition(SelectedUnit.transform.position, path, pathFinding.unitMovementSpeed, SelectedUnit.gameObject));

                //set the node you moved to as occupied
                targetNode.occupiedUnit = SelectedUnit.gameObject; 
                
                hexGrid.BlockTile(targetCords);//set the tile that the unit will travel to as none walkable
                hexGrid.UnblockTile(startCords);//sets the current tile as walkable

                //reseting variables
                TileScript startNode = hexGrid.GetTileScript(startCords);
                startNode.occupiedUnit = null;
                SelectedUnit = null;
                unitSelected = false;
                playerController.selectedTile = null;
                playerController.tileUI.SetActive(false);
            }
        }
    }
    public void SelectUnit(){
        //checks if the tile has a unit on it
        if(playerController.selectedTile.GetComponent<TileScript>().occupiedUnit == null){
            playerController.selectedTile = null;
            return;
        }
        
        //check its part of the correct team
        if(playerController.selectedTile.GetComponent<TileScript>().occupiedUnit.GetComponent<AssignTeam>().defenceTeam != turnManager.playerTeam){return;}

        //selectes the unit
        //Debug.Log("selected Unit");
        SelectedUnit = playerController.selectedTile.GetComponent<TileScript>().occupiedUnit.transform;
        unitSelected = true;
    }
    private IEnumerator lerpToPosition(Vector3 startPosition, List<GameObject> targetPositions, float unitMovementSpeed, GameObject gameObjectToMove){
        // Set initial position
        //startPosition.y += 1f;
        //gameObjectToMove.transform.position = startPosition;

        // Iterate through each target position in the list
        foreach (GameObject targetPosition in targetPositions)
        {
            // Offset the target position by 1 on the y-axis
            Vector3 adjustedTarget = new Vector3(targetPosition.transform.position.x, targetPosition.transform.position.y + 1f, targetPosition.transform.position.z);

            
            float distance = Vector3.Distance(gameObjectToMove.transform.position, adjustedTarget);
            float duration = distance / unitMovementSpeed;  // Calculate duration based on speed

            float timeElapsed = 0f;

            // Move towards the current target position
            while (timeElapsed < duration)
            {
                gameObjectToMove.transform.position = Vector3.Lerp(gameObjectToMove.transform.position, adjustedTarget, timeElapsed / duration);

                timeElapsed += Time.deltaTime;

                yield return null;  // Wait until the next frame
            }

            // Ensure the object reaches the target position exactly
            gameObjectToMove.transform.position = adjustedTarget;
        }
    }
    private void startUnits(){
        SetupStartUnits(StartUnit, startPositions);
    }
    private void SetupStartUnits(GameObject unitPrefab, Vector2Int[] TilePositions){
        Team currentTeam = Team.Team1;
        turnManager.playerTeam = Team.Team1;
        foreach(Vector2Int tilePos in TilePositions){
            TileScript tileScript = hexGrid.GetTileFromIntCoords(tilePos).GetComponent<TileScript>();
            tileScript.isWalkable = false;
            tileScript.occupiedUnit = Instantiate(unitPrefab, tileScript.gameObject.transform.position, Quaternion.identity);
            PlayerScriptableObject SO = Instantiate(playerSO);
            SO.Team = currentTeam;
            SO_Players.Add(SO);

            //assign the team
            AssignTeam unitTeam = tileScript.occupiedUnit.GetComponent<AssignTeam>();
            unitTeam.defenceTeam = currentTeam;
            currentTeam += 1;

            //reveal the tiles around the unit
            if(hexGrid.showFOW){hexGrid.RevealTile(tileScript);}
            turnManager.playerTeam += 1;
        }
        turnManager.playerTeam = Team.Team1;

        //reblock all tiles that arent team 1
        foreach(PlayerScriptableObject SO in SO_Players){
            if(SO.Team != Team.Team1){
                foreach(Vector2Int vInt in SO.RevealedTiles){
                    hexGrid.GetTileFromIntCoords(vInt).GetComponent<TileScript>().ReBlock();
                }
            }
        }
    }
}
