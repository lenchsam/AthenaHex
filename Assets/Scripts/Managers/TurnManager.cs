using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [HideInInspector] public e_Team PlayerTeam;
    private UnitManager _unitManager;
    private HexGrid _hexGrid;
    [SerializeField] Transform _cameraTransform;
    //public List<Vector2Int> RevealedTilesP2 = new List<Vector2Int>();
    //public List<Vector2Int> RevealedTilesP3 = new List<Vector2Int>();
    //public List<Vector2Int> RevealedTilesP4 = new List<Vector2Int>();
    private void Start(){
        _unitManager = FindAnyObjectByType<UnitManager>();
        _hexGrid = FindAnyObjectByType<HexGrid>();
    }
    public void EndTurn()
    {
        switch (PlayerTeam)
        {
            case e_Team.Team1:
                PlayerTeam = e_Team.Team2;
                ChangeCamera(1);
                ChangeFOW(_unitManager.SO_Players[1].RevealedTiles, _unitManager.SO_Players[0].RevealedTiles);
                //hide all p1 units. 
                //reveal all p2 units
                break;
            case e_Team.Team2:
                PlayerTeam = e_Team.Team3;
                ChangeCamera(2);
                ChangeFOW(_unitManager.SO_Players[2].RevealedTiles, _unitManager.SO_Players[1].RevealedTiles);
                //hide all p2 units. 
                //reveal all p3 units
                break;

            case e_Team.Team3:
                PlayerTeam = e_Team.Team4;
                ChangeCamera(3);
                ChangeFOW(_unitManager.SO_Players[3].RevealedTiles, _unitManager.SO_Players[2].RevealedTiles);
                //hide all p3 units. 
                //reveal all p4 units
                break;

            case e_Team.Team4:
                PlayerTeam = e_Team.Team1;
                ChangeCamera(0);
                ChangeFOW(_unitManager.SO_Players[0].RevealedTiles, _unitManager.SO_Players[3].RevealedTiles);
                //hide all p4 units. 
                //reveal all p1 units
                break;
        }
        _unitManager.SelectedUnit = null;
        _unitManager.UnitSelected = false;
    }
    private void ChangeCamera(int team){
        // Vector2Int Cords = unitManager.startPositions[team];
        // Vector3 pos = new Vector3(hexGrid.GetTileFromIntCoords(Cords).transform.position.x,cameraTransform.position.y, hexGrid.GetTileFromIntCoords(Cords).transform.position.z);
        // cameraTransform.position = pos;
    }
    private void ChangeFOW(List<Vector2Int> RevealedTiles, List<Vector2Int> TilesToBlock){
        foreach(Vector2Int tileCords in RevealedTiles){
            _hexGrid.GetTileScriptFromIntCords(tileCords).Reveal();
        }
        foreach(Vector2Int tileCords in TilesToBlock){
            _hexGrid.GetTileScriptFromIntCords(tileCords).ReBlock();
        }
    }
}
