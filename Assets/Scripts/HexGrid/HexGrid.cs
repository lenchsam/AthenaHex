using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
public class HexGrid : MonoBehaviour
{
    [BoxGroup("Assignables")]
    [SerializeField] GameObject TilesParent;
    [BoxGroup("Assignables")]
    [SerializeField] GameObject fogOfWarPrefab;
    [BoxGroup("Map Settings")]
    public int mapWidth;
    [BoxGroup("Map Settings")]
    public int mapHeight;
    [BoxGroup("Map Settings")]
    public bool showFOW;
    [BoxGroup("Map Settings")]
    int tileSize = 1;
    [SerializeField] Dictionary<GameObject, TileScript> Tiles = new Dictionary<GameObject, TileScript>();

    ProceduralGeneration proceduralGeneration;
    
    Vector2 seedOffset;  // Random offset for noise generation

    private async void Start()
    {
        proceduralGeneration = FindAnyObjectByType<ProceduralGeneration>();
        await proceduralGeneration.MakeMapGrid(mapWidth, mapHeight, Tiles, tileSize);
    }
    // Call this function to start generating the map asynchronously

    public TileScript GetTileScript(Vector2 coords){
        foreach(KeyValuePair<GameObject, TileScript> TS in Tiles){
            if(new Vector2(TS.Key.transform.position.x, TS.Key.transform.position.z) == coords){
                return TS.Value;
            }
        }
        return null;
    }
    public void BlockTile(Vector2 coords){
        var tileScript = GetTileScript(coords);
        tileScript.isWalkable = false;
    }
    public void UnblockTile(Vector2 coords){
        var tileScript = GetTileScript(coords);
        tileScript.isWalkable = true;
    }
    public GameObject GetTileFromPosition(Vector2 cords){
        //Debug.Log(cords.ToString() + "  asdf");
        //Debug.Log(TilesParent.transform.Find(cords.ToString()).gameObject);

        return TilesParent.transform.Find(cords.ToString()).gameObject;
    }
    public GameObject GetTileFromIntCoords(Vector2 cords) {
        foreach (Transform child in TilesParent.transform) {
            TileScript tileScript = child.GetComponent<TileScript>();
            if (tileScript != null && tileScript.intCoords == cords) {
                // Debug.Log("Found");
                return child.gameObject;
            }
        }
        // Debug.Log("not found");
        return null;
    }
    public Vector2 GetCoordinatesFromPosition(Vector3 position){
        //Debug.Log(new Vector2(position.x, position.z));
        return new Vector2(position.x, position.z);
    }
    public List<GameObject> GetSurroundingTiles(GameObject tileGO){
        List<GameObject> connecting = new List<GameObject>();
        
        Vector2 tileCords = tileGO.GetComponent<TileScript>().intCoords;

        connecting.Add(GetTileFromIntCoords(tileCords));

        if(tileCords.x % 2 != 0){//if the tile is on an odd row
            //check each possible tile surrounding it to see if its there, if it is there add it to the list
            if(GetTileFromIntCoords(new Vector2(tileCords.x, tileCords.y - 1))){connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x, tileCords.y - 1)));} //Top
            if(GetTileFromIntCoords(new Vector2(tileCords.x, tileCords.y + 1))){connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x, tileCords.y + 1)));} //Bottom

            if(GetTileFromIntCoords(new Vector2(tileCords.x - 1, tileCords.y + 1))){connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x - 1, tileCords.y + 1)));}//Left Top
            if(GetTileFromIntCoords(new Vector2(tileCords.x - 1, tileCords.y))){connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x - 1, tileCords.y)));}//Left Bottom

            if(GetTileFromIntCoords(new Vector2(tileCords.x + 1, tileCords.y + 1))){connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x + 1, tileCords.y + 1)));} //Right Top
            if(GetTileFromIntCoords(new Vector2(tileCords.x + 1, tileCords.y))){connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x + 1, tileCords.y)));} //Right Bottom  
        }else{//if the tile is on an even row
            //check each possible tile surrounding it to see if its there, if it is there add it to the list
            if(GetTileFromIntCoords(new Vector2(tileCords.x, tileCords.y - 1))){connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x, tileCords.y - 1)));} //Top
            if(GetTileFromIntCoords(new Vector2(tileCords.x, tileCords.y + 1))){connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x, tileCords.y + 1)));} //Bottom

            if(GetTileFromIntCoords(new Vector2(tileCords.x - 1, tileCords.y - 1))){connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x - 1, tileCords.y - 1)));} //Left Top
            if(GetTileFromIntCoords(new Vector2(tileCords.x - 1, tileCords.y))){connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x - 1, tileCords.y)));} //Left Bottom

            if(GetTileFromIntCoords(new Vector2(tileCords.x + 1, tileCords.y - 1))){connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x + 1, tileCords.y - 1)));} //Right Top
            if(GetTileFromIntCoords(new Vector2(tileCords.x + 1, tileCords.y))){connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x + 1, tileCords.y)));} //Right Bottom
        }
        //Debug.Log(connecting.Count);

        return connecting;
    }
    public int DistanceBetweenTiles(Vector2Int startCoords, Vector2Int targetCoords) {
        // Convert offset coordinates to cube coordinates
        Vector3Int startCube = OffsetToCube(startCoords);
        Vector3Int targetCube = OffsetToCube(targetCoords);

        // Calculate distance using cube coordinates
        int distance = (Mathf.Abs(startCube.x - targetCube.x) + Mathf.Abs(startCube.y - targetCube.y) + Mathf.Abs(startCube.z - targetCube.z)) / 2;

        //Debug.Log(distance + " THIS IS THE DISTANCE");

        return distance;
    }
    //only needed for DistanceBetweenTiles function
    private Vector3Int OffsetToCube(Vector2Int offsetCoords) {
        int col = offsetCoords.x;
        int row = offsetCoords.y;

        int x = col;
        int z = row - (col - (col & 1)) / 2; // handles the shift in ofset tiles

        //y is the negative of x and z. cube coordinated needs this
        int y = -x - z;

        return new Vector3Int(x, y, z);
    }
    public Vector2Int GetIntCordsFromPosition(Vector2 pos){
        TileScript TS = GetTileScript(pos);
        return TS.intCoords;
    }
    public void AddFogOfWar(TileScript tile){
        GameObject fow = Instantiate(fogOfWarPrefab, transform);
        fow.name = "FOW " + tile.gameObject.name;
        fow.transform.position = tile.transform.position;
        fow.GetComponent<TileScript>().intCoords = tile.intCoords;
        tile.fow = fow;
        tile.gameObject.layer = LayerMask.NameToLayer("Hidden");
    }
    public void RevealTile(TileScript tile){
        tile.Reveal();
        foreach(GameObject neighbour in GetSurroundingTiles(tile.gameObject)){
            neighbour.GetComponent<TileScript>().Reveal();
        }
    }
}
public enum OccupiedBy{
    None,
    wall,
    farm,
    barracks
}
public enum TileType{
    Ocean,
    Grass,
    Coast,
    Mountain
}
