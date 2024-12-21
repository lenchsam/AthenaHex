using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Districts, IInteractable
{
    public void OnClick(){
        //Debug.Log("clicked Barracks");
        _districtManager.UIToggle(_districtManager.UI_Barracks);
        _districtManager.SetSelectedBarracks(this);  // Notify DistrictManager of this barracks
    }
    public void SpawnEnemy(GameObject enemyPrefab){
        _tileScript.OccupiedUnit = Instantiate(enemyPrefab, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1f, gameObject.transform.position.z), Quaternion.identity);
        _tileScript.IsWalkable = false;
    }
}
