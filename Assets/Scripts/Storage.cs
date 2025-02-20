using UnityEngine;
using System.Collections.Generic;

public class Storage : Interactable
{
    [Tooltip("The list of holdable items that can be stored in this storage.")]
    [SerializeField] private List<Holdable> acceptedHoldables = new();

    [Tooltip("The anchor point where the holdable item will be placed.")]
    [SerializeField] private Transform anchor;
    
    private Holdable holdable;

    protected override void OnZ()
    {
        if(PlayerController.HoldingItem())
        {
            if(PlaceItem(PlayerController.GetItem()))
            {
                // visual feedback for placing item?
            }
        }
        else
        {
            if(TakeItem())
            {
                // visual feedback for taking item?
            }
        }
    }

    private bool PlaceItem(Holdable item)
    {
        if(holdable != null) return false;

        if(acceptedHoldables.Contains(item) == false) return false;

        holdable = item;
        holdable.transform.position = anchor.position;

        Debug.Log("Placed " + holdable.name + " in " + name);

        return true;
    }

    private bool TakeItem()
    {
        if(holdable == null) return false;

        if(PlayerController.HoldingItem()) return false;

        if(PlayerController.GrabItem(holdable) == false) return false;
            
        Debug.Log("Took " + holdable.name + " from " + name);

        holdable = null;

        return true;
    }
}