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
                //tileInstScript.coords = new Vector2(hexCoords.x, hexCoords.y);
                tileInstScript.isWalkable = true;
                tileInstScript.intCoords = new Vector2(x, z);

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
    public void blockTile(Vector2 coords){
        var tileScript = GetTileScript(coords);
        tileScript.isWalkable = false;
    }
    public void unblockTile(Vector2 coords){
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
                Debug.Log("Found");
                return child.gameObject;
            }
        }
        Debug.Log("not found");
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

        if(tileCords.x % 2 != 0){
            connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x, tileCords.y - 1))); //Top
            connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x, tileCords.y + 1))); //Bottom

            connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x - 1, tileCords.y + 1)));//Left Top
            connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x - 1, tileCords.y)));//Left Bottom

            connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x + 1, tileCords.y + 1)));//Right Top
            connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x + 1, tileCords.y)));//Right Bottom  
        }else{
            connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x, tileCords.y - 1))); //Top
            connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x, tileCords.y + 1))); //Bottom

            connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x - 1, tileCords.y - 1)));//Left Top
            connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x - 1, tileCords.y)));//Left Bottom

            connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x + 1, tileCords.y - 1)));//Right Top
            connecting.Add(GetTileFromIntCoords(new Vector2(tileCords.x + 1, tileCords.y)));//Right Bottom
        }

        Debug.Log(connecting.Count);



        // List<GameObject> tileList = Tiles.Keys.ToList();
        // int tileIndex = Tiles.Keys.ToList().IndexOf(tileGO);

        //do if x is less than/greater than the lengh of the dictionary
        // if(tileList[tileIndex - 1].transform.position.x == tileGO.transform.position.x){
        //     Debug.Log("HAS TILE BELOW");
        //     connecting.Add(tileList[tileIndex - 1]);
        // }
        // if(tileList[tileIndex + 1].transform.position.x == tileGO.transform.position.x){
        //     Debug.Log("HAS TILE ON TOP");
        //     connecting.Add(tileList[tileIndex + 1]);
        // }
        return connecting;
    }
}
