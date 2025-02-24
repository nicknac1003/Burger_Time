using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class CustomerManager : MonoBehaviour
{
    [SerializeField] GameObject[] customerPrefabs;
    [SerializeField] private float[] popularity = new float[24];
    [SerializeField] private float minSpawnAttemptTime;
    [SerializeField] private float maxSpawnAttemptTime;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private Transform lineStart;
    [SerializeField] private Transform pickupPoint;
    [SerializeField] private BoxCollider2D waitingArea;
    [SerializeField] private BoxCollider2D eatingArea;
    [SerializeField] private ObjectStack returnPlateStack;
    [SerializeField] private int maxLineLength;
    [SerializeField] private float lineSpacing;
    [SerializeField] private int maxCapacity;
    [SerializeField] private float minBurgerTime;
    [SerializeField] private float maxBurgerTime;
    [SerializeField] private float customerMoveSpeed;
    [SerializeField] private float maxWaitTime;
    [SerializeField] private float prepTimePerIngredient;
    [SerializeField] private float requestIngredientTime;
    [SerializeField] private int maxToppings = 8;
    [SerializeField] private AudioClip customerEnterSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float customerEnterVolume = 0.5f;


    public static CustomerManager Instance { get; private set; }

    private float timeSinceLastSpawn = 0;
    private float trySpawnAt = 0;
    private int customerCount = 0;
    private int inBuilding = 0;
    private List<Customer> line = new();

    public static (float, float) GetBurgerTimeRange() => (Instance.minBurgerTime, Instance.maxBurgerTime);
    public static float CustomerMoveSpeed() => Instance.customerMoveSpeed;
    public static Vector3 Exit() => Instance.exitPoint.position;
    public static Vector3 Pickup() => Instance.pickupPoint.position;
    public static float MaxWaitTime() => Instance.maxWaitTime;
    public static float PrepTimePerIngredient() => Instance.prepTimePerIngredient;
    public static float RequestTime() => Instance.requestIngredientTime;
    public static int MaxToppings() => Instance.maxToppings;
    public static int Customers() => Instance.customerCount;

    public static void ReturnPlate() => Instance.returnPlateStack.AddToStack();

    public List<Customer> lineDebugView;

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

        trySpawnAt = Random.Range(minSpawnAttemptTime, maxSpawnAttemptTime);
        if (audioSource != null)
        {
            audioSource.clip = customerEnterSound;
            audioSource.volume = customerEnterVolume;
        }
    }

    void Update()
    {
        lineDebugView = line;

        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn > trySpawnAt && CustomerArrives())
        {
            timeSinceLastSpawn = 0;
            line.Add(SpawnCustomer());
            trySpawnAt = Random.Range(minSpawnAttemptTime, maxSpawnAttemptTime);
        }
    }

    private GameObject GenerateCustomer()
    {
        GameObject customerObject = Instantiate(customerPrefabs[Random.Range(0, customerPrefabs.Length)]);
        customerObject.name = "Customer " + customerCount;
        return customerObject;
    }

    public Customer SpawnCustomer()
    {
        GameObject newCustomer = GenerateCustomer();
        newCustomer.transform.SetParent(transform);
        newCustomer.GetComponent<Customer>().Init(customerCount);
        newCustomer.transform.position = spawnPoint.position;
        PlayCustomerSpawnSound();
        return newCustomer.GetComponent<Customer>();
    }

    private void PlayCustomerSpawnSound()
    {
        audioSource.Play();
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
        if (GameManager.Open() == false) return false;

        if (LineTooLong() || AtCapacity()) return false;

        int hour = GameManager.GetHour();
        float currentPopularity = Mathf.Lerp(popularity[hour], popularity[(hour + 1) % 24], GameManager.GetHourCompletion());

        if (Random.value > currentPopularity) return false;

        customerCount++;
        inBuilding++;

        return true;
    }

    public void CustomerRefusedService(Customer customer)
    {
        CustomerState state = customer.GetState();

        if (state == CustomerState.WaitingToOrder) line.Remove(customer);
        if (state == CustomerState.WaitingForFood) OrderManager.RemoveTicket(OrderManager.FindTicket(customer));

        GameManager.WelpReview(customer, 0f);

        customer.SetState(CustomerState.Leaving);
    }

    public void CustomerLeaves(Customer customer)
    {
        inBuilding--;
        Destroy(customer.gameObject);
    }

    public Vector3 GetRandomEatingPosition()
    {
        Vector3 center = eatingArea.bounds.center;
        Vector3 size = eatingArea.bounds.size;

        return new Vector3(
            center.x + Random.Range(-size.x / 2, size.x / 2),
            center.y + Random.Range(-size.y / 2, size.y / 2),
            0
        );
    }

    public Vector3 GetRandomWaitingPosition()
    {
        Vector3 center = waitingArea.bounds.center;
        Vector3 size = waitingArea.bounds.size;

        return new Vector3(
            center.x + Random.Range(-size.x / 2, size.x / 2),
            center.y + Random.Range(-size.y / 2, size.y / 2),
            0
        );
    }
    public Vector3 GetSpotInLine(Customer customer)
    {
        return lineStart.position + new Vector3(customer.LineOffset(), -line.IndexOf(customer) * lineSpacing, 0);
    }
    private bool AtLineStart(Customer customer)
    {
        return Instance.line.IndexOf(customer) == 0 && Mathf.Abs(customer.transform.position.y - Instance.lineStart.position.y) < 0.1f;
    }

    public static void TakeOrder()
    {
        if (OrderManager.CanTakeOrder() == false) return;

        if (Instance.line.Count <= 0 || Instance.AtLineStart(Instance.line[0]) == false) return;

        Instance.line[0].SetState(CustomerState.Ordering);
    }

    public static void RemoveCustomerFromLine(Customer customer) // callback
    {
        Instance.line.Remove(customer);
    }
}