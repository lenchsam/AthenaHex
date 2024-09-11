using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileScript : MonoBehaviour, IInteractable
{
    public bool isWalkable;
    public GameObject occupiedUnit;
    public Vector2Int intCoords;
    public bool isCityCentre = false;
    public district districts;

    public void OnClick()
    {
        Debug.Log("I AM CLICKED + " + gameObject.name);
        if(isWalkable){
            
        }
    }
}
