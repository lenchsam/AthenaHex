using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool isFog = false;
    public bool isWalkable;
    public Vector2Int intCoords;
    public bool isCityCentre = false;
    public district districts;
    public CitiesScriptableObject SO_Cities;
    public OccupiedBy occupiedBy;
    public GameObject occupiedBuilding;
    public GameObject occupiedUnit;
    public GameObject fow;
    public TileType tileType;
    public TurnManager turnManager;
    private void Awake(){
        turnManager = FindObjectOfType<TurnManager>();
    }

    public void Reveal(){
        gameObject.layer = LayerMask.NameToLayer("Tile");

        //if it isnt already in the revealed list, add it to the list
        switch (turnManager.playerTeam)
        {
            case Team.Team1:
                if(!turnManager.RevealedTilesP1.Contains(intCoords)){
                    turnManager.RevealedTilesP1.Add(intCoords);
                }
                break;
            case Team.Team2:
                    if(!turnManager.RevealedTilesP2.Contains(intCoords)){
                    turnManager.RevealedTilesP2.Add(intCoords);
                }
                break;

            case Team.Team3:
                if(!turnManager.RevealedTilesP3.Contains(intCoords)){
                    turnManager.RevealedTilesP3.Add(intCoords);
                }
                break;

            case Team.Team4:
                if(!turnManager.RevealedTilesP4.Contains(intCoords)){
                    turnManager.RevealedTilesP4.Add(intCoords);
                }
                break;
        }
        
        fow.gameObject.SetActive(false);
    }
    public void ReBlock(){
        gameObject.layer = LayerMask.NameToLayer("Hidden");

        fow.gameObject.SetActive(true);
    }
    public void Constructor(bool _isWalkable, Vector2Int _intCords, TileType _tileType){
        isWalkable = _isWalkable;
        intCoords = _intCords;
        tileType = _tileType;
    }
}
