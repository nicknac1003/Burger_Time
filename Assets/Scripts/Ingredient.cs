using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public enum IngredientType
{
    Bun,
    Patty,
    Lettuce,
    Tomato,
    Cheese,
    Onion
}

public class Ingredient : Holdable
{
    public static int ingredientCount = System.Enum.GetValues(typeof(IngredientType)).Length;
    public static List<IngredientType> ingredientList = System.Enum.GetValues(typeof(IngredientType)).Cast<IngredientType>().ToList();

    [Tooltip("Ingredient type. Used for comparison.")]
    [SerializeField] private IngredientType ingredient;

    [Tooltip("Raw / Unprepared")]
    [SerializeField] private Sprite raw;
    [Tooltip("Cooked / Prepared")]
    [SerializeField] private Sprite cooked;
    [Tooltip("Burnt / Null if no burnt state")]
    [SerializeField] private Sprite burnt;
}