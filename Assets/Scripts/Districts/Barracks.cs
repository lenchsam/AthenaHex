using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : Districts, IInteractable
{
    public void OnClick(){
        //Debug.Log("clicked Barracks");
        districtManager.UIToggle(districtManager.UI_Barracks);
        districtManager.SetSelectedBarracks(this);  // Notify DistrictManager of this barracks
    }
    public void SpawnEnemy(GameObject enemyPrefab){
        tileScript.occupiedUnit = Instantiate(enemyPrefab, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1f, gameObject.transform.position.z), Quaternion.identity);
        tileScript.isWalkable = false;
    }
}
