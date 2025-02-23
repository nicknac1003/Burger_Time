using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Customer : MonoBehaviour
{
    private int id;
    private float timeSpentInLine;
    private float timeSpentWaitingForOrder;
    private float timeToEatBurger;
    private CustomerState state;
    private Vector3 waitForFoodPosition;
    private bool startedOrdering;
    private float lineOffset;
    private Vector3 goal;
    private Vector3 prevGoal;

    [SerializeField] private CustomerState debugStateView;
    [SerializeField] private Animator animator;

    public int GetID() => id;
    public float GetTimeSpentInLine() => timeSpentInLine;
    public float GetTimeSpentWaitingForOrder() => timeSpentWaitingForOrder;
    public float LineOffset() => lineOffset;

    public CustomerState GetState() => state;
    public void SetState(CustomerState newState)
    {
        state = newState;
    }

    public void Init(int newID)
    {
        id = newID;
        lineOffset = Random.Range(-0.25f, 0.25f);
        (float minBurgerTime, float maxBurgerTime) = CustomerManager.GetBurgerTimeRange();
        timeToEatBurger = Random.Range(minBurgerTime, maxBurgerTime);
        waitForFoodPosition = CustomerManager.Instance.GetRandomWaitingPosition();
    }

    void Update()
    {
        debugStateView = state;
        switch (state)
        {
            case CustomerState.Entering:
                // We dont want time to tick up until the customer stops moving and is in line
                transform.position = Vector3.MoveTowards(transform.position, CustomerManager.Instance.GetSpotInLine(this), Time.deltaTime * CustomerManager.CustomerMoveSpeed());
                goal = CustomerManager.Instance.GetSpotInLine(this);
                if (Vector3.Distance(transform.position, CustomerManager.Instance.GetSpotInLine(this)) < 0.1f)
                {
                    SetState(CustomerState.WaitingToOrder);
                }
                break;

            case CustomerState.WaitingToOrder:
                transform.position = Vector3.MoveTowards(transform.position, CustomerManager.Instance.GetSpotInLine(this), Time.deltaTime * CustomerManager.CustomerMoveSpeed());
                goal = CustomerManager.Instance.GetSpotInLine(this);
                timeSpentInLine += Time.deltaTime;
                break;

            case CustomerState.Ordering:
                if (startedOrdering == false)
                {
                    startedOrdering = true;
                    StartCoroutine(PlaceOrder());
                }
                break;

            case CustomerState.WaitingForFood:
                timeSpentWaitingForOrder += Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, waitForFoodPosition, Time.deltaTime * CustomerManager.CustomerMoveSpeed());
                goal = waitForFoodPosition;
                break;

            case CustomerState.Eating:
                if (timeToEatBurger <= 0) SetState(CustomerState.Leaving);
                timeToEatBurger -= Time.deltaTime;
                break;

            case CustomerState.Leaving:
                transform.position = Vector3.MoveTowards(transform.position, CustomerManager.Exit(), Time.deltaTime * CustomerManager.CustomerMoveSpeed());
                goal = CustomerManager.Exit();
                if (Vector3.Distance(transform.position, CustomerManager.Exit()) < 0.1f)
                {
                    CustomerManager.Instance.CustomerLeaves(this);
                }
                break;
        }
        if (timeSpentInLine > CustomerManager.MaxWaitTime() || timeSpentWaitingForOrder > CustomerManager.MaxWaitTime())
        {
            CustomerManager.Instance.CustomerRefusedService(this);
        }
        AdjustAnimation();
    }

    private void AdjustAnimation()
    {
        Vector2 diff = transform.position - goal;
        float xDiff = Mathf.Abs(diff.x);
        float yDiff = Mathf.Abs(diff.y);
        if (diff.magnitude > 0.1f)
        {
            animator.SetBool("Moving", true);
            if (prevGoal != goal)
            {
                animator.SetTrigger("DirectionChange");
                if (xDiff > yDiff)
                {
                    if (diff.x > 0)
                    {
                        animator.SetFloat("xVel", -1);
                        animator.SetFloat("yVel", 0);
                    }
                    else
                    {
                        animator.SetFloat("xVel", 1);
                        animator.SetFloat("yVel", 0);
                    }
                }
                else
                {
                    if (diff.y > 0)
                    {
                        animator.SetFloat("yVel", -1);
                        animator.SetFloat("xVel", 0);
                    }
                    else
                    {
                        animator.SetFloat("yVel", 1);
                        animator.SetFloat("xVel", 0);
                    }
                }
            }
        }
        else
        {
            animator.SetBool("Moving", false);
        }
        prevGoal = goal;
    }

    public float GiveReview()
    {
        float lineSpeed = 1 - timeSpentInLine / CustomerManager.MaxWaitTime();
        float orderSpeed = 1 - timeSpentWaitingForOrder / CustomerManager.MaxWaitTime();
        float normalized = (lineSpeed + orderSpeed) / 2;
        return normalized;
    }

    private IEnumerator PlaceOrder()
    {
        Debug.Log("Taking order for " + this);

        Burger order = Burger.GenerateRandomBurger(Random.Range(0, 1 + Mathf.Min(GameManager.GetDay(), CustomerManager.MaxToppings())));

        PlayerController.LockPlayer();

        GameObject speechBubble = Instantiate(GlobalConstants.speechBubble, transform);
        speechBubble.transform.localPosition = new Vector3(1.25f, 2.5f, 0);

        GameObject inBubble = Object.Instantiate(speechBubble, speechBubble.transform);
        inBubble.transform.localPosition = new Vector3(0, -5.5f * GlobalConstants.pixelWorldSize, 0);
        SpriteRenderer spriteRenderer = inBubble.GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = speechBubble.GetComponent<SpriteRenderer>().sortingOrder + 1;
        inBubble.SetActive(false);

        List<Ingredient> ingredients = order.GetIngredients();
        int ingredientCount = ingredients.Count;
        for (int i = 0; i < ingredientCount; i++)
        {
            if (ingredients[i].Type() == IngredientType.Plate) continue;

            spriteRenderer.sprite = ingredients[i].GetSprite();
            inBubble.SetActive(true);

            yield return new WaitForSeconds(CustomerManager.RequestTime());
            inBubble.SetActive(false);

            if (i != ingredientCount - 1)
            {
                yield return new WaitForSeconds(0.15f); // Buffer for clear separation between ingredients except last
            }
        }

        Destroy(speechBubble);
        PlayerController.UnlockPlayer();

        SetState(CustomerState.WaitingForFood);

        CustomerManager.RemoveCustomerFromLine(this);

        Debug.Log(this + " ordered " + order);

        OrderManager.NewTicket(order, this);
    }
}

public enum CustomerState
{
    Entering,
    WaitingToOrder,
    Ordering,
    WaitingForFood,
    Eating,
    Leaving
}