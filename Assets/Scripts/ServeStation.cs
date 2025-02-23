using UnityEngine;

public class ServeStation : Interactable
{
    public static ServeStation Instance { get; private set; }

    [SerializeField] Transform orderPosition;
    
    private BurgerObject burgerToServe;

    protected override void OnZ()
    {
        if(burgerToServe != null)
        {
            Debug.Log("Already serving a burger!");
            return;
        }

        if (OrderManager.CanServeFood() == false)
        {
            Debug.Log("No tickets!");
            return;
        }

        Holdable item = PlayerController.GetItem();

        if (item == null || item is not BurgerObject)
        {
            Debug.Log("No burger to serve!");
            return;
        }

        BurgerObject burgerObject = item as BurgerObject;
        PlaceBurger(burgerObject);

        Ticket ticket = OrderManager.FindTicket(burgerObject.GetBurger());
        if (ticket == null)
        {
            ticket = OrderManager.OldestTicket();
            Debug.Log("No ticket for this burger! Giving it to the oldest ticket: " + ticket);
        }
        else
        {
            Debug.Log("Serving ticket: " + ticket);
        }

        OrderManager.RemoveTicket(ticket);

        Customer serving = ticket.GetCustomer();
        serving.SetState(CustomerState.Eating);

        float score = serving.GiveReview();
        GameManager.WelpReview(score);
        new GameObject(item + "'s Score").AddComponent<RatingPopup>().SetRating(score);
    }

    private void PlaceBurger(BurgerObject burger)
    {
        if(burger == null || burgerToServe != null)
        {
            Debug.LogError("Burger is null or already serving a different burger");
            return;
        }

        burgerToServe = burger;
        burger.transform.SetParent(transform);
        burger.transform.position = orderPosition.position;

        PlayerController.SetHolding(null);
    }

    public static void PickUpBurger(Customer customer)
    {
        if(customer == null || Instance.burgerToServe == null)
        {
            Debug.LogError("Customer is null or no burger to serve");
            return;
        }
        Instance.burgerToServe.transform.SetParent(customer.transform);
        Instance.burgerToServe.transform.localPosition = new(0f, 2f, 0f);
        Instance.burgerToServe = null;
    }
}