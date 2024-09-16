using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [SerializeField] GameObject TilesParent;
    [SerializeField] int mapWidth;
    [SerializeField] int mapHeight;
    [SerializeField] int tileSize = 1;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Dictionary<GameObject, TileScript> Tiles = new Dictionary<GameObject, TileScript>();
    void Awake(){
        MakeMapGrid();
    }
    private Vector2 GetHexCoords(int x, int z){
        float xPos = x * tileSize * Mathf.Cos(Mathf.Deg2Rad * 30);
        float zPos = z * tileSize + ((x % 2 == 1) ? tileSize * 0.5f : 0);

        return new Vector2(xPos, zPos);
    }
    private void MakeMapGrid(){
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                Vector2 hexCoords = GetHexCoords(x, z);
                //Debug.Log(hexCoords);

                Vector3 position = new Vector3(hexCoords.x, 0, hexCoords.y);
                var instantiated = Instantiate(tilePrefab, position, Quaternion.Euler(0, 90, 0), TilesParent.transform);

                var tileInstScript = instantiated.AddComponent<TileScript>();
                //tileInstScript.coords = new Vector2(hexCoords.x, hexCoords.y);
                tileInstScript.isWalkable = true;
                tileInstScript.intCoords = new Vector2Int(x, z);

                Tiles.Add(instantiated, instantiated.GetComponent<TileScript>());
            }
        }
    }
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
    public GameObject GetTileFromIntCoords(Vector2 cords){
        foreach(Transform child in TilesParent.transform){
            if(child.GetComponent<TileScript>().intCoords == cords){
                //Debug.Log("Found");
                return child.gameObject;
            }
        }
        //Debug.Log("not found");
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
}

public enum OccupiedBy{
    None,
    wall,
    farm,
    barracks
}
