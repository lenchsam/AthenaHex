using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexSnap : MonoBehaviour
{
    private float _tileSize = 1f;
    [SerializeField] private LayerMask _hexLayerMask;

    private GameObject _snappedHex;
    private int _currentEdgeIndex = 0; // Keep track of which edge the object is snapped to

    public void SnapToEdge(GameObject goToSnap)
    {
        // Raycast to detect which hex tile was clicked
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, _hexLayerMask))
        {
            _snappedHex = hit.collider.gameObject;
            Vector3 hitPosition = hit.point;

            // Get the hex tile's center position
            Vector3 hexCenter = _snappedHex.transform.position;

            // Calculate the closest edge to the hit point
            Vector3 snappedPosition = FindClosestEdge(hexCenter, hitPosition);

            // Snap the object to that edge
            goToSnap.transform.position = snappedPosition;

            // Set the initial edge index (the one closest to the clicked position)
            _currentEdgeIndex = GetClosestEdgeIndex(hexCenter, hitPosition);

        }
    }
    private Vector3 FindClosestEdge(Vector3 hexCenter, Vector3 hitPosition)
    {
        Vector3[] vertices = GetHexVertices(hexCenter);

        // Find the closest edge by comparing distances to the middle of each edge
        Vector3 closestEdgeMidpoint = Vector3.zero;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < 6; i++)
        {
            Vector3 edgeMidpoint = (vertices[i] + vertices[(i + 1) % 6]) / 2;
            float distance = Vector3.Distance(hitPosition, edgeMidpoint);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEdgeMidpoint = edgeMidpoint;
            }
        }

        return closestEdgeMidpoint;
    }
    private int GetClosestEdgeIndex(Vector3 hexCenter, Vector3 hitPosition)
    {
        Vector3[] vertices = GetHexVertices(hexCenter);
        float closestDistance = Mathf.Infinity;
        int closestEdgeIndex = 0;

        for (int i = 0; i < 6; i++)
        {
            Vector3 edgeMidpoint = (vertices[i] + vertices[(i + 1) % 6]) / 2;
            float distance = Vector3.Distance(hitPosition, edgeMidpoint);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEdgeIndex = i;
            }
        }

        return closestEdgeIndex;
    }
    private Vector3[] GetHexVertices(Vector3 hexCenter)
    {
        float hexRadius = _tileSize;
        Vector3[] vertices = new Vector3[6];

        for (int i = 0; i < 6; i++)
        {
            float angle = Mathf.PI / 3 * i; // 60 degrees in radians
            vertices[i] = new Vector3(
                hexCenter.x + hexRadius * Mathf.Cos(angle),
                hexCenter.y,
                hexCenter.z + hexRadius * Mathf.Sin(angle)
            );
        }

        return vertices;
    }
    private void RotateToNextEdge(GameObject goToRotate)
    {
        //goToRotate.transform.rotation = Quaternion.Euler(0, goToRotate.transform.rotation.y + 60, 0);
    }
}
