using System.Collections.Generic;
using System.Linq;
using UnityEngine;



[System.Serializable]
public class Burger {
    public List<Ingredient> ingredients;
    public Burger() {
        ingredients = new List<Ingredient>();
    }
    public void AddIngredient(Ingredient ingredient) {
        ingredients.Add(ingredient);
    }
    public void RemoveIngredient(Ingredient ingredient) {
        ingredients.Remove(ingredient);
    }
    public bool ContainsIngredient(Ingredient ingredient) {
        return ingredients.Contains(ingredient);
    }
    public void ClearBurger() {
        ingredients.Clear();
    }
    public string GetBurgerString() {
        string burgerString = "";
        foreach (Ingredient ingredient in ingredients) {
            burgerString += ingredient.ToString() + " ";
        }
        return burgerString;
    }
        public override bool Equals(object obj) {
        if (obj == null || GetType() != obj.GetType()) {
            return false;
        }

        Burger other = (Burger)obj;
        var thisIngredientCounts = ingredients.GroupBy(i => i).ToDictionary(g => g.Key, g => g.Count());
        var otherIngredientCounts = other.ingredients.GroupBy(i => i).ToDictionary(g => g.Key, g => g.Count());

        return thisIngredientCounts.Count == otherIngredientCounts.Count &&
               thisIngredientCounts.All(pair => otherIngredientCounts.TryGetValue(pair.Key, out int count) && count == pair.Value);
    }

    public override int GetHashCode() {
        int hash = 17;
        foreach (var ingredient in ingredients) {
            hash = hash * 31 + ingredient.GetHashCode();
        }
        return hash;
    }

    public static bool operator ==(Burger lhs, Burger rhs) {
        if (ReferenceEquals(lhs, rhs)) {
            return true;
        }

        if (lhs is null || rhs is null) {
            return false;
        }

        return lhs.Equals(rhs);
    }

    public static bool operator !=(Burger lhs, Burger rhs) {
        return !(lhs == rhs);
    }

    public static Burger GenerateRandomBurger(int numIngredients, List<Ingredient> ingredientOptions) {
        Burger burger = new Burger();

        burger.AddIngredient(ingredientOptions[0]); //must include Bun
        burger.AddIngredient(ingredientOptions[1]); //must include Patty (atleast 1)

        for (int i = 0; i < numIngredients; i++)
        {
            burger.AddIngredient(ingredientOptions[Random.Range(1, ingredientOptions.Count)]);
        }

        return burger;
    }
}