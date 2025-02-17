using UnityEngine;

public class ServeStation : Interactable
{
    public CustomerSpawner customerSpawner;
    public PlayerController player;
    public override void InteractZ(bool held)
    {
        if (held)
        {
            if (customerSpawner.waitingQueue.customerQueue.Count > 0 && customerSpawner.waitingQueue.customerQueue.Peek().GetComponent<CustomerController>().GetState() == CustomerState.waitingToBeServed)
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
}