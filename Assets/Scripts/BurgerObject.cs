using System.Collections.Generic;
using System.Linq;
using NUnit.Compatibility;
using UnityEngine;

[System.Serializable]
public class SpriteSpace
{
    public Sprite sprite;
    public float spacing;
    public SpriteSpace(Sprite sprite, float spacing)
    {
        this.sprite  = sprite;
        this.spacing = spacing;
    }
}

public class BurgerObject : Holdable
{
    [SerializeField] private Burger burger;
    private List<IngredientObject> ingredientsOnBurger = new();

    public Burger GetBurger() => burger;

    // Players can create a burger by combining any 2 ingredients
    // Burgers can only have 1 bun and 1 plate, never more
    // Burgers can only be added to. Ingredients cannot be removed once added.

    public float spacing = 0.1f;

    /// <summary>
    /// Adds an IngredientObject to the BurgerObject
    /// </summary>
    /// <param name="ingredientObj"></param>
    /// <returns>
    /// True if ingredient was added, false if not
    /// </returns>
    public bool Add(IngredientObject ingredientObj)
    {
        Ingredient ingredient = ingredientObj.GetIngredient();

        if(ingredient.State() == IngredientState.Raw) return false;

        if(ingredient.Type() == IngredientType.Plate)
        {
            if(burger.Contains(ingredient)) return false;
            
            ingredientsOnBurger.Insert(0, ingredientObj); // Plate ALWAYS goes on the bottom of list
        }
        else if(ingredient.Type() == IngredientType.Bun)
        {
            if(burger.Contains(ingredient)) return false;

            ingredientsOnBurger.Insert(burger.Contains(IngredientType.Plate) ? 1 : 0, ingredientObj); // Bun 1 ALWAYS goes above plate in list - cooked = bottom bun

            IngredientObject secondBun = IngredientObject.Instantiate(new Ingredient(IngredientType.Bun, IngredientState.Burnt));
            ingredientsOnBurger.Add(secondBun); // Bun 2 ALWAYS goes at the top of list - burnt = top bun

            secondBun.PutOnBurger();
            secondBun.transform.SetParent(transform);
            secondBun.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            ingredientsOnBurger.Insert(ingredientsOnBurger.Count - 1, ingredientObj); // Add ingredient before top bun
        }

        ingredientObj.PutOnBurger();
        ingredientObj.transform.SetParent(transform);
        ingredientObj.transform.localScale = new Vector3(1f, 1f, 1f);

        Debug.Log("Adding " + ingredientObj + " to " + this);
        burger.Add(ingredient);

        UpdateBurger();

        return true;
    }

    private void UpdateBurger()
    {
        for(int i = 0; i < ingredientsOnBurger.Count; i++)
        {
            ingredientsOnBurger[i].transform.localPosition = new Vector3(0, i * spacing, 0);
        }
    }

    // DEBUG ONLY
    void Awake()
    {
        List<Ingredient> workingList = new(burger.GetIngredients());
        burger = new Burger();

        foreach(Ingredient ingredient in workingList)
        {
            Debug.Log("Adding " + ingredient + " to " + this);
            if(Add(IngredientObject.Instantiate(ingredient)) == false)
            {
                Debug.LogError("Failed to add " + ingredient + " to " + this);
            }
        }   
    }
}

[System.Serializable]
public class Burger
{
    [SerializeField] private List<Ingredient> ingredients;

    public List<Ingredient> GetIngredients() => ingredients;

    public Burger()
    {
        ingredients = new();
    }

    /// <summary>
    /// Adds an ingredient to the burger ingredient list
    /// </summary>
    /// <param name="ingredient"></param>
    public void Add(Ingredient ingredient)
    {
        ingredients.Add(ingredient);
    }

    /// <summary>
    /// Removes an ingredient from the burger ingredient list. Debug only.
    /// </summary>
    /// <param name="ingredient"></param>
    public void Remove(Ingredient ingredient)
    {
        ingredients.Remove(ingredient);
    }

    /// <summary>
    /// Check if the burger contains a specific ingredient at specific state
    /// </summary>
    /// <param name="ingredient"></param>
    /// <returns></returns>
    public bool Contains(Ingredient ingredient)
    {
        return ingredients.Contains(ingredient);
    }

    /// <summary>
    /// Check if the burger contains a specific ingredient type, regardless of state
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool Contains(IngredientType type)
    {
        return ingredients.Any(ingredient => ingredient.Type() == type);
    }

    /// <summary>
    /// Generate a random burger with the specified number of toppings
    /// </summary>
    /// <param name="toppings"></param>
    /// <returns></returns>
    public static Burger GenerateRandomBurger(int toppings)
    {
        Burger burger = new();

        burger.Add(new Ingredient(IngredientType.Bun, IngredientState.Cooked));   // must include Bun
        burger.Add(new Ingredient(IngredientType.Plate, IngredientState.Cooked)); // must include Plate

        if(Random.Range(0f, 1f) > 0.05f) // 5% chance of no patty, 19% chance of burnt patty, 76% chance of cooked patty
        {
            burger.Add(new Ingredient(IngredientType.Patty, Random.Range(0f, 1f) > 0.2f ? IngredientState.Cooked : IngredientState.Burnt));
        }

        List<IngredientType> options = Ingredient.ingredientList;
        options.Remove(IngredientType.Bun);
        options.Remove(IngredientType.Plate);

        for (int i = 0; i < toppings; i++)
        {
            IngredientType topping = options[Random.Range(0, options.Count)];
            
            IngredientState state = topping != IngredientType.Patty ? IngredientState.Cooked : Random.Range(0f, 1f) > 0.2f ? IngredientState.Cooked : IngredientState.Burnt;

            burger.Add(new Ingredient(topping, state));
        }

        return burger;
    }

    public override bool Equals(object other)
    {
        if(ReferenceEquals(this, other)) return true;

        if(other == null || other is not Burger) return false;

        Burger comp = (Burger)other;

        // Check if burger a has the same ingredient count (early exit)
        if(ingredients.Count == comp.ingredients.Count) return false;

        // Now we have to check and make sure burger a has the same ingredients at the same states as burger b. Find a match and remove from working list.
        List<Ingredient> workingList = new(comp.ingredients);
        foreach (Ingredient ingredient in ingredients)
        {
            bool fail = true;
            foreach (Ingredient compIng in workingList)
            {
                if (ingredient == compIng) // comparison for type & state happen in the Ingredient class
                {
                    fail = false;
                    workingList.Remove(compIng);
                    break;
                }
            }
            if (fail) return false;
        }
        return true;
    }
    public static bool operator ==(Burger a, Burger b)
    {
        return a.Equals(b);
    }
    public static bool operator !=(Burger a, Burger b)
    {
        return !(a == b);
    }
    public override int GetHashCode()
    {
        // Each ingredient has its own hash. Put into list and use list's hash.
        List<int> hashCodes = new();
        foreach(Ingredient ingredient in ingredients)
        {
            hashCodes.Add(ingredient.GetHashCode());
        }
        return hashCodes.GetHashCode();
    }
}