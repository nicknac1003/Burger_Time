using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] private float[]     popularity = new float[24];
    [SerializeField] private float       spawnTimer;
    [SerializeField] private Transform   spawnPoint;
    [SerializeField] private Transform   exitPoint;
    [SerializeField] private Transform   lineStart;
    [SerializeField] private BoxCollider2D waitingArea;
    [SerializeField] private int   maxLineLength;
    [SerializeField] private float lineSpacing;
    [SerializeField] private int   maxCapacity;
    [SerializeField] private float minBurgerTime;
    [SerializeField] private float maxBurgerTime;
    [SerializeField] private float customerMoveSpeed;
    [SerializeField] private float maxWaitTime;
    [SerializeField] private float requestIngredientTime;
    [SerializeField] private int   maxToppings = 8;

    public static CustomerManager Instance { get; private set; }

    private float timeSinceLastSpawn = 0;
    private int customerCount   = 0;
    private int inBuilding      = 0;
    private List<Customer> line = new();

    public static (float, float) GetBurgerTimeRange() => (Instance.minBurgerTime, Instance.maxBurgerTime);
    public static float   CustomerMoveSpeed() => Instance.customerMoveSpeed;
    public static Vector3 Exit()              => Instance.exitPoint.position;
    public static float   MaxWaitTime()       => Instance.maxWaitTime;
    public static float   RequestTime()       => Instance.requestIngredientTime;
    public static int     MaxToppings()       => Instance.maxToppings;
    public static int     Customers()         => Instance.customerCount;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if(timeSinceLastSpawn > spawnTimer && CustomerArrives())
        {
            timeSinceLastSpawn = 0;
            line.Add(SpawnCustomer());
        }
    }

    private GameObject GenerateCustomer()
    {
        // Empty for now, will implement code to pick a random customer later
        return new GameObject("Customer " + customerCount);
    }

    public Customer SpawnCustomer()
    {
        GameObject newCustomer = GenerateCustomer();
        newCustomer.AddComponent<Customer>().Init(customerCount);
        newCustomer.transform.position = spawnPoint.position;
        return newCustomer.GetComponent<Customer>();
    }

    private bool AtCapacity()
    {
        return inBuilding >= maxCapacity;
    }
    private bool LineTooLong()
    {
        return line.Count >= maxLineLength;
    }

    private bool CustomerArrives()
    {
        if(GameManager.Open() == false) return false;

        if(LineTooLong() || AtCapacity()) return false;

        int hour = GameManager.GetHour();
        float currentPopularity = Mathf.Lerp(popularity[hour], popularity[(hour + 1) % 24], GameManager.GetHourCompletion());

        if(Random.value > currentPopularity) return false;

        customerCount++;
        inBuilding++;

        return true;
    }

    public void CustomerLeaves(Customer customer)
    {
        CustomerState state = customer.GetState();

        if(state == CustomerState.WaitingToOrder) line.Remove(customer);
        if(state == CustomerState.WaitingForFood) // remove order

        customer.SetState(CustomerState.Leaving);

        inBuilding--;
    }

    public Vector3 GetRandomWaitingPosition()
    {
        Vector3 center = waitingArea.bounds.center;
        Vector3 size   = waitingArea.bounds.size;

        return new Vector3(
            center.x + Random.Range(-size.x / 2, size.x / 2),
            center.y + Random.Range(-size.y / 2, size.y / 2),
            0
        );
    }
    public Vector3 GetSpotInLine(Customer customer)
    {
        return lineStart.position + new Vector3(Random.Range(-0.5f, 0.5f), 0, -line.IndexOf(customer) * lineSpacing);
    }

    public static void TakeOrder()
    {
        if(OrderManager.CanTakeOrder() == false) return;

        if (Instance.line.Count <= 0 || Vector3.Distance(Instance.line[0].transform.position, Instance.lineStart.position) > 0.1f) return;
        
        Customer customer = Instance.line[0];
        customer.SetState(CustomerState.Ordering);
        Instance.line.RemoveAt(0);
    }

    public static void ServeFood()
    {
        if(OrderManager.CanServeFood() == false) return;

        Holdable item = PlayerController.GetItem();
        if(item == null || item is not BurgerObject) return;

        Burger order = (item as BurgerObject).GetBurger();

        Ticket ticket = OrderManager.FindTicket(order);
        if(ticket == null) ticket = OrderManager.OldestTicket();

        OrderManager.RemoveTicket(ticket);

        Customer serving = ticket.GetCustomer();
        serving.SetState(CustomerState.Eating);
        
        float score = serving.GiveReview();
        GameManager.WelpReview(score);
        new GameObject(item + "'s Score").AddComponent<RatingPopup>().SetRating(score);
    }
}