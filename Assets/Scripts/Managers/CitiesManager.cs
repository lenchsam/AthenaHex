using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitiesManager : MonoBehaviour
{
    HexGrid _hexGrid;
    TurnManager _turnManager;
    [SerializeField] private CitiesScriptableObject _citiesSOBase;
    [SerializeField] List<CitiesScriptableObject> _allCities = new List<CitiesScriptableObject>();
    [HideInInspector] public int NumberOfCities;
    private Barracks _selectedCity;
    [SerializeField] GameObject _cityCentrePrefab;
    void Start(){
        _hexGrid = FindAnyObjectByType<HexGrid>();
        _turnManager = FindAnyObjectByType<TurnManager>();
    }
    public void expandBorder(GameObject tileToExpand, CitiesScriptableObject SO_Cities){
        //add the tile to the list of tiles inside of the citiesscriptable object
        SO_Cities.CityTiles.Add(tileToExpand);
        tileToExpand.GetComponent<TileScript>().SO_Cities = SO_Cities;
        //Debug.Log(tileToExpand);
    }
    public void initialiseCity(CitiesScriptableObject citiesSO, GameObject CityCentre){
        List<GameObject> tiles = _hexGrid.GetSurroundingTiles(CityCentre); //creates a list of every connecting tile
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
        Vector2 tileCords = _hexGrid.GetCoordinatesFromPosition(positionToInstantiate); //get the tileCords to make the city at
        CitiesScriptableObject CitySO = Instantiate(_citiesSOBase); //create a new scriptable object for the city
        CitySO.constructor(("City: " + NumberOfCities).ToString(), NumberOfCities, _turnManager.PlayerTeam, tileCords);
        NumberOfCities++;
        _allCities.Add(CitySO); //add it to the list of city scriptable objects

        //get tile script, then assign the city centre
        var tileScript = _hexGrid.GetTileScriptFromPosition(new Vector2(tileCords.x, tileCords.y));
        tileScript.IsCityCentre = true;
        tileScript.Districts = eDistrict.CityCentre;
        tileScript.transform.gameObject.AddComponent<CityCentre>();

        Instantiate(_cityCentrePrefab,tileScript.gameObject.transform.position, Quaternion.Euler(0, 90, 0));//instantiate castle.

        initialiseCity(CitySO, _hexGrid.GetTileFromPosition(new Vector2(tileCords.x, tileCords.y))); //make the city
    }
    public CitiesScriptableObject GetCitySOFromTile(GameObject tile){
        if(tile.GetComponent<TileScript>().SO_Cities == null){
            //Debug.Log("returned");
            return null;
        }
        foreach(CitiesScriptableObject SO_Cities in _allCities){
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
