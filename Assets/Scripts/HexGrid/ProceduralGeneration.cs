using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

public class ProceduralGeneration : MonoBehaviour
{
    //This script uses a modified version of poisson disc sampling. I split the map into 4 equal parts, i picked a random point in that section and added it to a list. 
    //I then chose a random point in the other sections and made sure that it wasnt too close to all other points. its essensially how poisson disc sampling works, just modified a bit to suit my game more.
    //the cell size in the grid doesnt need to be accurate to the real one used in the game as only the coordinates will be used outside of this algorithm.

    //Then i used a flood fill algorithm to assign each tile a biome.

    [BoxGroup("Poisson Disc Sampling")]
    [Tooltip("area around the poisson disc sample where another sample cant be placed")]
    [SerializeField]int PoissonRadius = 10;
    public List<Vector2Int> points = new List<Vector2Int>();

    HexGrid hexGrid;
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
        
        float TileHeight = hexGrid.GetHeightFromPerlinNoise(point.x, point.y);
        if(hexGrid.getPerlinNoiseHeight(point.x, point.y) < hexGrid.oceanThreshold){
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
}
