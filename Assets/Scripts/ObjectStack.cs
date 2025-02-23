using System.Collections.Generic;
using UnityEngine;

public class ObjectStack : Interactable
{
    [SerializeField] private Ingredient ingredient;
    [SerializeField] private int startingCount;
    [SerializeField] private float spacing;

    private List<IngredientObject> inStack = new();

    protected override void OnZ()
    {
        if(PlayerController.HoldingItem())
        {
            Holdable playerHolding = PlayerController.GetItem();
            if(playerHolding is IngredientObject ingredientObject && ingredientObject.GetIngredient() == ingredient)
            {
                ingredientObject.transform.SetParent(transform);
                ingredientObject.transform.localPosition = new Vector3(0, inStack.Count * spacing, 0);
                inStack.Add(ingredientObject); // add after setting position to account for 0-indexing
                PlayerController.SetHolding(null);
            }
            return;
        }

        if(inStack.Count == 0) return;

        IngredientObject topOfStack = inStack[inStack.Count - 1];
        PlayerController.GrabItem(topOfStack);
        inStack.Remove(topOfStack);
    }

    void Awake()
    {
        for(int i = 0; i < startingCount; i++)
        {
            AddToStack();
        }
    }

    public void AddToStack() // creates a new object to add to stack
    {
        IngredientObject ingredientObject = IngredientObject.Instantiate(ingredient, transform);
        ingredientObject.transform.localPosition = new Vector3(0, inStack.Count * spacing, 0);
        inStack.Add(ingredientObject);
    }
}
