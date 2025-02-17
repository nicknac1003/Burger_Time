using UnityEngine;
public enum CustomerState
{
    entering,
    waitingToOrder,
    readyToOrder,
    ordering,
    waitingToBeServed,
    readyToBeServed,
    leaving

}
public class CustomerController : MonoBehaviour
{
    private int queuePosition;
    private Transform targetPosition;
    private CustomerSpawner customerSpawner;
    private CustomerState state = CustomerState.entering;
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
        Debug.Log(state);
    }
    public void ReadyToOrder()
    {
        state = CustomerState.ordering;
    }
    public void ReadyToBeServed()
    {
        state = CustomerState.readyToBeServed;
    }

    public void OrderTaken()
    {
        // Logic for taking the order
        state = CustomerState.waitingToBeServed;
    }
    public void Serve()
    {
        // Logic for getting the food
        state = CustomerState.leaving;
    }
    public CustomerState GetState()
    {
        return state;
    }
}