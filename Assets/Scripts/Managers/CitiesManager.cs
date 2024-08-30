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
        //initialiseCity(SO_Cities, new Vector2Int(1,1));
        gridManager.GetTile(new Vector2Int (0, 3));
    }
    public void expandBorder(Vector2 tileToExpand, CitiesScriptableObject SO_Cities){
        SO_Cities.CityTiles.Add(tileToExpand, gridManager.GetTileScript(tileToExpand));
        //Debug.Log(tileToExpand);
    }
    public void initialiseCity(CitiesScriptableObject SO_Cities, Vector2 CityCentre){
        for(int i = -1; i < 2; i++){
            for(int j = -1; j < 2; j++){
                //Debug.Log(CityCentre - new Vector2Int(i, j));
                expandBorder(CityCentre - new Vector2(i, j), SO_Cities);
                changeTileColour(CityCentre - new Vector2(i, j));//makes the border for the city
            }
        }
    }
    public void changeTileColour(Vector2 tileCords){
        GameObject tile = gridManager.GetTile(tileCords);//get tile

        //change colour
        var rend = tile.transform.GetChild(0).gameObject.GetComponent<MeshRenderer>();
        rend.material.color = Color.black;
    }
}
public enum districts{
    
}
