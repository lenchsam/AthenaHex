using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistrictManager : MonoBehaviour
{
    [SerializeField] List<CitiesScriptableObject> AllCities = new List<CitiesScriptableObject>();
    private CitiesManager citiesManager;
    public GameObject UI_CityCentre;
    public GameObject UI_Barracks;
    private GameObject currentlyEnabled;
    private Barracks selectedBarracks;
    [HideInInspector] public bool waitingForClick = false;
        [SerializeField] GameObject BarracksPrefab;
//-------------------------------------------------------------------------
    void Start(){
        citiesManager = FindObjectOfType<CitiesManager>();
    }
    public void waitForClick(){
        waitingForClick = true;
    }
    public void BuildBarracks(RaycastHit hit){
        //Debug.Log("RAN");
        TileScript tileScript = hit.transform.gameObject.GetComponent<TileScript>();
        //if isnt part of the
        if(tileScript.SO_Cities == null){
            Debug.Log("not part of a city");
        }
        if(tileScript.SO_Cities == citiesManager.GetCitySOFromTile(hit.transform.gameObject)){
            Debug.Log("IS PART OF THE CITY");
            tileScript.gameObject.AddComponent<Barracks>();
            Instantiate(BarracksPrefab,tileScript.gameObject.transform.position, Quaternion.Euler(0, 90, 0));//instantiate barracks.

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


}
public enum district{
    None,
    CityCentre,
    Barrack
}