using UnityEngine;

public class OrderStation : Interactable
{
    public static OrderStation Instance { get; private set; }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected override void OnZ()
    {
        CustomerManager.TakeOrder();
    }
}