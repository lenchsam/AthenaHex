using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public void Reveal(){
        gameObject.layer = LayerMask.NameToLayer("Tile");
        
        fow.gameObject.SetActive(false);
    }
}
