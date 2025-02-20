using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Image))]
public class Ticket : MonoBehaviour
{
    [SerializeField] private Burger   order;
    [SerializeField] private Customer customer;

    public void SetOrder(Burger burger)      => order    = burger;
    public void SetCustomer(Customer person) => customer = person;

    private RectTransform timer;
    private Transform ingredients;
    
    private int width;

    void Awake()
    {
        timer = transform.Find("Timer").GetComponent<RectTransform>();
        ingredients = transform.Find("Ingredients");
    }

    public void Init(Burger burger, Customer person)
    {
        order    = burger;
        customer = person;

        List<Ingredient> ingredientList = order.GetIngredients();
        int ingredientCount = ingredientList.Count;

        width = 10 + 85 * ingredientCount;
        GetComponent<RectTransform>().sizeDelta = new Vector2(width, 200);

        // Add ingredients to horizontal layout group
        for(int i = 0; i < ingredientCount; i++)
        {
            GameObject ingredientSlot = new(ingredientList[i] + " Slot");
            ingredientSlot.transform.SetParent(ingredients);
            ingredientSlot.AddComponent<Image>().sprite = ingredientList[i].GetSprite();
        }
    }

    void Update()
    {
        float percentRemaining = (CustomerManager.MaxWaitTime() - customer.GetTimeSpentWaitingForOrder()) / CustomerManager.MaxWaitTime();

        timer.offsetMax = new(-Mathf.Lerp(width, 10, percentRemaining), timer.offsetMax.y); // Shrink timer bar to the left

        // Change color based on time remaining?
    }

    public IEnumerator Expand(float time)
    {
        float timer = 0f;
        while(timer < time)
        {
            EaseFunctions.Interpolate(120, width, time / timer, EaseFunctions.Ease.OutQuadratic); // Change width from 120 to width (the variable)
            EaseFunctions.Interpolate(0, 10, time / timer, EaseFunctions.Ease.OutQuadratic); // Change spacing of ingredients from 0 to 10

            time += Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator Shrink(float time)
    {
        float timer = 0f;
        while(timer < time)
        {
            EaseFunctions.Interpolate(width, 120, time / timer, EaseFunctions.Ease.OutQuadratic); // Change width from width (the variable) to 120
            EaseFunctions.Interpolate(10, 0, time / timer, EaseFunctions.Ease.OutQuadratic); // Change spacing of ingredients from 10 to 0

            time += Time.deltaTime;
            yield return null;
        }
    }
}