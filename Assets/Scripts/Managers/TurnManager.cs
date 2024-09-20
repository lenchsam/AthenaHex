using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public Team playerTeam;
    private UnitManager unitManager;
    private HexGrid hexGrid;
    [SerializeField] Transform cameraTransform;
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
                break;
            case Team.Team2:
                playerTeam = Team.Team3;
                ChangeCamera(2);
                break;

            case Team.Team3:
                playerTeam = Team.Team4;
                ChangeCamera(3);
                break;

            case Team.Team4:
                playerTeam = Team.Team1;
                ChangeCamera(0);
                break;
        }
    }
    private void ChangeCamera(int team){
        Vector2Int Cords = unitManager.startPositions[team];
        Vector3 pos = new Vector3(hexGrid.GetTileFromIntCoords(Cords).transform.position.x,cameraTransform.position.y, hexGrid.GetTileFromIntCoords(Cords).transform.position.z);
        cameraTransform.position = pos;
    }
}
