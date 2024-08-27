using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [SerializeField] int mapWidth;
    [SerializeField] int mapHeight;
    [SerializeField] int tileSize = 1;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] List<GameObject> Tiles = new List<GameObject>();
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

                Vector3 position = new Vector3(hexCoords.x, 0, hexCoords.y);
                Tiles.Add(Instantiate(tilePrefab, position, Quaternion.Euler(0, 90, 0)));
            }
        }
    }
}
