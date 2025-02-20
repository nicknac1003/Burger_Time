using UnityEngine;

public class Ticket : MonoBehaviour
{
    [SerializeField] private Burger   order;
    [SerializeField] private Customer customer;

    public void SetOrder(Burger burger)      => order    = burger;
    public void SetCustomer(Customer person) => customer = person;
}