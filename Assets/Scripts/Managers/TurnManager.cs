using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [HideInInspector] public Team playerTeam;
    private UnitManager unitManager;
    private HexGrid hexGrid;
    [SerializeField] Transform cameraTransform;
    //public List<Vector2Int> RevealedTilesP2 = new List<Vector2Int>();
    //public List<Vector2Int> RevealedTilesP3 = new List<Vector2Int>();
    //public List<Vector2Int> RevealedTilesP4 = new List<Vector2Int>();
    private void Start(){
        unitManager = FindAnyObjectByType<UnitManager>();
        hexGrid = FindAnyObjectByType<HexGrid>();
    }
    public void EndTurn()
    {
        switch (playerTeam)
        {
            case Team.Team1:
                playerTeam = Team.Team2;
                ChangeCamera(1);
                ChangeFOW(unitManager.SO_Players[1].RevealedTiles, unitManager.SO_Players[0].RevealedTiles);
                //hide all p1 units. 
                //reveal all p2 units
                break;
            case Team.Team2:
                playerTeam = Team.Team3;
                ChangeCamera(2);
                ChangeFOW(unitManager.SO_Players[2].RevealedTiles, unitManager.SO_Players[1].RevealedTiles);
                //hide all p2 units. 
                //reveal all p3 units
                break;

            case Team.Team3:
                playerTeam = Team.Team4;
                ChangeCamera(3);
                ChangeFOW(unitManager.SO_Players[3].RevealedTiles, unitManager.SO_Players[2].RevealedTiles);
                //hide all p3 units. 
                //reveal all p4 units
                break;

            case Team.Team4:
                playerTeam = Team.Team1;
                ChangeCamera(0);
                ChangeFOW(unitManager.SO_Players[0].RevealedTiles, unitManager.SO_Players[3].RevealedTiles);
                //hide all p4 units. 
                //reveal all p1 units
                break;
        }
    }
    private void ChangeCamera(int team){
        Vector2Int Cords = unitManager.startPositions[team];
        Vector3 pos = new Vector3(hexGrid.GetTileFromIntCoords(Cords).transform.position.x,cameraTransform.position.y, hexGrid.GetTileFromIntCoords(Cords).transform.position.z);
        cameraTransform.position = pos;
    }
    private void ChangeFOW(List<Vector2Int> RevealedTiles, List<Vector2Int> TilesToBlock){
        foreach(Vector2Int tileCords in RevealedTiles){
            hexGrid.GetTileFromIntCoords(tileCords).GetComponent<TileScript>().Reveal();
        }
        foreach(Vector2Int tileCords in TilesToBlock){
            hexGrid.GetTileFromIntCoords(tileCords).GetComponent<TileScript>().ReBlock();
        }
    }
}
