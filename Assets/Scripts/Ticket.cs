using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(Image))]
public class Ticket : MonoBehaviour
{
    [SerializeField] private Burger   order;
    [SerializeField] private Customer customer;

    public void SetOrder(Burger burger)      => order    = burger;
    public void SetCustomer(Customer person) => customer = person;

    private RectTransform rectTransform;
    private RectTransform timerRect;
    private RectTransform ingredientsRect;
    private RectTransform burgerObjectRect;
    
    private int width;

    public int      GetWidth()    => width;
    public Customer GetCustomer() => customer;
    public Burger   GetOrder()    => order;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        timerRect = transform.Find("Timer").GetComponent<RectTransform>();
        ingredientsRect = transform.Find("Ingredients").GetComponent<RectTransform>();
        burgerObjectRect = transform.Find("Burger").GetComponent<RectTransform>();
    }

    public void Init(Burger burger, Customer person)
    {
        order    = burger;
        customer = person;

        // Remove plates from list
        List<Ingredient> ingredientList = order.GetIngredients().Where(ingredient => ingredient.Type() != IngredientType.Plate).ToList();
        int ingredientCount = ingredientList.Count;

        width = 120 + 55 * Mathf.CeilToInt(ingredientCount / 2f);
        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);

        // Start ticket off screen
        rectTransform.anchoredPosition = new Vector2(width, 0);

        // Populate burger
        GameObject UIBurger = order.DrawUIBurger();
        UIBurger.transform.SetParent(burgerObjectRect);
        UIBurger.transform.localPosition = new Vector3(0, 0, 0);
        UIBurger.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40);

        // Add ingredients to grid layout
        for(int i = 0; i < ingredientCount; i++)
        {
            GameObject ingredientSlot = new(ingredientList[i] + " Slot");
            ingredientSlot.transform.SetParent(ingredientsRect);
            ingredientSlot.AddComponent<Image>().sprite = ingredientList[i].GetSprite();
        }
    }

    void Update()
    {
        float percentRemaining = 1 - (customer.GetTimeSpentWaitingForOrder() / customer.GetMaxOrderTime());

        timerRect.offsetMax = new(-Mathf.Lerp(width, 10, percentRemaining), timerRect.offsetMax.y);  // Shrink timer bar to the left
        timerRect.GetComponent<Image>().color = OrderManager.GetGradientColor(1 - percentRemaining); // Change color of timer bar
    }

    public IEnumerator SlideTicket(float newX, float slideTime)
    {
        float oldX = rectTransform.anchoredPosition.x;

        for(float t = 0; t < slideTime; t += Time.deltaTime)
        {
            rectTransform.anchoredPosition = new(EaseFunctions.Interpolate(oldX, newX, t / slideTime, EaseFunctions.Ease.OutBack), rectTransform.anchoredPosition.y);
            yield return null;
        }
        rectTransform.anchoredPosition = new(newX, rectTransform.anchoredPosition.y);
    }

    public IEnumerator KillTicket(float leaveTime)
    {
        float oldY = rectTransform.anchoredPosition.y;

        for(float t = 0; t < leaveTime; t += Time.deltaTime)
        {
            rectTransform.anchoredPosition = new(rectTransform.anchoredPosition.x, EaseFunctions.Interpolate(oldY, 150, t / leaveTime, EaseFunctions.Ease.InBack));
            yield return null;
        }

        OrderManager.RemoveTicketFromList(this);

        Destroy(gameObject);
    }
}