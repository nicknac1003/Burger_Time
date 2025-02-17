using UnityEngine;

public class Holdable : Interactable
{
    public override void InteractC(bool held)
    {
        if(held)
        {
            PlayerController.Instance.PickUpItem(this);
        }
        else
        {
            PlayerController.Instance.DropItem();
        }
    }
}