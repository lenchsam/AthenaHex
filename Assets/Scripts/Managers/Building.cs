using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Building : MonoBehaviour
{
    [SerializeField] GameObject objectToInstantiate;
    [SerializeField] bool isBuilding = false;
    private HexSnap hexSnap;
    private PlayerController playerController;
    void Start(){
        hexSnap = FindAnyObjectByType<HexSnap>();
        playerController = FindAnyObjectByType<PlayerController>();
    }
    public void PlaceDown(RaycastHit hit){
        if(!isBuilding){return;}
        var GO = Instantiate(objectToInstantiate, hit.transform.position, Quaternion.identity);
        GO.transform.rotation = Quaternion.Euler(0, GO.transform.eulerAngles.y + 30, 0);
        
        TileScript tileScript = hit.transform.gameObject.GetComponent<TileScript>();
        tileScript.occupiedBuilding = GO;
        tileScript.occupiedBy = eOccupiedBy.wall;
        tileScript.isWalkable = false;
    }
    public void rotateBuilding(){
        //if the tile
        if(playerController.selectedTile.GetComponent<TileScript>().occupiedBy == eOccupiedBy.None){return;}//if nothing is on the tile do nothing

        //get the gameobject on the tile, then rotate it
        GameObject buildingToRotate = playerController.selectedTile.GetComponent<TileScript>().occupiedBuilding;
        buildingToRotate.transform.rotation = Quaternion.Euler(0, buildingToRotate.transform.eulerAngles.y + 60, 0);
    }
}
