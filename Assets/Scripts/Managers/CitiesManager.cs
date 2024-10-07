using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitiesManager : MonoBehaviour
{
    HexGrid gridManager;
    TurnManager turnManager;
    private CitiesScriptableObject CitiesSOBase;
    [SerializeField] List<CitiesScriptableObject> AllCities = new List<CitiesScriptableObject>();
    [HideInInspector] public int numberOfCities;
    private Barracks selectedCity;
    [SerializeField] GameObject CityCentrePrefab;
    void Start(){
        gridManager = FindObjectOfType<HexGrid>();
        turnManager = FindObjectOfType<TurnManager>();
    }
    public void expandBorder(GameObject tileToExpand, CitiesScriptableObject SO_Cities){
        //add the tile to the list of tiles inside of the citiesscriptable object
        SO_Cities.CityTiles.Add(tileToExpand);
        tileToExpand.GetComponent<TileScript>().SO_Cities = SO_Cities;
        //Debug.Log(tileToExpand);
    }
    public void initialiseCity(CitiesScriptableObject citiesSO, GameObject CityCentre){
        List<GameObject> tiles = gridManager.GetSurroundingTiles(CityCentre); //creates a list of every connecting tile
        //Debug.Log(tiles.Count);

        //loop through the list and add them to the city scriptable object
        foreach(GameObject GO in tiles){
            //Debug.Log("asdfasdf");
            expandBorder(GO, citiesSO);
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
        tileScript.transform.gameObject.AddComponent<CityCentre>();

        Instantiate(CityCentrePrefab,tileScript.gameObject.transform.position, Quaternion.Euler(0, 90, 0));//instantiate castle.

        initialiseCity(CitySO, gridManager.GetTileFromPosition(new Vector2(tileCords.x, tileCords.y))); //make the city
    }
    public CitiesScriptableObject GetCitySOFromTile(GameObject tile){
        if(tile.GetComponent<TileScript>().SO_Cities == null){
            //Debug.Log("returned");
            return null;
        }
        foreach(CitiesScriptableObject SO_Cities in AllCities){
            //Debug.Log("next SO");
            foreach(GameObject CityTiles in SO_Cities.CityTiles){
                //Debug.Log("next tile");
                if (CityTiles == tile){
                    return SO_Cities;
                }
            }
        }
        return null;
    }
}
