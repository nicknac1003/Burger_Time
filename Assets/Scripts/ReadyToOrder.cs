using UnityEngine;

public class ReadyToOrder : Interactable
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Customer"))
        {   
            other.GetComponent<CustomerController>().ReadyToOrder();
        }
    }
}