using UnityEngine;

public class Holdable : Interactable
{
    protected bool toggleHold = false;

    public override void InteractC(bool held)
    {
        // toggle held state
        if(held)
        {
            toggleHold = !toggleHold;
        }

        // pick up or drop item depending on held state
        if(toggleHold)
        {
            PlayerController.Instance.PickUpItem(this);
        }
        else
        {
            PlayerController.Instance.DropItem();
        }
    }

    public void SetHold(bool holding) => toggleHold = holding; // for when an item gets swapped and needs to be set back to not held
}