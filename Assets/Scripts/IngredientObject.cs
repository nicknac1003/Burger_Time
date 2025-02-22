using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public enum IngredientType : short
{
    Bun,
    Patty,
    Lettuce,
    Tomato,
    Cheese,
    Onion,
    Plate
}

public enum IngredientState : short
{
    Raw,
    Cooked,
    Burnt
}

public enum PrepMethod
{
    None,
    Grill, // patty
    Chop,  // lettuce, onion
    Slice, // cheese, tomato
    Half,  // bun
    Wash   // plate
}

[RequireComponent(typeof(SpriteRenderer))]
public class IngredientObject : Holdable
{
    [Tooltip("Ingredient Data")]
    [SerializeField] private Ingredient ingredient;

    [Tooltip("Preparation method. Used for cooking.")]
    [SerializeField] private PrepMethod prepMethod;

    [Tooltip("Raw / Unprepared")]
    [SerializeField] private Sprite raw;
    [Tooltip("Cooked / Prepared")]
    [SerializeField] private Sprite cooked;
    [Tooltip("Burnt")]
    [SerializeField] private Sprite burnt;
    [Tooltip("Raw / Unprepared sprite for burger")]
    [SerializeField] private Sprite onBurgerRaw;
    [Tooltip("Cooked / Prepared sprite for burger")]
    [SerializeField] private Sprite onBurgerCooked;
    [Tooltip("Burnt sprite for burger")]
    [SerializeField] private Sprite onBurgerBurnt;

    [SerializeField] private float cookTime = 0f;

    public float GetCookTime() => cookTime;
    public float UpdateCookTime(float added) => cookTime += added;

    private SpriteRenderer spriteRenderer;
    private bool onBurger = false;

    public static IngredientObject Instantiate(Ingredient data)
    {
        GameObject obj = data.Type() switch
        {
            IngredientType.Bun     => Instantiate(GlobalConstants.bun.gameObject),
            IngredientType.Patty   => Instantiate(GlobalConstants.patty.gameObject),
            IngredientType.Lettuce => Instantiate(GlobalConstants.lettuce.gameObject),
            IngredientType.Tomato  => Instantiate(GlobalConstants.tomato.gameObject),
            IngredientType.Cheese  => Instantiate(GlobalConstants.cheese.gameObject),
            IngredientType.Onion   => Instantiate(GlobalConstants.onion.gameObject),
            IngredientType.Plate   => Instantiate(GlobalConstants.plate.gameObject),
            _ => null
        };

        if(obj == null)
        {
            Debug.LogError("Could not instantiate " + data + ". Does the ingredient exist in Resources/Ingredients? Is it being loaded in GlobalConstants?");
            return null;
        }

        IngredientObject ingredientObject = obj.GetComponent<IngredientObject>();
        ingredientObject.ingredient = data;
        ingredientObject.ChangeState(data.State());

        return ingredientObject;
    }

    public IngredientType  Type()  => ingredient.Type();
    public IngredientState State() => ingredient.State();
    public Ingredient      GetIngredient() => ingredient;

    public static Sprite GetSprite(IngredientType t, IngredientState s)
    {
        return t switch
        {
            IngredientType.Bun     => GlobalConstants.bun.GetSprite(s),
            IngredientType.Patty   => GlobalConstants.patty.GetSprite(s),
            IngredientType.Lettuce => GlobalConstants.lettuce.GetSprite(s),
            IngredientType.Tomato  => GlobalConstants.tomato.GetSprite(s),
            IngredientType.Cheese  => GlobalConstants.cheese.GetSprite(s),
            IngredientType.Onion   => GlobalConstants.onion.GetSprite(s),
            IngredientType.Plate   => GlobalConstants.plate.GetSprite(s),
            _ => null,
        };
    }
    public Sprite GetSprite(IngredientState s)
    {
        return s switch
        {
            IngredientState.Raw    => raw,
            IngredientState.Cooked => cooked,
            IngredientState.Burnt  => burnt,
            _ => null,
        };
    }

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        ChangeState(ingredient.State()); // update sprite
    }

    public void PutOnBurger()
    {
        onBurger = true;
        UpdateSprite();
    }

    public void ChangeState(IngredientState newState)
    {
        ingredient.SetState(newState);
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        switch(ingredient.State())
        {
            case IngredientState.Raw:
                spriteRenderer.sprite = onBurger ? onBurgerRaw : raw;
                break;
            case IngredientState.Cooked:
                spriteRenderer.sprite = onBurger ? onBurgerCooked : cooked;
                break;
            case IngredientState.Burnt:
                spriteRenderer.sprite = onBurger ? onBurgerBurnt : burnt;
                break;
        }
    }
    
}

[System.Serializable]
public class Ingredient
{
    public static int ingredientCount = System.Enum.GetValues(typeof(IngredientType)).Length;
    public static List<IngredientType> ingredientList = System.Enum.GetValues(typeof(IngredientType)).Cast<IngredientType>().ToList();

    [SerializeField] private IngredientType  type;
    [SerializeField] private IngredientState state;

    public Ingredient(IngredientType t, IngredientState s)
    {
        type  = t;
        state = s;
    }

    public IngredientState State() => state;
    public IngredientType  Type()  => type;

    public Sprite GetSprite()
    {
        return IngredientObject.GetSprite(type, state);
    }

    public override string ToString()
    {
        return state + " " + type;
    }

    public void SetState(IngredientState newState)
    {
        state = newState;
    }
    public void SetType(IngredientType newType)
    {
        type = newType;
    }

    public override bool Equals(object other)
    {
        if(ReferenceEquals(this, other)) return true;

        if(other == null || other is not Ingredient) return false;

        Ingredient comp = (Ingredient)other;

        return state == comp.state && type == comp.type;
    }
    public static bool operator ==(Ingredient a, Ingredient b)
    {
        return a.Equals(b);
    }
    public static bool operator !=(Ingredient a, Ingredient b)
    {
        return !(a == b);
    }
    public override int GetHashCode()
    {
        return ((int)type << 16) | (int)state;
    }
}