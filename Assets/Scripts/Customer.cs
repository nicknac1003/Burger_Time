using System.Collections;
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
        Burger order = Burger.GenerateRandomBurger(Random.Range(0, GameManager.GetDay()));

        PlayerController.LockPlayer();

        GameObject speechBubble = Instantiate(GlobalConstants.speechBubble, transform);
        speechBubble.transform.localPosition = new Vector3(0.5f, 1.5f, 0);

        IngredientObject ingredientObject = null;
        foreach(Ingredient ingredient in order.GetIngredients())
        {
            if(ingredientObject == null)
            {
                ingredientObject = IngredientObject.Instantiate(ingredient);
            }
            else
            {
                yield return new WaitForSeconds(0.15f); // Buffer for clear separation between ingredients

                ingredientObject.ChangeIngredient(ingredient);
                ingredientObject.enabled = true;
            }

            yield return new WaitForSeconds(CustomerManager.RequestTime());
            ingredientObject.enabled = false;
        }

        Destroy(speechBubble);

        PlayerController.UnlockPlayer();

        SetState(CustomerState.WaitingForFood);

        // Generate ticket for order
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