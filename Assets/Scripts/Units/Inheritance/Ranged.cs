using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranged : Units
{
    protected virtual void Awake()
    {
        e_defenceType = unitTypes.Ranged;
    }
    protected override void Start()
    {
        base.Start();
    }
}
