using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Districts : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject UIToEnable;
    public void OnClick(){
        UIToEnable.SetActive(true);
    }
}
