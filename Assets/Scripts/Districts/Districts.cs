using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Districts : MonoBehaviour
{
    [SerializeField] protected TileScript _tileScript;
    [SerializeField] protected DistrictManager _districtManager;
    public void Start(){
        _tileScript = gameObject.GetComponent<TileScript>();
        _districtManager = FindAnyObjectByType<DistrictManager>();
    }
}
