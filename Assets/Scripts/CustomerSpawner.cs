using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public Transform exitPosition;
    public float spawnInterval = 5f;
    private float spawnTimer = 0f;

    public int maxCustomers = 5;

    public QueueType orderQueue = new QueueType();
    public QueueType serveQueue= new QueueType();

    public GameManager gameManager;

    public Transform orderQueueHead;
    public Transform serveQueueHead;

    public void Start()
    {
        //space the queuepositions array based on queuehead 
        orderQueue.lineHead = orderQueueHead;
        orderQueue.GenerateQueuePositions();
        serveQueue.lineHead = serveQueueHead;
        serveQueue.GenerateQueuePositions();

    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval )
        {
            spawnTimer = 0f;
            if (gameManager.canCreateCustomers && gameManager.getCurrentCustomerCount() < maxCustomers)
                SpawnCustomer();
        }
    }

    void SpawnCustomer()
    {
        gameManager.addCustomer();
        GameObject customer = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        orderQueue.customerQueue.Enqueue(customer.transform);
        orderQueue.UpdateQueuePositions();
    }



    public Transform GetQueuePosition(int position, CustomerState state)
    {
        switch (state)
        {
            case CustomerState.entering:
            case CustomerState.waitingToOrder:
                return orderQueue.GetQueuePosition(position);
            case CustomerState.waitingToBeServed:
                return serveQueue.GetQueuePosition(position);
        }
        
        return null;
    }
    public void CustomerOrderTaken()
    {
        Transform customer = orderQueue.customerQueue.Dequeue();
        orderQueue.UpdateQueuePositions();
        serveQueue.customerQueue.Enqueue(customer);
        customer.GetComponent<CustomerController>().OrderTaken();
        serveQueue.UpdateQueuePositions();
    }
    public void CustomerServed()
    {
        Transform customer = serveQueue.customerQueue.Dequeue();
        customer.GetComponent<CustomerController>().Served();
        serveQueue.UpdateQueuePositions();
        gameManager.removeCustomer();
        
        Destroy(customer.gameObject, 5f); //do somekind of walkout later
    }

}

[System.Serializable]
public class QueueType{
    public Queue<Transform> customerQueue = new Queue<Transform>();
    public Transform[] queuePositions;
    public Transform lineHead;
    public int queueLength = 10; // Number of positions in the queue
    public float spacing = 1f; // Spacing between each position in the queue

    public void GenerateQueuePositions(){
        queuePositions = new Transform[queueLength];
        for (int i = 0; i < queueLength; i++)
        {
            GameObject positionMarker = new GameObject("QueuePosition" + (i + 1));
            positionMarker.transform.position = lineHead.position + new Vector3(-i * spacing, 0, 0);
            queuePositions[i] = positionMarker.transform;
        }
    }
    public void UpdateQueuePositions()
    {
        int position = 1;
        foreach (Transform customer in customerQueue)
        {
            customer.GetComponent<CustomerController>().SetQueuePosition(position);
            position++;
        }
    }
    public Transform GetQueuePosition(int position)
    {
        if (position - 1 < queuePositions.Length)
        {
            return queuePositions[position - 1];
        }
        return null;
    }
}