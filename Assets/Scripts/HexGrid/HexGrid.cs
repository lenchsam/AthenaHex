using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
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
                Debug.Log(hexCoords);

                Vector3 position = new Vector3(hexCoords.x, 0, hexCoords.y);
                var instantiated = Instantiate(tilePrefab, position, Quaternion.Euler(0, 90, 0));

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
    
}
