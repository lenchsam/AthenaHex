using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Events;

public class ProceduralGeneration : MonoBehaviour
{

    //Then i used a flood fill algorithm to assign each tile a biome.
    HexGrid _hexGrid;
    [BoxGroup("Assignables")]
    [SerializeField] GameObject _tilesParent;
    [BoxGroup("Assignables")]
    [SerializeField] Material[] _biomeMaterials;
    [BoxGroup("Assignables/Prefabs")]
    [SerializeField] GameObject _oceanPrefab;
    [BoxGroup("Assignables/Prefabs")]
    [SerializeField] GameObject _grassPrefab;

    //---------------------------------------------------------------------------------------------------POISSON DISC SAMPLING
    [BoxGroup("Poisson Disc Sampling")]
    [Tooltip("area around the poisson disc sample where another sample cant be placed")]
    [SerializeField]int PoissonRadius = 10;
    [HideInInspector] public List<Vector2Int> Points = new List<Vector2Int>();

    //---------------------------------------------------------------------------------------------------Perlin Noise
    //perlin noise

    [BoxGroup("Noise")]
    [SerializeField] float noiseScale = 0.1f;
    [BoxGroup("Noise")]
    [SerializeField] float heightThreshold = 0.5f; // Threshold to decide when it will go to a new layer
    [BoxGroup("Noise")]
    [SerializeField] float oceanThreshold = 0.2f; 
    float lowerLayerHeight = 0;
    float upperLayerHeight = 0.5f;

    Vector2 seedOffset;  // Random offset for noise generation

    //---------------------------------------------------------------------------------------------------
    [HideInInspector] public UnityEvent OnMapGenerated = new UnityEvent();
    void Awake(){
        seedOffset = new Vector2(UnityEngine.Random.Range(0f, 1000f), UnityEngine.Random.Range(0f, 1000f)); //generates a random seed for procedural generation
    }
    void Start(){
        _hexGrid = FindAnyObjectByType<HexGrid>(); 
        Points = poissonDiscSampling(_hexGrid.MapWidth, _hexGrid.MapHeight, PoissonRadius);
        Points = randomisePoints(Points);
    }
    //lists

#region Poisson Disc Sampling
    bool isValidPoint(Vector2Int point, List<Vector2Int> points, int minDistance){
        //gets the tile height
        //checks if it will be an ocean tile,
        //if yes, return false

        //then check if its too close to other tiles by using DistanceBetweenTiles function from the HexGrid class
        
        float TileHeight = GetHeightFromPerlinNoise(point.x, point.y);
        if(getPerlinNoiseHeight(point.x, point.y) < oceanThreshold){
            return false;
        }

        //check if tile is too close to other tiles
        foreach(Vector2Int coord in points){
            int distance = _hexGrid.DistanceBetweenTiles(point, coord);
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
        
        //points = randomisePoints(points);
        return points;
    }
    //done so players and biomes arent always in the same quarter of the map
    List<Vector2Int> randomisePoints(List<Vector2Int> list){
        List<Vector2Int> random = new List<Vector2Int>();
        
        int count = Points.Count;
        for(int i = 0; i <= count - 1; i++){
            var rand = Random.Range(0, list.Count);
            random.Add(list[rand]);
            Points.RemoveAt(rand);

        }
        return random;
    }
#endregion

#region Voronoi
    int Voronoi(Vector2Int tileCords){
        Vector2Int temp = closestPoint(tileCords);
        int pointNumber = getPointNumber(temp);
        return pointNumber;
    }
    int getPointNumber(Vector2Int point){
        if(point == Points[0]){
            return 0;
        }else if(point == Points[1]){
            return 1;
        }else if(point == Points[2]){
            return 2;
        }else {
            return 3;
        }
        
    }
    Vector2Int closestPoint(Vector2Int nextPoint){
        int closestDistance = 999999999; //just a big number so it triggers the if statement below
        Vector2Int closestPoint = new Vector2Int(0, 0);

        //goes through each spawn point, gets the distance to it, and returns the spawn point closest to the tile
        foreach(Vector2Int point in Points){
            int distance = _hexGrid.DistanceBetweenTiles(nextPoint, point);
            if(distance < closestDistance){
                closestDistance = distance;
                closestPoint = point;
            }
        }
        return closestPoint;
    }

#endregion

#region Perlin Noise (height and placing map)
    public async Task MakeMapGrid(int mapWidth, int mapHeight, Dictionary<GameObject, TileScript> Tiles, int tileSize){
        for (int x = 0; x < mapWidth; x++)
        {
            for (int z = 0; z < mapHeight; z++)
            {
                await Task.Yield();

                int biome = Voronoi(new Vector2Int(x, z));//get the biome number
                //calculate height from perlin noise
                Vector2 hexCoords = GetHexCoords(x, z, tileSize);
                float height = GetHeightFromPerlinNoise(x, z);
                Vector3 position = new Vector3(hexCoords.x, height, hexCoords.y);

                GameObject instantiated;
                eTileType tileType;

                // If the tile is an ocean (at the lower layer), instantiate the ocean prefab
                if (height == lowerLayerHeight && getPerlinNoiseHeight(x, z) < oceanThreshold) {
                    instantiated = Instantiate(_oceanPrefab, position, Quaternion.Euler(0, 90, 0), _tilesParent.transform);
                    tileType = eTileType.Ocean;
                } else if (height == upperLayerHeight) {//if its the upper layer
                    instantiated = Instantiate(_grassPrefab, position, Quaternion.Euler(0, 90, 0), _tilesParent.transform);
                    tileType = eTileType.Grass;

                    // Instantiate a base object under the upper layer at the lower layer's height
                    Vector3 basePosition = new Vector3(hexCoords.x, lowerLayerHeight, hexCoords.y);
                    //var baseInst = Instantiate(TopBasePrefab, basePosition, Quaternion.Euler(0, 90, 0), TilesParent.transform);
                    //GameObjectUtility.SetStaticEditorFlags(baseInst, StaticEditorFlags.BatchingStatic);
                }else {//if not ocean, or upper layer, it must be the normal grass layer
                    instantiated = Instantiate(_grassPrefab, position, Quaternion.Euler(0, 90, 0), _tilesParent.transform);
                    tileType = eTileType.Grass;
                    //potentialCoastTiles.Add(instantiated);
                }
                GameObjectUtility.SetStaticEditorFlags(instantiated, StaticEditorFlags.BatchingStatic);

                instantiated.GetComponent<MeshRenderer> ().material = _biomeMaterials[biome];

                var tileInstScript = instantiated.AddComponent<TileScript>();

                tileInstScript.Constructor(true, new Vector2Int(x, z), tileType, (eBiomes) biome);
                
                Tiles.Add(instantiated, instantiated.GetComponent<TileScript>());

                if(_hexGrid.ShowFOW){_hexGrid.AddFogOfWar(tileInstScript);}
            }
        }
        //ConvertGrassToCoastTiles();
        // Fire the UnityEvent once the map is generated
        OnMapGenerated?.Invoke();
        //Debug.Log("RANANNANS");
        StaticBatchingUtility.Combine(_tilesParent);//enables static batching for optimisation
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
#endregion
}
public enum eBiomes{
    Desert,
    Savannah,
    Forest,
    Snow
}
