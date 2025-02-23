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
    private Vector3 eatFoodPosition;
    private bool startedOrdering;
    private float lineOffset;
    private Vector3 goal;
    private Vector3 prevGoal;

    private BurgerObject burger;

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
        eatFoodPosition = CustomerManager.Instance.GetRandomEatingPosition();
    }

    void Update()
    {
        debugStateView = state;
        switch (state)
        {
            case CustomerState.Entering:
                // We dont want time to tick up until the customer stops moving and is in line
                goal = CustomerManager.Instance.GetSpotInLine(this);
                transform.position = Vector3.MoveTowards(transform.position, goal, Time.deltaTime * CustomerManager.CustomerMoveSpeed());
                if (Vector3.Distance(transform.position, goal) < 0.1f)
                {
                    SetState(CustomerState.WaitingToOrder);
                }
            break;

            case CustomerState.WaitingToOrder:
                goal = CustomerManager.Instance.GetSpotInLine(this);
                transform.position = Vector3.MoveTowards(transform.position, goal, Time.deltaTime * CustomerManager.CustomerMoveSpeed());
                timeSpentInLine += Time.deltaTime;

                if(timeSpentInLine > CustomerManager.MaxWaitTime())
                {
                    CustomerManager.Instance.CustomerRefusedService(this);
                }
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
                goal = waitForFoodPosition;
                transform.position = Vector3.MoveTowards(transform.position, goal, Time.deltaTime * CustomerManager.CustomerMoveSpeed());

                if(timeSpentWaitingForOrder > CustomerManager.MaxWaitTime())
                {
                    CustomerManager.Instance.CustomerRefusedService(this);
                }
            break;

            case CustomerState.PickingUpFood:
                goal = CustomerManager.Pickup();
                transform.position = Vector3.MoveTowards(transform.position, goal, Time.deltaTime * CustomerManager.CustomerMoveSpeed());
                if(Vector3.Distance(transform.position, goal) < 0.1f)
                {
                    ServeStation.PickUpBurger(this);
                    SetState(CustomerState.Eating);
                    GameManager.WelpReview(this, GiveReview());
                }
            break;

            case CustomerState.Eating:
                goal = eatFoodPosition;
                transform.position = Vector3.MoveTowards(transform.position, goal, Time.deltaTime * CustomerManager.CustomerMoveSpeed());
                if (timeToEatBurger <= 0)
                {
                    // destroy burger and return plate to dirty stack
                    Destroy(burger.gameObject);

                    CustomerManager.ReturnPlate();

                    SetState(CustomerState.Leaving);
                }
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

    public void GrabBurger(BurgerObject newBurger)
    {
        burger = newBurger;
        burger.transform.SetParent(transform);
        burger.transform.localPosition = new(0f, 1f, 0f);
    }

    private IEnumerator PlaceOrder()
    {
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
    PickingUpFood,
    Leaving
}