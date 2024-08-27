using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour, IInteractable
{
    public void OnClick()
    {
        Debug.Log("I AM CLICKED + " + gameObject.name);
    }
}
