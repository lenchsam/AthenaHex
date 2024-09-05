using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

[CreateAssetMenu(fileName = "Cities", menuName = "ScriptableObjects/Cities")]
public class CitiesScriptableObject : ScriptableObject
{
    public string cityName;
    public int cityNumber;
    public List<GameObject> CityTiles = new List<GameObject>();
    //public Dictionary<districts, Vector2Int> Districts = new Dictionary<districts, Vector2Int>();
    public void constructor (string _cityName, int _cityNumber){
        cityName = _cityName;
        cityNumber = _cityNumber;
    }
}
