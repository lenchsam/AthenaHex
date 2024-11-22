using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Events;

public class ProceduralGeneration : MonoBehaviour
{
    //This script uses a modified version of poisson disc sampling. I split the map into 4 equal parts, i picked a random point in that section and added it to a list. 
    //I then chose a random point in the other sections and made sure that it wasnt too close to all other points. its essensially how poisson disc sampling works, just modified a bit to suit my game more.
    //the cell size in the grid doesnt need to be accurate to the real one used in the game as only the coordinates will be used outside of this algorithm.

    //Then i used a flood fill algorithm to assign each tile a biome.
    HexGrid hexGrid;
    [BoxGroup("Assignables")]
    [SerializeField] GameObject TilesParent;
    [BoxGroup("Assignables/Prefabs")]
    [SerializeField] GameObject OceanPrefab;
    [BoxGroup("Assignables/Prefabs")]
    [SerializeField] GameObject GrassPrefab;

    //---------------------------------------------------------------------------------------------------POISSON DISC SAMPLING
    [BoxGroup("Poisson Disc Sampling")]
    [Tooltip("area around the poisson disc sample where another sample cant be placed")]
    [SerializeField]int PoissonRadius = 10;
    public List<Vector2Int> points = new List<Vector2Int>();

    //---------------------------------------------------------------------------------------------------Perlin Noise
    //perlin noise
    [BoxGroup("Procedural Generation/Noise")]
    [SerializeField] float noiseScale = 0.1f;
    [BoxGroup("Procedural Generation/Noise")]
    [SerializeField] float heightThreshold = 0.5f; // Threshold to decide when it will go to a new layer
    [BoxGroup("Procedural Generation/Noise")]
    [SerializeField] float oceanThreshold = 0.2f; 
    float lowerLayerHeight = 0;
    float upperLayerHeight = 0.5f;

    [BoxGroup("Procedural Generation")]
    [SerializeField] Material[] biomeMaterials;
    Vector2 seedOffset;  // Random offset for noise generation
    public List<GameObject> potentialCoastTiles = new List<GameObject>();
    [HideInInspector] public UnityEvent OnMapGenerated = new UnityEvent();
    void Awake(){
        seedOffset = new Vector2(UnityEngine.Random.Range(0f, 1000f), UnityEngine.Random.Range(0f, 1000f)); //generates a random seed for procedural generation
    }
    void Start(){
        hexGrid = FindAnyObjectByType<HexGrid>(); 
        points = poissonDiscSampling(hexGrid.mapWidth, hexGrid.mapHeight, PoissonRadius);
    }
    //lists

    //---------------------------------------------------------------------------------------------------POISSON DISC SAMPLING
    bool isValidPoint(Vector2Int point, List<Vector2Int> points, int minDistance){
        //gets the tile height
        //checks if it will be an ocean tile,
        //if yes, return false

        //then check if its too close to other tiles by using DistanceBetweenTiles function from the HexGrid class
        
        float TileHeight = GetHeightFromPerlinNoise(point.x, point.y);
        if(getPerlinNoiseHeight(point.x, point.y) < oceanThreshold){
            Debug.Log("IS OCEAN");
            return false;
        }

        //check if tile is too close to other tiles
        foreach(Vector2Int coord in points){
            int distance = hexGrid.DistanceBetweenTiles(point, coord);
            if(distance < minDistance){
                return false;
            }
        }
        //if is walkable and not too close to other tiles then return true, else return false
        return true;
    }
    List<Vector2Int> poissonDiscSampling(int width, int height, int minDistance){
        //map is split into quarters
        //random point is chosen in one of the quarters
        //check if its too close to other points
        //if no add to list
        //if yes, choose another random point
        //repeat until each quarter has a random point chosen.

        //this can be made more efficient, but with the game only requiring 4 points from this algorithm, it isn't needed

        
        List<RectInt> quarters = new List<RectInt>();
        //rect = first 2 numbers are bottom left position coordinates. last 2 numbers are width and height of rectangle. width and height of rectangle is always / 2 cuz we split it into quarters

        quarters.Add(new RectInt(0, height / 2 ,width / 2 ,height / 2));//top left
        quarters.Add(new RectInt(width/2, height / 2, width / 2,height / 2));//top right
        quarters.Add(new RectInt(0, 0, width / 2, height / 2));//bottom left
        quarters.Add(new RectInt(width / 2, 0, width / 2,height / 2));//bottom right

        List<Vector2Int> points = new List<Vector2Int>();
        bool valid = false;

        Vector2Int testingPoint = new Vector2Int();

        //points.Add(new Vector2Int(Random.Range(quarters[0].xMin, quarters[0].xMax), Random.Range(quarters[0].yMin, quarters[0].yMax))); //pick the first random point

        while(points.Count < 4){
            foreach(RectInt quarter in quarters){
                while(!valid){
                    testingPoint = new Vector2Int(Random.Range(quarter.xMin, quarter.xMax), Random.Range(quarter.yMin, quarter.yMax));
                    valid = isValidPoint(testingPoint, points, minDistance);
                }
                points.Add(testingPoint);//if its gotten here, then it must have found a valid point, so add to the list

                valid = false;//reset valid variable
            }
            break;
        }
        return points;
    }
    
    //---------------------------------------------------------------------------------------------------Perlin Noise
    public async Task MakeMapGrid(int mapWidth, int mapHeight, Dictionary<GameObject, TileScript> Tiles, int tileSize){
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                await Task.Yield();

                //calculate height from perlin noise
                Vector2 hexCoords = GetHexCoords(x, z, tileSize);
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
    private Vector2 GetHexCoords(int x, int z, int tileSize){
        float xPos = x * tileSize * Mathf.Cos(Mathf.Deg2Rad * 30);
        float zPos = z * tileSize + ((x % 2 == 1) ? tileSize * 0.5f : 0);

        return new Vector2(xPos, zPos);
    }
    public float getPerlinNoiseHeight(int x, int z){
        return Mathf.PerlinNoise((x + seedOffset.x) * noiseScale, (z + seedOffset.y) * noiseScale);
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
}
