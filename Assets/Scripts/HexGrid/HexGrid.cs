using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [SerializeField] GameObject TilesParent;
    [SerializeField] int mapWidth;
    [SerializeField] int mapHeight;
    [SerializeField] int tileSize = 1;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Dictionary<GameObject, TileScript> Tiles = new Dictionary<GameObject, TileScript>();
    void Start(){
        MakeMapGrid();
    }
    private Vector2 GetHexCoords(int x, int z){
        float xPos = x * tileSize * Mathf.Cos(Mathf.Deg2Rad * 30);
        float zPos = z * tileSize + ((x % 2 == 1) ? tileSize * 0.5f : 0);

        return new Vector2(xPos, zPos);
    }
    void MakeMapGrid(){
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                Vector2 hexCoords = GetHexCoords(x, z);
                //Debug.Log(hexCoords);

                Vector3 position = new Vector3(hexCoords.x, 0, hexCoords.y);
                var instantiated = Instantiate(tilePrefab, position, Quaternion.Euler(0, 90, 0), TilesParent.transform);

                var tileInstScript = instantiated.AddComponent<TileScript>();
                tileInstScript.coords = new Vector2(hexCoords.x, hexCoords.y);
                tileInstScript.isWalkable = true;

                Tiles.Add(instantiated, instantiated.GetComponent<TileScript>());
            }
        }
    }

    public TileScript GetTileScript(Vector2 coords){
        foreach(KeyValuePair<GameObject, TileScript> TS in Tiles){
            if(TS.Value.coords == coords){
                return TS.Value;
            }
        }
        return null;
    }
    public void blockTile(Vector2 coords){
        var tileScript = GetTileScript(coords);
        tileScript.isWalkable = false;
    }
    public void unblockTile(Vector2 coords){
        var tileScript = GetTileScript(coords);
        tileScript.isWalkable = true;
    }
    public GameObject GetTile(Vector2 cords){
        Debug.Log(cords.ToString() + "  asdf");
        //Debug.Log(TilesParent.transform.Find(cords.ToString()).gameObject);

        return TilesParent.transform.Find(cords.ToString()).gameObject;
    }
    public Vector2 GetCoordinatesFromPosition(Vector3 position){
        //Debug.Log(new Vector2(position.x, position.z));
        return new Vector2(position.x, position.z);
    }
    public List<GameObject> GetSurroundingTiles(GameObject tileGO){
        List<GameObject> connecting = new List<GameObject>();
        //left tile = -1
        //right tile = + 1
        //top tile = - map width
        //bottom tile = + map width
        //find tileGO index in the dictionary of tiles

        List<GameObject> tileList = Tiles.Keys.ToList();
        int tileIndex = Tiles.Keys.ToList().IndexOf(tileGO);

        //do if x is less than/greater than the lengh of the dictionary
        if(tileList[tileIndex - 1].transform.position.x == tileGO.transform.position.x){
            Debug.Log("HAS TILE BELOW");
            connecting.Add(tileList[tileIndex - 1]);
        }
        if(tileList[tileIndex + 1].transform.position.x == tileGO.transform.position.x){
            Debug.Log("HAS TILE ON TOP");
            connecting.Add(tileList[tileIndex + 1]);
        }
        return connecting;
    }
}
