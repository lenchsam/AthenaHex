using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private HexGrid _hexGrid;
    public int UnitMovementSpeed;
    void Start(){
        _hexGrid = FindAnyObjectByType<HexGrid>();
    }
    public List<GameObject> FindPath(Vector2Int startCoords, Vector2Int targetCoords)
    {
        List<GameObject> path;
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

        TileScript startTile = _hexGrid.GetTileScriptFromIntCords(startCoords);
        TileScript targetTile = _hexGrid.GetTileScriptFromIntCords(targetCoords);

        openList.Add(startTile);
        gCost[startTile] = 0;
        fCost[startTile] = _hexGrid.DistanceBetweenTiles(startCoords, targetCoords);

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
                //Debug.Log("RAN123123123");
                path = ReconstructPath(cameFrom, currentTile);

                // Check if targetTile is already in the path; if not, add it at the end
                if (path[path.Count - 1] != targetTile.gameObject)
                {
                    //Debug.Log("RAN1");
                    path.Add(targetTile.gameObject);
                }

                return path;
            }

            // Move current tile from open to closed list
            openList.Remove(currentTile);
            closedList.Add(currentTile);

            // Loop through each neighbor of the current tile
            foreach (GameObject neighborGO in _hexGrid.GetSurroundingTiles(currentTile.gameObject))
            {
                TileScript neighbor = neighborGO.GetComponent<TileScript>();

                // Skip this neighbor if it's not walkable or it's already in the closed list
                if (!neighbor.IsWalkable || closedList.Contains(neighbor))
                {
                    continue;
                }

                // Calculate the tentative gCost for this neighbor
                int tentativeGCost = gCost[currentTile] + _hexGrid.DistanceBetweenTiles(currentTile.IntCoords, neighbor.IntCoords);

                // If this is a new node or we found a better path, update gCost, fCost and cameFrom
                if (!openList.Contains(neighbor) || tentativeGCost < gCost[neighbor])
                {
                    cameFrom[neighbor] = currentTile;
                    gCost[neighbor] = tentativeGCost;
                    fCost[neighbor] = gCost[neighbor] + _hexGrid.DistanceBetweenTiles(neighbor.IntCoords, targetCoords);

                    // Add this neighbor to the open list if it's not already there
                    if (!openList.Contains(neighbor))
                    {
                    openList.Add(neighbor);
                    }
                }
            }
            path = ReconstructPath(cameFrom, currentTile);

            // Check if targetTile is already in the path; if not, add it at the end
            if (path[path.Count - 1] != targetTile.gameObject)
            {
                //Debug.Log("RAN1");
                path.Add(targetTile.gameObject);
                return path;
            }
        }

        // Return null if no path is found
        Debug.Log("no path found");
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
