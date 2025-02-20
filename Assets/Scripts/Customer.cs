using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Customer : MonoBehaviour
{
    private int   id;
    private float timeSpentInLine;
    private float timeSpentWaitingForOrder;
    private float timeToEatBurger;
    private CustomerState state;
    private Vector3 waitForFoodPosition;
    private Vector3 waitInLinePosition;
    private bool startedOrdering;
    private bool doneOrdering;

    public float GetTimeSpentInLine() => timeSpentInLine;
    public float GetTimeSpentWaitingForOrder() => timeSpentWaitingForOrder;

    public void Init(int newID)
    {
        id = newID;
        (float minBurgerTime, float maxBurgerTime) = CustomerManager.GetBurgerTimeRange();
        timeToEatBurger     = Random.Range(minBurgerTime, maxBurgerTime);
        waitForFoodPosition = CustomerManager.Instance.GetRandomWaitingPosition();
    }

    void Update()
    {
        switch(state)
        {
            case CustomerState.Entering:
                // We dont want time to tick up until the customer stops moving and is in line
                Vector3.MoveTowards(transform.position, CustomerManager.Instance.GetSpotInLine(this), Time.deltaTime * CustomerManager.CustomerMoveSpeed());
                if(Vector3.Distance(transform.position, CustomerManager.Instance.GetSpotInLine(this)) < 0.1f)
                {
                    SetState(CustomerState.WaitingToOrder);
                }
            break;

            case CustomerState.WaitingToOrder:
                timeSpentInLine += Time.deltaTime;
                Vector3.MoveTowards(transform.position, CustomerManager.Instance.GetSpotInLine(this), Time.deltaTime * CustomerManager.CustomerMoveSpeed());
            break;

            case CustomerState.Ordering:
                if(startedOrdering == false)
                {
                    startedOrdering = true;
                    StartCoroutine(PlaceOrder());
                }
            break;

            case CustomerState.WaitingForFood:
                timeSpentWaitingForOrder += Time.deltaTime;
                Vector3.MoveTowards(transform.position, waitForFoodPosition, Time.deltaTime * CustomerManager.CustomerMoveSpeed());
            break;

            case CustomerState.Eating:
                if(timeToEatBurger <= 0) SetState(CustomerState.Leaving);
                timeToEatBurger -= Time.deltaTime;
            break;

            case CustomerState.Leaving:
                Vector3.MoveTowards(transform.position, CustomerManager.Exit(), Time.deltaTime * CustomerManager.CustomerMoveSpeed());
            break;
        }

        if(timeSpentInLine > CustomerManager.MaxWaitTime() || timeSpentWaitingForOrder > CustomerManager.MaxWaitTime())
        {
            CustomerManager.Instance.CustomerLeaves(this);
        }
    }

    public void SetState(CustomerState newState)
    {
        state = newState;
    }
    public CustomerState GetState()
    {
        return state;
    }

    public float GiveReview()
    {
        float lineSpeed  =  1 - timeSpentInLine / CustomerManager.MaxWaitTime();
        float orderSpeed =  1 - timeSpentWaitingForOrder / CustomerManager.MaxWaitTime();
        float normalized = (lineSpeed + orderSpeed) / 2;
        return normalized;
    }

    private IEnumerator PlaceOrder()
    {
        Burger order = Burger.GenerateRandomBurger(Random.Range(0, Mathf.Min(GameManager.GetDay(), CustomerManager.MaxToppings())));

        PlayerController.LockPlayer();

        GameObject speechBubble = Instantiate(GlobalConstants.speechBubble, transform);
        speechBubble.transform.localPosition = new Vector3(0.5f, 1.5f, 0);

        GameObject inBubble = Object.Instantiate(speechBubble, speechBubble.transform);
        SpriteRenderer spriteRenderer = inBubble.GetComponent<SpriteRenderer>();
        inBubble.SetActive(false);

        List<Ingredient> ingredients = order.GetIngredients();
        int ingredientCount = ingredients.Count;
        for(int i = 0; i < ingredientCount; i++)
        {
            spriteRenderer.sprite = ingredients[i].GetSprite();
            inBubble.SetActive(true);

            yield return new WaitForSeconds(CustomerManager.RequestTime());
            inBubble.SetActive(false);

            if(i != ingredientCount - 1)
            {
                yield return new WaitForSeconds(0.15f); // Buffer for clear separation between ingredients except last
            }
        }

        Destroy(speechBubble);

        PlayerController.UnlockPlayer();

        SetState(CustomerState.WaitingForFood);

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