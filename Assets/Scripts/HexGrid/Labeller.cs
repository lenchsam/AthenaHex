using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//used https://www.youtube.com/watch?v=4JaHSLA2CKs for this
public class Labeller : MonoBehaviour
{
    [SerializeField] TextMeshPro _label;
    public Vector2 Cords = new Vector2();
    public Vector2Int IntCords = new Vector2Int();
    HexGrid _gridManager;

    [SerializeField] private bool _displayName = false;

    void Awake()
    {
        _gridManager = FindAnyObjectByType<HexGrid>();
        
        _label = GetComponentInChildren<TextMeshPro>();

        DisplayCords();
        transform.name = Cords.ToString();

        if(!_displayName){
            _label.text = "";
        }
    }
    void Start(){

        IntCords = _gridManager.GetTileScriptFromPosition(Cords).IntCoords;
        //Debug.Log(intCords);
        _label.text = $"{IntCords.x}, {IntCords.y}";
                if(!_displayName){
            _label.text = "";
        }else{
            DisplayCords();
            transform.name = Cords.ToString();
        }

        //Destroy(label.gameObject);
        //Destroy(this);
    }

    private void DisplayCords()
    {
        if (!_gridManager) { return; }
        Cords.x = transform.position.x;
        Cords.y = transform.position.z;

        //label.text = $"{cords.x}, {cords.y}";
    }
}
