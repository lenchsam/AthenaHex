using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
//https://www.redblobgames.com/grids/hexagons USEFUL RESOURCE FOR ALL THINGS HEX GRID
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
    public TileScript GetTileScriptFromPosition(Vector2 cords){
        foreach(KeyValuePair<GameObject, TileScript> TS in Tiles){
            if(new Vector2(TS.Key.transform.position.x, TS.Key.transform.position.z) == cords){
                return TS.Value;
            }
        }
        return null;
    }
    public TileScript GetTileScriptFromIntCords(Vector2Int cords){
        foreach(KeyValuePair<GameObject, TileScript> TS in Tiles){
            if(TS.Value.intCoords == cords){
                return TS.Value;
            }
        }
        return null;
    }
    public void BlockTile(Vector2 coords){
        var tileScript = GetTileScriptFromPosition(coords);
        tileScript.isWalkable = false;
    }
    public void UnblockTile(Vector2 coords){
        var tileScript = GetTileScriptFromPosition(coords);
        tileScript.isWalkable = true;
    }
    public GameObject GetTileFromPosition(Vector2 cords){
        foreach(KeyValuePair<GameObject, TileScript> TS in Tiles){
            if(new Vector2(TS.Key.transform.position.x, TS.Key.transform.position.z) == cords){
                return TS.Key;
            }
        }
        return null;;
    }
    public GameObject GetTileFromIntCords(Vector2Int cords) {
        foreach(KeyValuePair<GameObject, TileScript> TS in Tiles){
            if(TS.Value.intCoords == cords){
                return TS.Key;
            }
        }
        return null;
    }
    public Vector2 GetCoordinatesFromPosition(Vector3 position){
        //Debug.Log(new Vector2(position.x, position.z));
        return new Vector2(position.x, position.z);
    }
    public List<GameObject> GetSurroundingTiles(GameObject tileGO){
        List<GameObject> connecting = new List<GameObject>();
        
        Vector2Int tileCords = tileGO.GetComponent<TileScript>().intCoords;

        connecting.Add(GetTileFromIntCords(tileCords));

        if(tileCords.x % 2 != 0){//if the tile is on an odd row
            //check each possible tile surrounding it to see if its there, if it is there add it to the list
            if(GetTileFromIntCords(new Vector2Int(tileCords.x, tileCords.y - 1))){connecting.Add(GetTileFromIntCords(new Vector2Int(tileCords.x, tileCords.y - 1)));} //Top
            if(GetTileFromIntCords(new Vector2Int(tileCords.x, tileCords.y + 1))){connecting.Add(GetTileFromIntCords(new Vector2Int(tileCords.x, tileCords.y + 1)));} //Bottom

            if(GetTileFromIntCords(new Vector2Int(tileCords.x - 1, tileCords.y + 1))){connecting.Add(GetTileFromIntCords(new Vector2Int(tileCords.x - 1, tileCords.y + 1)));}//Left Top
            if(GetTileFromIntCords(new Vector2Int(tileCords.x - 1, tileCords.y))){connecting.Add(GetTileFromIntCords(new Vector2Int(tileCords.x - 1, tileCords.y)));}//Left Bottom

            if(GetTileFromIntCords(new Vector2Int(tileCords.x + 1, tileCords.y + 1))){connecting.Add(GetTileFromIntCords(new Vector2Int(tileCords.x + 1, tileCords.y + 1)));} //Right Top
            if(GetTileFromIntCords(new Vector2Int(tileCords.x + 1, tileCords.y))){connecting.Add(GetTileFromIntCords(new Vector2Int(tileCords.x + 1, tileCords.y)));} //Right Bottom  
        }else{//if the tile is on an even row
            //check each possible tile surrounding it to see if its there, if it is there add it to the list
            if(GetTileFromIntCords(new Vector2Int(tileCords.x, tileCords.y - 1))){connecting.Add(GetTileFromIntCords(new Vector2Int(tileCords.x, tileCords.y - 1)));} //Top
            if(GetTileFromIntCords(new Vector2Int(tileCords.x, tileCords.y + 1))){connecting.Add(GetTileFromIntCords(new Vector2Int(tileCords.x, tileCords.y + 1)));} //Bottom

            if(GetTileFromIntCords(new Vector2Int(tileCords.x - 1, tileCords.y - 1))){connecting.Add(GetTileFromIntCords(new Vector2Int(tileCords.x - 1, tileCords.y - 1)));} //Left Top
            if(GetTileFromIntCords(new Vector2Int(tileCords.x - 1, tileCords.y))){connecting.Add(GetTileFromIntCords(new Vector2Int(tileCords.x - 1, tileCords.y)));} //Left Bottom

            if(GetTileFromIntCords(new Vector2Int(tileCords.x + 1, tileCords.y - 1))){connecting.Add(GetTileFromIntCords(new Vector2Int(tileCords.x + 1, tileCords.y - 1)));} //Right Top
            if(GetTileFromIntCords(new Vector2Int(tileCords.x + 1, tileCords.y))){connecting.Add(GetTileFromIntCords(new Vector2Int(tileCords.x + 1, tileCords.y)));} //Right Bottom
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
        TileScript TS = GetTileScriptFromPosition(pos);
        return TS.intCoords;
    }
    public void AddFogOfWar(TileScript tile){
        GameObject fow = Instantiate(fogOfWarPrefab, transform);
        fow.name = "FOW " + tile.gameObject.name;
        fow.transform.position = new Vector3(tile.transform.position.x, 0, tile.transform.position.z);
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
public enum eOccupiedBy{
    None,
    wall,
    farm,
    barracks
}
public enum eTileType{
    Ocean,
    Grass,
    Coast,
    Mountain
}
