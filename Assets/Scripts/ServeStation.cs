using UnityEngine;

public class ServeStation : Interactable
{
    protected override void OnZ()
    {
        CustomerManager.ServeFood();
    }
}