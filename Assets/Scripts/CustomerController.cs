using UnityEngine;
using System.Collections.Generic;
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
    [Header("Customer Order Parameters")]
    public int minIngredients = 1;
    public int maxIngredients = 5;
    private int queuePosition;
    private Transform targetPosition;
    private CustomerSpawner customerSpawner;
    private CustomerState state = CustomerState.entering;
    public ReceiptGenerator receiptGenerator;
    private List<string> order = new List<string>();
    void Awake()
    {
        customerSpawner = FindFirstObjectByType<CustomerSpawner>();
    }

    void GenerateOrder()
    {
        List<string> ingredients = receiptGenerator.ingredientNames;
        int numIngredients = Random.Range(minIngredients, maxIngredients + 1);

        for (int i = 0; i < numIngredients; i++)
        {
            order.Add(ingredients[Random.Range(0, ingredients.Count)]);
        }

        receiptGenerator.GenerateReceipt(order);
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
        // Player has taken the order
        GenerateOrder();

        // Logic for taking the order
        state = CustomerState.waitingToBeServed;
    }
    public void Served()
    {
        // Logic for getting the food
        state = CustomerState.leaving;
        targetPosition = customerSpawner.exitPosition;
    }
    public CustomerState GetState()
    {
        return state;
    }
}