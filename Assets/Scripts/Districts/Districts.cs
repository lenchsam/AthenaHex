using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Districts : MonoBehaviour
{
    [SerializeField] protected TileScript tileScript;
    [SerializeField] protected DistrictManager districtManager;
    public void Start(){
        tileScript = gameObject.GetComponent<TileScript>();
        districtManager = FindAnyObjectByType<DistrictManager>();
    }
}
