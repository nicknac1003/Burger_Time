using UnityEngine;

public class OrderStation: Interactable
{
    protected override void OnZ()
    {
        CustomerManager.TakeOrder();
    }
}