using UnityEngine;

public class ReadyToBeServed : Interactable
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Customer"))
        {   
            other.GetComponent<CustomerController>().ReadyToBeServed();
        }
    }
}