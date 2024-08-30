using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cities", menuName = "ScriptableObjects/Cities")]
public class CitiesScriptableObject : ScriptableObject
{
    public string cityName;
    public int CityNumber;
    public Dictionary<Vector2, TileScript> CityTiles = new Dictionary<Vector2, TileScript>();
    //public Dictionary<districts, Vector2Int> Districts = new Dictionary<districts, Vector2Int>();
}
