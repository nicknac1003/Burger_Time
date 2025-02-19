using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
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

    public float MaxWaitTime = 10f;
    private float waitTimer = 0f;

    private Image image;

    private float maxRating = 6f;
    private float finalRating = 0f;

    private GameManager gameManager;
    private void Awake()
    {
        customerSpawner = FindFirstObjectByType<CustomerSpawner>();
        image = GetComponentInChildren<Image>();
        gameManager = GameManager.Instance;
    }
    private void Start()
    {
        if (image) HideTimerImage();
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
            if (Vector3.Distance(transform.position, targetPosition.position) < 0.1f && state == CustomerState.entering)
            {
                state = CustomerState.waitingToOrder;
            }
        }
        if (state != CustomerState.entering && image.gameObject.activeSelf == false)
        {
            image.gameObject.SetActive(true);
        }

        if (state != CustomerState.entering && state != CustomerState.leaving)
        {

            waitTimer += Time.deltaTime;
            if (waitTimer >= MaxWaitTime)
            {
                CustomerWaitTimeOut();
            }
            UpdateTimerImage();
        }
        if (state == CustomerState.leaving)
        {
            HideTimerImage();
        }

    }
    public void CustomerWaitTimeOut()
    {
        customerSpawner.customerWaitTimeOut(this);
        state = CustomerState.leaving;
        targetPosition = customerSpawner.exitPosition;
        GameManager.Instance.WelpReview(0);
    }
    public void ReadyToOrder()
    {
        state = CustomerState.readyToOrder;
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
        finalRating = CalculateReview();
        gameManager.WelpReview(finalRating);
    }
    public CustomerState GetState()
    {
        return state;
    }
    private void UpdateTimerImage()
    {
        image.fillAmount = 1 - (waitTimer / MaxWaitTime);
        image.color = Color.Lerp(Color.red, Color.green, image.fillAmount);
    }
    private void HideTimerImage()
    {
        image.gameObject.SetActive(false);
    }
    private float CalculateReview()
    {
        //edit here for burger scoring effect on review
        return Mathf.Min(maxRating * (MaxWaitTime - waitTimer) / MaxWaitTime, 5f);
    }
    public float GetRating()
    {
        return finalRating;
    }
}