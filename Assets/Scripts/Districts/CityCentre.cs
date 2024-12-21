using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityCentre : Districts, IInteractable
{
    public void OnClick(){
        _districtManager.UIToggle(_districtManager.UI_CityCentre);
        //Debug.Log("clicked cityCentre");
    }
}
