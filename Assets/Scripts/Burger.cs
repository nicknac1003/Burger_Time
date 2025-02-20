using System.Collections.Generic;
using System.Linq;
using NUnit.Compatibility;
using UnityEngine;

[System.Serializable]
public class Burger
{
    public List<IngredientType> ingredients;

    public Burger()
    {
        ingredients = new List<IngredientType>();
    }

    public void Add(IngredientType ingredient)
    {
        ingredients.Add(ingredient);
    }
    public void Remove(IngredientType ingredient)
    {
        ingredients.Remove(ingredient);
    }
    public bool Contains(IngredientType ingredient)
    {
        return ingredients.Contains(ingredient);
    }
    public void Clear()
    {
        ingredients.Clear();
    }

    public string GetBurgerString()
    {
        string burgerString = "";
        foreach (IngredientType ingredient in ingredients)
        {
            burgerString += ingredient.ToString() + " ";
        }
        return burgerString;
    }
    
    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        Burger other = (Burger)obj;

        var thisIngredientCounts  = ingredients.GroupBy(i => i).ToDictionary(g => g.Key, g => g.Count());
        var otherIngredientCounts = other.ingredients.GroupBy(i => i).ToDictionary(g => g.Key, g => g.Count());

        return thisIngredientCounts.Count == otherIngredientCounts.Count && thisIngredientCounts.All(pair => otherIngredientCounts.TryGetValue(pair.Key, out int count) && count == pair.Value);
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (var ingredient in ingredients)
        {
            hash = hash * 31 + ingredient.GetHashCode();
        }
        return hash;
    }

    public static bool operator ==(Burger a, Burger b)
    {
        if (ReferenceEquals(a, b)) {
            return true;
        }

        if (a is null || b is null) {
            return false;
        }

        return a.Equals(b);
    }

    public static bool operator !=(Burger a, Burger b)
    {
        return !(a == b);
    }

    public static Burger GenerateRandomBurger(int toppings)
    {
        Burger burger = new();

        burger.Add(IngredientType.Bun);   // must include Bun
        burger.Add(IngredientType.Patty); // must include Patty (at least 1)

        List<IngredientType> options = Ingredient.ingredientList;
        options.Remove(IngredientType.Bun);

        for (int i = 0; i < toppings; i++)
        {
            burger.Add(options[Random.Range(0, options.Count)]);
        }

        return burger;
    }
}