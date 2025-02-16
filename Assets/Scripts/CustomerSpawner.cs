using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab;
    public Transform spawnPoint;
    public float spawnInterval = 5f;
    private float spawnTimer = 0f;

    public QueueType orderQueue = new QueueType();
    public QueueType waitingQueue= new QueueType();

    public GameManager gameManager;

    public Transform orderQueueHead;
    public Transform waitingQueueHead;

    public void Start()
    {
        //space the queuepositions array based on queuehead 
        orderQueue.lineHead = orderQueueHead;
        orderQueue.generateQueuePositions();
        waitingQueue.lineHead = waitingQueueHead;
        waitingQueue.generateQueuePositions();

    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval && gameManager.canCreateCustomers)
        {
            spawnTimer = 0f;
            SpawnCustomer();
        }
    }

    void SpawnCustomer()
    {
        gameManager.addCustomer();
        GameObject customer = Instantiate(customerPrefab, spawnPoint.position, spawnPoint.rotation);
        Debug.Log(orderQueue.customerQueue);
        orderQueue.customerQueue.Enqueue(customer.transform);
        orderQueue.UpdateQueuePositions();
    }



    public Transform GetQueuePosition(int position, CustomerState state)
    {
        switch (state)
        {
            case CustomerState.ordering:
                return orderQueue.GetQueuePosition(position);
            case CustomerState.waiting:
                return waitingQueue.GetQueuePosition(position);
        }
        
        return null;
    }
    public void CustomerOrderTaken()
    {
        Transform customer = orderQueue.customerQueue.Dequeue();
        orderQueue.UpdateQueuePositions();
        waitingQueue.customerQueue.Enqueue(customer);
        waitingQueue.UpdateQueuePositions();
    }
    public void serveCustomer()
    {
        Transform customer = waitingQueue.customerQueue.Dequeue();
        waitingQueue.UpdateQueuePositions();
        gameManager.removeCustomer();
        Destroy(customer.gameObject); //do somekind of walkout later
    }

}
public class QueueType{
    public Queue<Transform> customerQueue = new Queue<Transform>();
    public Transform[] queuePositions;
    public Transform lineHead;
    public int queueLength = 10; // Number of positions in the queue
    public float spacing = 1f; // Spacing between each position in the queue

    public void generateQueuePositions(){
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