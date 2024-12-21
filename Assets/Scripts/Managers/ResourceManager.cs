using NUnit.Framework.Internal.Commands;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] int _woodAmountPerTurn;
    [SerializeField] int[] _woodAmount; //array should be the equal to the amout of players playing
    //------------------------------------------------------------------------------------------------------- RESOURCE PRODUCTION AND COLLECTION
    void AddResource(Resources resourceType, int amountm, e_Team team){

    }
    //calculates total production rate of a resource per turn
    int CalculateProductionRate(Resources resourceType, e_Team team){
        return 0;
    }
    //collects all resources from all resource types and adds them to the total
    void CollectResources(e_Team team){

    }
    //------------------------------------------------------------------------------------------------------- RESOURCE CONSUMPTION
    //deducts the required recources for building
    void ConsumeResource(Resources resourceType, int amount){
        
    }
    //returns true if the player has enough resources for a specified action
    bool HasSufficientResource(Resources resourceType, int amount){
        return true;
    }
    //applys the needed upkeep
    void ApplyUpkeepCosts(){

    }
    //------------------------------------------------------------------------------------------------------- UI
    //returns the amount of a specified resource
    int GetResourceAmount(Resources resourceType){
        return 0;
    }
    //------------------------------------------------------------------------------------------------------- 

}
public enum Resources{
    Wood,
}
