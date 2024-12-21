using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : MonoBehaviour
{
    [SerializeField] GameObject _objectToInstantiate;
    [SerializeField] bool _isBuilding = false;
    private HexSnap _hexSnap;
    private PlayerController _playerController;
    void Start(){
        _hexSnap = FindAnyObjectByType<HexSnap>();
        _playerController = FindAnyObjectByType<PlayerController>();
    }
    public void PlaceDown(RaycastHit hit){
        if(!_isBuilding){return;}
        var GO = Instantiate(_objectToInstantiate, hit.transform.position, Quaternion.identity);
        GO.transform.rotation = Quaternion.Euler(0, GO.transform.eulerAngles.y + 30, 0);
        
        TileScript tileScript = hit.transform.gameObject.GetComponent<TileScript>();
        tileScript.OccupiedBuilding = GO;
        tileScript.OccupiedBy = eOccupiedBy.wall;
        tileScript.IsWalkable = false;
    }
    public void rotateBuilding(){
        //if the tile
        if(_playerController.SelectedTile.GetComponent<TileScript>().OccupiedBy == eOccupiedBy.None){return;}//if nothing is on the tile do nothing

        //get the gameobject on the tile, then rotate it
        GameObject buildingToRotate = _playerController.SelectedTile.GetComponent<TileScript>().OccupiedBuilding;
        buildingToRotate.transform.rotation = Quaternion.Euler(0, buildingToRotate.transform.eulerAngles.y + 60, 0);
    }
}
