using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using UnityEngine.Events;
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
    [SerializeField] int tileSize = 1;
    [BoxGroup("Map Settings")]
    public bool showFOW;
    [SerializeField] Dictionary<GameObject, TileScript> Tiles = new Dictionary<GameObject, TileScript>();

    //perlin noise
    [BoxGroup("Procedural Generation/Noise")]
    [SerializeField] float noiseScale = 0.1f;
    [BoxGroup("Procedural Generation/Noise")]
    [SerializeField] float heightThreshold = 0.5f; // Threshold to decide when it will go to a new layer
    [BoxGroup("Procedural Generation/Noise")]
    public  float oceanThreshold = 0.2f; 
    float lowerLayerHeight = 0;
    float upperLayerHeight = 0.5f;
    [BoxGroup("Procedural Generation/Prefabs")]
    [SerializeField] GameObject GrassPrefab;
    [BoxGroup("Procedural Generation/Prefabs")]
    [SerializeField] GameObject OceanPrefab;
    [BoxGroup("Procedural Generation/Prefabs")]
    [SerializeField] GameObject TopBasePrefab;
    [BoxGroup("Procedural Generation/Prefabs")]
    [SerializeField] GameObject[] CoastalPrefabs;
    [BoxGroup("Procedural Generation")]
    [SerializeField] Material[] biomeMaterials;

    public List<GameObject> potentialCoastTiles = new List<GameObject>();
    
    Vector2 seedOffset;  // Random offset for noise generation
    [HideInInspector] public UnityEvent OnMapGenerated = new UnityEvent();
    void Awake(){
        seedOffset = new Vector2(UnityEngine.Random.Range(0f, 1000f), UnityEngine.Random.Range(0f, 1000f)); //generates a random seed for procedural generation
    }
    private async void Start()
    {
        await MakeMapGrid();
    }
    // Call this function to start generating the map asynchronously
    private Vector2 GetHexCoords(int x, int z){
        float xPos = x * tileSize * Mathf.Cos(Mathf.Deg2Rad * 30);
        float zPos = z * tileSize + ((x % 2 == 1) ? tileSize * 0.5f : 0);

        return new Vector2(xPos, zPos);
    }
    private async Task MakeMapGrid(){
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                await Task.Yield();

                //calculate height from perlin noise
                Vector2 hexCoords = GetHexCoords(x, z);
                float height = GetHeightFromPerlinNoise(x, z);
                Vector3 position = new Vector3(hexCoords.x, height, hexCoords.y);

                GameObject instantiated;
                TileType tileType;

                // If the tile is an ocean (at the lower layer), instantiate the ocean prefab
                if (height == lowerLayerHeight && getPerlinNoiseHeight(x, z) < oceanThreshold) {
                    instantiated = Instantiate(OceanPrefab, position, Quaternion.Euler(0, 90, 0), TilesParent.transform);
                    tileType = TileType.Ocean;
                } else if (height == upperLayerHeight) {//if its the upper layer
                    instantiated = Instantiate(GrassPrefab, position, Quaternion.Euler(0, 90, 0), TilesParent.transform);
                    tileType = TileType.Grass;

                    // Instantiate a base object under the upper layer at the lower layer's height
                    Vector3 basePosition = new Vector3(hexCoords.x, lowerLayerHeight, hexCoords.y);
                    //var baseInst = Instantiate(TopBasePrefab, basePosition, Quaternion.Euler(0, 90, 0), TilesParent.transform);
                    //GameObjectUtility.SetStaticEditorFlags(baseInst, StaticEditorFlags.BatchingStatic);
                }else {//if not ocean, or upper layer, it must be the normal grass layer
                    instantiated = Instantiate(GrassPrefab, position, Quaternion.Euler(0, 90, 0), TilesParent.transform);
                    tileType = TileType.Grass;
                    //potentialCoastTiles.Add(instantiated);
                }
                GameObjectUtility.SetStaticEditorFlags(instantiated, StaticEditorFlags.BatchingStatic);

                var tileInstScript = instantiated.AddComponent<TileScript>();

                tileInstScript.Constructor(true, new Vector2Int(x, z), tileType);

                Tiles.Add(instantiated, instantiated.GetComponent<TileScript>());

                //if(showFOW){AddFogOfWar(tileInstScript);}
            }
        }
        //ConvertGrassToCoastTiles();
        // Fire the UnityEvent once the map is generated
        OnMapGenerated?.Invoke();

        //Debug.Log("RANANNANS");
        StaticBatchingUtility.Combine(TilesParent);//enables static batching for optimisation
        return;
    }
    public float getPerlinNoiseHeight(int x, int z){
        return Mathf.PerlinNoise((x + seedOffset.x) * noiseScale, (z + seedOffset.y) * noiseScale);
    }
    private int CountOceanNeighbors(GameObject tileGO) {
        int oceanNeighbors = 0;
    
        // Get surrounding tiles
        List<GameObject> surroundingTiles = GetSurroundingTiles(tileGO);

        // Loop through the surrounding tiles and count how many are ocean
        foreach (GameObject neighbor in surroundingTiles) {
            if (neighbor != null) {
                TileScript tileScript = neighbor.GetComponent<TileScript>();

                if (tileScript != null && tileScript.tileType == TileType.Ocean) {
                    oceanNeighbors++;
                }
            }
        }

        return oceanNeighbors;
    }
    private void ConvertGrassToCoastTiles() {
            for(int i = 0; i < potentialCoastTiles.Count; i++){
                var tile = potentialCoastTiles[i];
                int oceanNeighbors = CountOceanNeighbors(tile);
                // If the tile has water neighbors, convert it to a coast tile
                if (oceanNeighbors > 0 && oceanNeighbors < 6) {
                    Vector3 position = tile.transform.position;
                    var gameObject = Instantiate(GetCoastalPrefab(oceanNeighbors), position, Quaternion.Euler(0, 90, 0), TilesParent.transform);
                    gameObject.AddComponent<TileScript>();
                    gameObject.GetComponent<TileScript>().Constructor(true, GetIntCordsFromPosition(new Vector2(position.x, position.z)), TileType.Coast);
                    //potentialCoastTiles.Remove(tile);
                    Tiles.Remove(tile);
                    Destroy(tile); // Remove the grass tile
                    Tiles.Add(gameObject, gameObject.GetComponent<TileScript>());
                }
            }
    
        // Clear the list after conversion
        potentialCoastTiles.Clear();
    }
    private GameObject GetCoastalPrefab(int waterNeighbors) {
        // Return the appropriate coastal prefab based on the number of water neighbors
        // Customize this based on your coastal prefab setup (e.g., coastalPrefabs[0] for 1 water, coastalPrefabs[1] for 2 water, etc.)
        return CoastalPrefabs[Mathf.Clamp(waterNeighbors - 1, 0, CoastalPrefabs.Length - 1)];
    }
    public float GetHeightFromPerlinNoise(int x, int z) {
        float noiseValue = Mathf.PerlinNoise((x + seedOffset.x) * noiseScale, (z + seedOffset.y) * noiseScale);
        
        // Check for ocean first (if the noise is very low, it becomes an ocean)
        if (noiseValue < oceanThreshold) {
            return lowerLayerHeight;
        }

        // Use the threshold to decide between two layers
        return noiseValue > heightThreshold ? upperLayerHeight : lowerLayerHeight;
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
