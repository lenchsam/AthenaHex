using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistrictManager : MonoBehaviour
{
    List<CitiesScriptableObject> AllCities = new List<CitiesScriptableObject>();
    private CitiesManager citiesManager;
    public GameObject UI_CityCentre;
    public GameObject UI_Barracks;
    private GameObject currentlyEnabled;
    private Barracks selectedBarracks;
    private eOccupiedBy selectedBuilding;
    [HideInInspector] public bool waitingForClick = false;
    [SerializeField] GameObject BarracksPrefab;
    public CitiesScriptableObject selectedCitiesScriptableObject;
    //-------------------------------------------------------------------------
    void Start(){
        citiesManager = FindAnyObjectByType<CitiesManager>();
    }
    public void waitForClick(){
        waitingForClick = true;
    }
    public void BuildBarracks(RaycastHit hit){
        TileScript tileScript = hit.transform.gameObject.GetComponent<TileScript>();

        //if the city already contains this district then return.
        if(checkCitiesSOForDistrict(selectedCitiesScriptableObject, eDistrict.Barrack)){return;}
        
        //if the tile the player hit is part of the city
        if(selectedCitiesScriptableObject == citiesManager.GetCitySOFromTile(hit.transform.gameObject)){
            tileScript.gameObject.AddComponent<Barracks>();
            tileScript.occupiedBy = eOccupiedBy.barracks;
            Instantiate(BarracksPrefab,tileScript.gameObject.transform.position, Quaternion.Euler(0, 90, 0));//instantiate barracks.
            selectedCitiesScriptableObject.containedDistricts.Add(eDistrict.Barrack);
            //instantiate the barracks GO
        }
    }
    public void UIToggle(GameObject uiToEnable){
        //set the currently activeUI to inactive
        if(currentlyEnabled != null)
            currentlyEnabled.SetActive(false);

        //enable UI
        uiToEnable.SetActive(true); 
        currentlyEnabled = uiToEnable;
    }
    public void SetSelectedBarracks(Barracks barracks)
    {
        selectedBarracks = barracks;  // Set the selected barracks
    }
    public void SpawnEnemyButtonClicked(GameObject enemyPrefab)
    {
        if (selectedBarracks == null){return;}

        if(selectedBarracks.gameObject.GetComponent<TileScript>().occupiedUnit == null){
            selectedBarracks.SpawnEnemy(enemyPrefab);  // Only spawn enemy at the selected barracks
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