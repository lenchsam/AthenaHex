using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool isWalkable;
    public GameObject occupiedUnit;
    public Vector2Int intCoords;
    public bool isCityCentre = false;
    public district districts;
    public CitiesScriptableObject SO_Cities;
    public OccupiedBy occupiedBy;
}
