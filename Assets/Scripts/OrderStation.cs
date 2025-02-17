using UnityEngine;

public class OrderStation: Interactable
{
    public CustomerSpawner customerSpawner;
    public override void InteractZ(bool held)
    {
        if (held)
        {
            if (customerSpawner.orderQueue.customerQueue.Count > 0 && customerSpawner.orderQueue.customerQueue.Peek().GetComponent<CustomerController>().GetState() == CustomerState.ordering)
            {
                //maybe add something here for during order taking
                Debug.Log("Order Taken");
                customerSpawner.CustomerOrderTaken();
            } else {
                Debug.Log("No customers in line");
            }
        }
    }
}