using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitiesManager : MonoBehaviour
{
    public GameObject border;
    HexGrid gridManager;
    TurnManager turnManager;
    public CitiesScriptableObject CitiesSOBase;
    [SerializeField] List<CitiesScriptableObject> AllCities = new List<CitiesScriptableObject>();
    public int numberOfCities;
    void Start(){
        gridManager = FindObjectOfType<HexGrid>();
        turnManager = FindObjectOfType<TurnManager>();
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
        Vector2 tileCords = gridManager.GetCoordinatesFromPosition(positionToInstantiate); //get the tileCords to make the city at
        CitiesScriptableObject CitySO = Instantiate(CitiesSOBase); //create a new scriptable object for the city
        CitySO.constructor(("City: " + numberOfCities).ToString(), numberOfCities, turnManager.playerTeam, tileCords);
        numberOfCities++;
        AllCities.Add(CitySO); //add it to the list of city scriptable objects

        //get tile script, then assign the city centre
        var tileScript = gridManager.GetTileFromPosition(new Vector2(tileCords.x, tileCords.y)).GetComponent<TileScript>();
        tileScript.isCityCentre = true;
        tileScript.districts = district.CityCentre;

        initialiseCity(CitySO, gridManager.GetTileFromPosition(new Vector2(tileCords.x, tileCords.y))); //make the city
    }
}
public enum district{
    None,
    CityCentre,
    Barrack
}
