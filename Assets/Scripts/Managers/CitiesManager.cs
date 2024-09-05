using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitiesManager : MonoBehaviour
{
    public GameObject border;
    HexGrid gridManager;
    public CitiesScriptableObject CitiesSOBase;
    [SerializeField] List<CitiesScriptableObject> AllCities = new List<CitiesScriptableObject>();
    public int numberOfCities;
    void Start(){
        gridManager = FindObjectOfType<HexGrid>();
    }
    public void expandBorder(GameObject tileToExpand, CitiesScriptableObject SO_Cities){
        //add the tile to the list of tiles inside of the citiesscriptable object
        SO_Cities.CityTiles.Add(tileToExpand);


        //Debug.Log(tileToExpand);
    }
    public void initialiseCity(CitiesScriptableObject citiesCO, GameObject CityCentre){
        List<GameObject> tiles = gridManager.GetSurroundingTiles(CityCentre); //creates a list of every connecting tile
        //Debug.Log(tiles.Count);

        //loop through the list and add them to the city scriptable object
        foreach(GameObject GO in tiles){
            //Debug.Log("asdfasdf");
            expandBorder(GO, citiesCO);
            changeTileColour(GO);
        }
    }
    public void changeTileColour(GameObject tile){
        //Debug.Log("changed colour");

        //change colour of the tile. It's for testing
        var rend = tile.GetComponent<MeshRenderer>();
        rend.material.color = Color.black;
    }
    public void MakeNewCity(Vector3 positionToInstantiate){
        CitiesScriptableObject CitySO = Instantiate(CitiesSOBase); //create a new scriptable object for the city
        AllCities.Add(CitySO); //add it to the list of city scriptable objects

        Vector2 tileCords = gridManager.GetCoordinatesFromPosition(positionToInstantiate); //get the tileCords to make the city at
        initialiseCity(CitySO, gridManager.GetTileFromPosition(new Vector2(tileCords.x, tileCords.y))); //make the city
    }
}
public enum districts{
    
}
