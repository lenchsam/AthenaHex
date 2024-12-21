using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistrictManager : MonoBehaviour
{
    List<CitiesScriptableObject> _allCities = new List<CitiesScriptableObject>();
    private CitiesManager _citiesManager;
    public GameObject UI_CityCentre;
    public GameObject UI_Barracks;
    private GameObject _currentlyEnabled;
    private Barracks _selectedBarracks;
    private eOccupiedBy _selectedBuilding;
    [HideInInspector] public bool WaitingForClick = false;
    [SerializeField] GameObject _barracksPrefab;
    public CitiesScriptableObject SelectedCitiesScriptableObject;
    //-------------------------------------------------------------------------
    void Start(){
        _citiesManager = FindAnyObjectByType<CitiesManager>();
    }
    public void waitForClick(){
        WaitingForClick = true;
    }
    public void BuildBarracks(RaycastHit hit){
        TileScript tileScript = hit.transform.gameObject.GetComponent<TileScript>();

        //if the city already contains this district then return.
        if(checkCitiesSOForDistrict(SelectedCitiesScriptableObject, eDistrict.Barrack)){return;}
        
        //if the tile the player hit is part of the city
        if(SelectedCitiesScriptableObject == _citiesManager.GetCitySOFromTile(hit.transform.gameObject)){
            tileScript.gameObject.AddComponent<Barracks>();
            tileScript.OccupiedBy = eOccupiedBy.barracks;
            Instantiate(_barracksPrefab,tileScript.gameObject.transform.position, Quaternion.Euler(0, 90, 0));//instantiate barracks.
            SelectedCitiesScriptableObject.containedDistricts.Add(eDistrict.Barrack);
            //instantiate the barracks GO
        }
    }
    public void UIToggle(GameObject uiToEnable){
        //set the currently activeUI to inactive
        if(_currentlyEnabled != null)
            _currentlyEnabled.SetActive(false);

        //enable UI
        uiToEnable.SetActive(true); 
        _currentlyEnabled = uiToEnable;
    }
    public void SetSelectedBarracks(Barracks barracks)
    {
        _selectedBarracks = barracks;  // Set the selected barracks
    }
    public void SpawnEnemyButtonClicked(GameObject enemyPrefab)
    {
        if (_selectedBarracks == null){return;}

        if(_selectedBarracks.gameObject.GetComponent<TileScript>().OccupiedUnit == null){
            _selectedBarracks.SpawnEnemy(enemyPrefab);  // Only spawn enemy at the selected barracks
        }
    }
    private bool checkCitiesSOForDistrict(CitiesScriptableObject citiesScriptableObject, eDistrict _district){
        foreach(eDistrict dist in citiesScriptableObject.containedDistricts){
            if(dist == _district){
                return true;
            }
        }
        return false;
    }
}
public enum eDistrict{
    None,
    CityCentre,
    Barrack
}