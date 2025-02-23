using UnityEngine;
using System.Collections.Generic;
public class Appliance : Storage
{
    [SerializeField] protected Breakable breakable;

    [SerializeField] private bool inUse = false;
    protected bool working = true;

    public bool InUse() => inUse;
    public Breakable GetBreakable() => breakable;

    protected virtual void Update()
    {
        if (breakable.CanBreak() && !inUse)
        {
            breakable.HandleBreaking(this);
            breakable.HandleRepairing(zPressed, this);
            working = breakable.IsBroken() == false;
        }
    }
    protected override bool TakeItem()
    {
        if (!working) return false;

        return base.TakeItem();
    }
    protected override bool PlaceItem(Holdable item)
    {
        if (!working) return false;

        return base.PlaceItem(item);
    }
}