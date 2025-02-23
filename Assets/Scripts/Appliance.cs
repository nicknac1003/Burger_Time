using UnityEngine;
using System.Collections.Generic;
public class Appliance : Storage
{
    [SerializeField] protected Breakable breakable;

    [SerializeField] private bool inUse = false;
    protected bool working = true;

    public bool InUse() => inUse;

    protected virtual void Update()
    {
        if (breakable.CanBreak())
        {
            breakable.HandleBreaking(this);
            breakable.HandleRepairing(zPressed, this);
            working = breakable.IsBroken() == false;
        }
    }
}