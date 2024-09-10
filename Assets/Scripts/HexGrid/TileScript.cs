using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TileScript : MonoBehaviour, IInteractable
{
    public bool isWalkable;
    public GameObject occupiedUnit;
    //public Vector2 coords;
    public Vector2Int intCoords;

    public void OnClick()
    {
        Debug.Log("I AM CLICKED + " + gameObject.name);
        if(isWalkable){

        }
    }
}
