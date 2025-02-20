using UnityEngine;

public class ServeStation : Interactable
{
    public CustomerSpawner customerSpawner;
    public PlayerController player;

    protected override void OnZ()
    {
        if (customerSpawner.serveQueue.customerQueue.Count > 0 && customerSpawner.serveQueue.Peek().GetComponent<CustomerController>().GetState() == CustomerState.readyToBeServed)
        {
            Debug.Log("Serving Customer");
            customerSpawner.CustomerServed();
        }
        else
        {
            Debug.Log("No customers in line");
        }
    }
}