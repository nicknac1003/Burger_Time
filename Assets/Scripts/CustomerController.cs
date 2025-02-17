using UnityEngine;
public enum CustomerState
{
    ordering,
    waiting,
    eating,
    leaving

}
public class CustomerController : MonoBehaviour
{
    private int queuePosition;
    private Transform targetPosition;
    private CustomerSpawner customerSpawner;
    private CustomerState state = CustomerState.ordering;
    void Awake()
    {
        customerSpawner = FindFirstObjectByType<CustomerSpawner>();

    }

    public void SetQueuePosition(int position)
    {
        queuePosition = position;
        UpdateTargetPosition();
    }

    void UpdateTargetPosition()
    {
        // Calculate the target position based on the queue position
        targetPosition = customerSpawner.GetQueuePosition(queuePosition, state);
    }

    void Update()
    {
        // Move towards the target position
        if (targetPosition != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.position, Time.deltaTime * 2f);
        }
    }

    // public void TakeOrder()
    // {
    //     // Logic for taking the order
    //     Debug.Log("Order taken for customer at position: " + queuePosition);
    //     customerSpawner.CustomerOrderTaken(this);
    // }
    // public void Serve()
    // {
    //     // Logic for getting the food
    //     Debug.Log("Food given to customer at position: " + queuePosition);
    //     customerSpawner.serveCustomer(this);
    // }
}