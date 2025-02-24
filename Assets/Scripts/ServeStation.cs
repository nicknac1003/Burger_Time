using System.Collections.Generic;
using UnityEngine;

public class ServeStation : Interactable
{
    public static ServeStation Instance { get; private set; }

    [SerializeField] Transform orderPosition;

    private BurgerObject burgerToServe;

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

    protected override void OnZ()
    {
        if(burgerToServe != null)
        {
            return;
        }

        if (OrderManager.CanServeFood() == false)
        {
            return;
        }

        Holdable item = PlayerController.GetItem();

        if (item == null || item is not BurgerObject)
        {
            return;
        }

        BurgerObject burgerObject = item as BurgerObject;
        PlaceBurger(burgerObject);

        List<Ticket> tickets = OrderManager.FindTickets(burgerObject.GetBurger());
        Ticket ticket;
        if (tickets == null)
        {
            ticket = OrderManager.LowestTimeLeftTicket();
        }
        else
        {
            ticket = tickets[0];
        }

        OrderManager.RemoveTicket(ticket);

        ticket.GetCustomer().SetState(CustomerState.PickingUpFood);
    }

    private void PlaceBurger(BurgerObject burger)
    {
        if(burger == null || burgerToServe != null)
        {
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
            return;
        }

        customer.GrabBurger(Instance.burgerToServe);
        Instance.burgerToServe = null;
    }
}