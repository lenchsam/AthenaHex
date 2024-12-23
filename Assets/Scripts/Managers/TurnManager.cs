using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.VirtualTexturing;

public class TurnManager : MonoBehaviour
{
    public UnityEvent NextTurn = new UnityEvent();
    public e_Team PlayerTeam;
    private UnitManager _unitManager;
    private HexGrid _hexGrid;
    private ProceduralGeneration _proceduralGeneration;
    [SerializeField] private List<Vector3> _cameraPositions = new List<Vector3>(); // Used for switchng the camera position when the turn ends. saves the position of the camera when the turn was ended
    [SerializeField] Transform _cameraTransform;
    //public List<Vector2Int> RevealedTilesP2 = new List<Vector2Int>();
    //public List<Vector2Int> RevealedTilesP3 = new List<Vector2Int>();
    //public List<Vector2Int> RevealedTilesP4 = new List<Vector2Int>();
    private void Start(){
        _unitManager = FindAnyObjectByType<UnitManager>();
        _hexGrid = FindAnyObjectByType<HexGrid>();
        _proceduralGeneration = FindAnyObjectByType<ProceduralGeneration>();
        _proceduralGeneration.OnMapGenerated.AddListener(InitiateCameraPositions);
    }
    void InitiateCameraPositions(){
        for(int i = 0; i <= 3; i++){
            Vector2Int Cords = _proceduralGeneration.Points[i];
            Vector3 pos = new Vector3(_hexGrid.GetTileFromIntCords(Cords).transform.position.x, _cameraTransform.position.y, _hexGrid.GetTileFromIntCords(Cords).transform.position.z);

            _cameraPositions.Add(pos);
        }
        _cameraTransform.position = _cameraPositions[0];
    }
    public void EndTurn()
    {
        NextTurn.Invoke();
        switch (PlayerTeam)
        {
            case e_Team.Team1:
                PlayerTeam = e_Team.Team2;
                _cameraPositions[0] = _cameraTransform.position;
                ChangeCamera(1);
                ChangeFOW(_unitManager.SO_Players[1].RevealedTiles, _unitManager.SO_Players[0].RevealedTiles);
                //hide all p1 units. 
                //reveal all p2 units
                break;
            case e_Team.Team2:
                PlayerTeam = e_Team.Team3;
                _cameraPositions[1] = _cameraTransform.position;
                ChangeCamera(2);
                ChangeFOW(_unitManager.SO_Players[2].RevealedTiles, _unitManager.SO_Players[1].RevealedTiles);
                //hide all p2 units. 
                //reveal all p3 units
                break;

            case e_Team.Team3:
                PlayerTeam = e_Team.Team4;
                _cameraPositions[2] = _cameraTransform.position;
                ChangeCamera(3);
                ChangeFOW(_unitManager.SO_Players[3].RevealedTiles, _unitManager.SO_Players[2].RevealedTiles);
                //hide all p3 units. 
                //reveal all p4 units
                break;

            case e_Team.Team4:
                PlayerTeam = e_Team.Team1;
                _cameraPositions[3] = _cameraTransform.position;
                ChangeCamera(0);
                ChangeFOW(_unitManager.SO_Players[0].RevealedTiles, _unitManager.SO_Players[3].RevealedTiles);
                //hide all p4 units. 
                //reveal all p1 units
                break;
        }
        _unitManager.SelectedUnit = null;
        _unitManager.HasUnitSelected = false;
    }
    private void ChangeCamera(int team){
        Vector3 cords = _cameraPositions[team];
        _cameraTransform.position = cords;
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
