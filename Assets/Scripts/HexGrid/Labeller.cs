using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//used https://www.youtube.com/watch?v=4JaHSLA2CKs for this
[ExecuteAlways]
public class Labeller : MonoBehaviour
{
    [SerializeField] TextMeshPro label;
    public Vector2 cords = new Vector2Int();
    HexGrid gridManager;

    [SerializeField] private bool displayName = false;

    void Awake()
    {
        gridManager = FindObjectOfType<HexGrid>();
        
        label = GetComponentInChildren<TextMeshPro>();

        DisplayCords();
    }
    void Update(){
        DisplayCords();
        transform.name = cords.ToString();
        
        if(!displayName){
            label.text = "";
        }
    }

    private void DisplayCords()
    {
        if (!gridManager) { return; }
        cords.x = transform.position.x;
        cords.y = transform.position.z;

        label.text = $"{cords.x}, {cords.y}";
    }
}
