using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private HexGrid hexGrid;
    public int unitMovementSpeed;
    void Start(){
        hexGrid = FindAnyObjectByType<HexGrid>();
    }
    public List<GameObject> FindPath(Vector2Int startCoords, Vector2Int targetCoords)
    {
        // Open list to track nodes to be evaluated
        List<TileScript> openList = new List<TileScript>();

        // Closed list to track nodes already evaluated
        HashSet<TileScript> closedList = new HashSet<TileScript>();

        // Dictionary to store the cost of moving from the start node
        Dictionary<TileScript, int> gCost = new Dictionary<TileScript, int>();
        // Dictionary to store the total estimated cost (gCost + hCost)
        Dictionary<TileScript, int> fCost = new Dictionary<TileScript, int>();
        // Dictionary to store the parent of each tile (used to reconstruct the path)
        Dictionary<TileScript, TileScript> cameFrom = new Dictionary<TileScript, TileScript>();

        TileScript startTile = hexGrid.GetTileFromIntCoords(startCoords).GetComponent<TileScript>();
        TileScript targetTile = hexGrid.GetTileFromIntCoords(targetCoords).GetComponent<TileScript>();

        openList.Add(startTile);
        gCost[startTile] = 0;
        fCost[startTile] = hexGrid.DistanceBetweenTiles(startCoords, targetCoords);

        while (openList.Count > 0)
        {
            // Get the tile in the open list with the lowest fCost
            TileScript currentTile = openList[0];
            foreach (TileScript tile in openList)
            {
                if (fCost[tile] < fCost[currentTile])
                {
                    currentTile = tile;
                }
            }

            // If we've reached the target, reconstruct the path
            if (currentTile == targetTile)
            {
                return ReconstructPath(cameFrom, currentTile);
            }

            // Move current tile from open to closed list
            openList.Remove(currentTile);
            closedList.Add(currentTile);

            // Loop through each neighbor of the current tile
            foreach (GameObject neighborGO in hexGrid.GetSurroundingTiles(currentTile.gameObject))
            {
                TileScript neighbor = neighborGO.GetComponent<TileScript>();

                // Skip this neighbor if it's not walkable or it's already in the closed list
                if (!neighbor.isWalkable || closedList.Contains(neighbor))
                {
                    continue;
                }

                // Calculate the tentative gCost for this neighbor
                int tentativeGCost = gCost[currentTile] + hexGrid.DistanceBetweenTiles(currentTile.intCoords, neighbor.intCoords);

                // If this is a new node or we found a better path, update gCost, fCost and cameFrom
                if (!openList.Contains(neighbor) || tentativeGCost < gCost[neighbor])
                {
                    cameFrom[neighbor] = currentTile;
                    gCost[neighbor] = tentativeGCost;
                    fCost[neighbor] = gCost[neighbor] + hexGrid.DistanceBetweenTiles(neighbor.intCoords, targetCoords);

                    // Add this neighbor to the open list if it's not already there
                    if (!openList.Contains(neighbor))
                    {
                    openList.Add(neighbor);
                    }
                }
            }
        }

        // Return null if no path is found
        return null;
    }
    private List<GameObject> ReconstructPath(Dictionary<TileScript, TileScript> cameFrom, TileScript currentTile)
    {
        List<GameObject> totalPath = new List<GameObject>();
        totalPath.Add(currentTile.gameObject);

        while (cameFrom.ContainsKey(currentTile))
        {
            currentTile = cameFrom[currentTile];
            totalPath.Add(currentTile.gameObject);
        }

        totalPath.Reverse(); // Reverse to get the path from start to goal
        return totalPath;
    }
}
