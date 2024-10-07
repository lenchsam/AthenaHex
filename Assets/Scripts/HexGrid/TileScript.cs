using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public UnitManager unitManager;
    private void Awake(){
        turnManager = FindObjectOfType<TurnManager>();
        unitManager = FindObjectOfType<UnitManager>();
    }

    public void Reveal(){
        gameObject.layer = LayerMask.NameToLayer("Tile");

        //if it isnt already in the revealed list, add it to the list
        switch (turnManager.playerTeam)
        {
            case Team.Team1:
                if(!unitManager.SO_Players[0].RevealedTiles.Contains(intCoords)){
                    unitManager.SO_Players[0].RevealedTiles.Add(intCoords);
                }
                break;
            case Team.Team2:
                if(!unitManager.SO_Players[1].RevealedTiles.Contains(intCoords)){
                    unitManager.SO_Players[1].RevealedTiles.Add(intCoords);
                }
                break;

            case Team.Team3:
                if(!unitManager.SO_Players[2].RevealedTiles.Contains(intCoords)){
                    unitManager.SO_Players[2].RevealedTiles.Add(intCoords);
                }
                break;

            case Team.Team4:
                if(!unitManager.SO_Players[3].RevealedTiles.Contains(intCoords)){
                    unitManager.SO_Players[3].RevealedTiles.Add(intCoords);
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
