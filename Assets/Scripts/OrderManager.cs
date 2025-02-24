using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }
    private List<Ticket> tickets = new();

    [SerializeField] private GameObject ticketPrefab;
    [SerializeField] private Gradient pleasantGradient;
    [SerializeField] private int maxTicketPosition = 1200;
    [SerializeField] private float ticketSlideTime = 0.25f;

    private bool roomForTickets = true;

    public static Color GetGradientColor(float percentRemaining)
    {
        return Instance.pleasantGradient.Evaluate(percentRemaining);
    }

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

    public static bool CanTakeOrder()
    {
        return Instance.roomForTickets;   
    }
    public static bool CanServeFood()
    {
        return Instance.tickets.Count > 0;
    }

    public static void NewTicket(Burger burger, Customer customer)
    {
        GameObject ticketObj = Instantiate(Instance.ticketPrefab, Instance.transform);
        Ticket ticket = ticketObj.GetComponent<Ticket>();
        ticket.Init(burger, customer);
        Instance.tickets.Add(ticket);
        Instance.UpdateTicketPositions();
    }

    public static void RemoveTicket(Ticket ticket)
    {
        Instance.StartCoroutine(ticket.KillTicket(Instance.ticketSlideTime));
    }
    public static void RemoveTicketFromList(Ticket ticket) // callback
    {
        Instance.tickets.Remove(ticket);
        Instance.UpdateTicketPositions();
    }

    private void UpdateTicketPositions()
    {
        // Tickets pop in from right to left, so oldest ticket is furthest left as well as first in list
        // Iterate through list backwards to ensure oldest ticket is furthest left
        int ticketPosition = 10;
        for(int i = tickets.Count - 1; i >= 0; i--)
        {
            tickets[i].StopAllCoroutines(); // Kill all coroutines so we stop sliding if we're already sliding
            StartCoroutine(tickets[i].SlideTicket(-ticketPosition, ticketSlideTime)); // negative since we're moving left
            ticketPosition += tickets[i].GetWidth() + 10; // Figure out where next ticket should / would go
        }
        roomForTickets = ticketPosition < maxTicketPosition;
    }

    /// <summary>
    /// Finds a customer's ticket -- useful when people get mad and leave
    /// </summary>
    public static Ticket FindTicket(Customer customer)
    {
        foreach(Ticket ticket in Instance.tickets)
        {
            if(ticket.GetCustomer() == customer)
            {
                return ticket;
            }
        }
        return null;
    }

    /// <summary>
    /// Finds all the tickets for a specific order
    /// </summary>
    public static List<Ticket> FindTickets(Burger order)
    {
        List<Ticket> foundTickets = new();
        for(int i = 0; i < Instance.tickets.Count; i++)
        {
            if(Instance.tickets[i].GetOrder() == order)
            {
                foundTickets.Add(Instance.tickets[i]);
            }
        }
        if(foundTickets.Count == 0) return null;

        // sort list so that ticket with highest time spent waiting is first in list
        foundTickets.Sort((a, b) => b.GetCustomer().GetTimeSpentWaitingForOrder().CompareTo(a.GetCustomer().GetTimeSpentWaitingForOrder()));
        return foundTickets;
    }

    public static Ticket LowestTimeLeftTicket()
    {
        if(CanServeFood() == false) return null;
        
        Ticket lowest = Instance.tickets[0];
        for(int i = 1; i < Instance.tickets.Count; i++)
        {
            if(Instance.tickets[i].GetCustomer().GetTimeSpentWaitingForOrder() > lowest.GetCustomer().GetTimeSpentWaitingForOrder())
            {
                lowest = Instance.tickets[i];
            }
        }
        return lowest;
    }
}
