using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cities", menuName = "ScriptableObjects/Cities")]
public class CitiesScriptableObject : ScriptableObject
{
    public string cityName;
    public int cityNumber;
    public Vector2 cityCentreCords;
    public e_Team ownedBy;
    public List<GameObject> CityTiles = new List<GameObject>();
    public List<eDistrict> containedDistricts = new List<eDistrict>();
    //public Dictionary<districts, Vector2Int> Districts = new Dictionary<districts, Vector2Int>();
    public void constructor (string _cityName, int _cityNumber, e_Team _ownedBy, Vector2 _cityCentreCords){
        cityName = _cityName;
        cityNumber = _cityNumber;
        ownedBy = _ownedBy;
        cityCentreCords = _cityCentreCords;
    }
}
