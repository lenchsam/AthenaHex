using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool IsFog = false;
    public bool IsWalkable;
    public Vector2Int IntCoords;
    public bool IsCityCentre = false;
    public eDistrict Districts;
    public CitiesScriptableObject SO_Cities;
    public eOccupiedBy OccupiedBy;
    public GameObject OccupiedBuilding;
    public GameObject OccupiedUnit;
    public GameObject Fow;
    public eTileType TileType;
    private TurnManager _turnManager;
    private UnitManager _unitManager;
    public eBiomes Biome;
    private void Awake(){
        _turnManager = FindAnyObjectByType<TurnManager>();
        _unitManager = FindAnyObjectByType<UnitManager>();
    }

    public void Reveal(){
        gameObject.layer = LayerMask.NameToLayer("Tile");

        //if it isnt already in the revealed list, add it to the list
        switch (_turnManager.PlayerTeam)
        {
            case e_Team.Team1:
                if(!_unitManager.SO_Players[0].RevealedTiles.Contains(IntCoords)){
                    _unitManager.SO_Players[0].RevealedTiles.Add(IntCoords);
                }
                break;
            case e_Team.Team2:
                if(!_unitManager.SO_Players[1].RevealedTiles.Contains(IntCoords)){
                    _unitManager.SO_Players[1].RevealedTiles.Add(IntCoords);
                }
                break;

            case e_Team.Team3:
                if(!_unitManager.SO_Players[2].RevealedTiles.Contains(IntCoords)){
                    _unitManager.SO_Players[2].RevealedTiles.Add(IntCoords);
                }
                break;

            case e_Team.Team4:
                if(!_unitManager.SO_Players[3].RevealedTiles.Contains(IntCoords)){
                    _unitManager.SO_Players[3].RevealedTiles.Add(IntCoords);
                }
                break;
        }
        
        Fow.gameObject.SetActive(false);
        if(OccupiedUnit){OccupiedUnit.SetActive(true);}
    }
    public void ReBlock(){
        gameObject.layer = LayerMask.NameToLayer("Hidden");

        Fow.gameObject.SetActive(true);
        if(OccupiedUnit){OccupiedUnit.SetActive(false);}
    }
    public void Constructor(bool isWalkable, Vector2Int intCords, eTileType tileType, eBiomes biome){
        IsWalkable = isWalkable;
        IntCoords = intCords;
        TileType = tileType;
        Biome = biome;
        if(TileType == eTileType.Ocean){
            IsWalkable = false;
        }
    }
}
