using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : Units
{
    protected virtual void Awake()
    {
        defenceType = unitTypes.Melee;
    }
    protected override void Start()
    {
        base.Start();

    }
}