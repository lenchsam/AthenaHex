using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Ranged, IAttacking
{
    [SerializeField] protected float maximumHealth = 25.0f;
    [SerializeField] HexGrid hexGrid;
    protected override void Awake()
    {
        base.Awake();
        initialise(maximumHealth);
        hexGrid = FindObjectOfType<HexGrid>();
    }

    // Update is called once per frame
    protected override void Start()
    {
        base.Start();
    }
    public void attack(GameObject thingToAttack){
        //if thing is x tiles away then attack
        Vector2Int startCords = hexGrid.GetTileScript(new Vector2(gameObject.transform.position.x, gameObject.transform.position.z)).intCoords;
        Vector2Int targetCords = hexGrid.GetTileScript(new Vector2(thingToAttack.transform.position.x, thingToAttack.transform.position.z)).intCoords;

        //Debug.Log();
        

        int tileDistance = hexGrid.DistanceBetweenTiles(startCords, targetCords);
        if(tileDistance <= MaxAttackDistance){
            thingToAttack.GetComponent<IDamageable>().takeDamage(damage);
        }else{
            Debug.Log("too far away");
        }
        //thingToAttack.GetComponent<IDamageable>().takeDamage(damage);
    }
}
