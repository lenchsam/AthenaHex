using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Building : MonoBehaviour
{
    [SerializeField] GameObject objectToInstantiate;
    [SerializeField] bool isBuilding = false;

    private HexSnap hexSnap;
    void Start(){
        hexSnap = FindAnyObjectByType<HexSnap>();
    }
    public void PlaceDown(RaycastHit hit){
        if(!isBuilding){return;}
        var GO = Instantiate(objectToInstantiate, hit.transform.position, Quaternion.identity);
        GO.transform.rotation = Quaternion.Euler(0, GO.transform.eulerAngles.y + 30, 0);
    }
    public void rotateBuildnig(GameObject buildingToRotate){
        // Use eulerAngles to rotate the object around the Y-axis
        buildingToRotate.transform.rotation = Quaternion.Euler(0, buildingToRotate.transform.eulerAngles.y + 60, 0);
    }
}
