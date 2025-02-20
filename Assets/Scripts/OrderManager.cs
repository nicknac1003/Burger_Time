using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }
    private List<Ticket> tickets;

    [SerializeField] private GameObject ticketPrefab;
    [SerializeField] private Gradient pleasantGradient;
    [SerializeField] private int maxTickets;

    private Ticket activeTicket;

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
        return Instance.tickets.Count < Instance.maxTickets;
    }

    public static void NewTicket(Burger burger, Customer customer)
    {
        GameObject ticketObj = Instantiate(Instance.ticketPrefab); // tweak this
        Ticket ticket = ticketObj.GetComponent<Ticket>();
        ticket.Init(burger, customer);
        Instance.tickets.Add(ticket);
    }

    public static void OpenTickets()
    {
        Debug.Log("Opening Tickets");
        PlayerController.LockPlayer();
    }

    public static void CloseTickets()
    {
        Debug.Log("Closing Tickets");
        PlayerController.UnlockPlayer();
    }
}
