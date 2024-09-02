using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitiesManager : MonoBehaviour
{
public GameObject border;
    HexGrid gridManager;
    [SerializeField] CitiesScriptableObject SO_Cities;
    public int numberOfCities;
    void Start(){
        gridManager = FindObjectOfType<HexGrid>();
    }
    public void expandBorder(GameObject tileToExpand, CitiesScriptableObject SO_Cities){
        SO_Cities.CityTiles.Add(tileToExpand);
        //Debug.Log(tileToExpand);
    }
    public void initialiseCity(CitiesScriptableObject citiesCO, GameObject CityCentre){
        List<GameObject> tiles = gridManager.GetSurroundingTiles(CityCentre);
        Debug.Log(tiles.Count);

        foreach(GameObject GO in tiles){
            //Debug.Log("asdfasdf");
            expandBorder(GO, citiesCO);
            changeTileColour(GO);//makes the border for the city
        }
    }
    public void changeTileColour(GameObject tile){
        //Debug.Log("changed colour");
        //change colour
        var rend = tile.GetComponent<MeshRenderer>();
        rend.material.color = Color.black;
    }
}
public enum districts{
    
}
