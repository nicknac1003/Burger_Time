using UnityEngine;
using System.Collections.Generic;

public class Storage : Interactable
{
    [Tooltip("The list of holdable items that can be stored in this storage.")]
    [SerializeField] private List<IngredientType> acceptedHoldables = new();

    [Tooltip("The anchor point where the holdable item will be placed.")]
    [SerializeField] private Transform anchor;

    [SerializeField] private bool canHoldBurgers = false;
    protected Holdable holdable;

    protected override void OnZ()
    {
        if (PlayerController.HoldingItem())
        {
            Holdable playerHolding = PlayerController.GetItem();
            if (holdable != null)
            {
                TryCombine(playerHolding);
            }
            if (PlaceItem(playerHolding))
            {
                // visual feedback for placing item?
            }
        }
        else
        {
            if (TakeItem())
            {
                // visual feedback for taking item?
            }
        }
    }


    private bool PlaceItem(Holdable item)
    {
        if (holdable != null) return false;
        Debug.Log(item is IngredientObject);
        if (item is IngredientObject ingredientObject)
        {
            if (acceptedHoldables.Count > 0 && !acceptedHoldables.Contains(ingredientObject.Type()))
                return false;
        }
        else if (!canHoldBurgers) return false;

        holdable = item;
        holdable.transform.SetParent(anchor.transform);
        holdable.transform.localPosition = Vector3.zero;
        PlayerController.Instance.SetHolding(null);

        Debug.Log("Placed " + holdable.name + " in " + name);

        return true;
    }

    protected virtual bool TakeItem()
    {
        if (holdable == null) return false;
        if (PlayerController.HoldingItem()) return false;
        if (PlayerController.GrabItem(holdable) == false) return false;
        Debug.Log("Took " + holdable.name + " from " + name);

        holdable = null;

        return true;
    }

    private void TryCombine(Holdable playerHolding)
    {
        if (holdable is BurgerObject burger && playerHolding is IngredientObject ingredient)
        {
            if (burger.Add(ingredient))
                PlayerController.Instance.SetHolding(null);
        }
        if (holdable is IngredientObject ingredient1 && playerHolding is IngredientObject ingredient2 && canHoldBurgers)
        {
            GameObject gameObject = new GameObject("Burger", typeof(BurgerObject));
            gameObject.transform.SetParent(anchor);
            gameObject.transform.localPosition = Vector3.zero;
            BurgerObject newBurger = gameObject.GetComponent<BurgerObject>();
            if (newBurger.Add(ingredient1) && newBurger.Add(ingredient2))
            {
                PlayerController.Instance.SetHolding(null);
                holdable = newBurger;
            }
        }
        if (holdable is IngredientObject ingredient3 && playerHolding is BurgerObject burger2)
        {
            if (burger2.Add(ingredient3))
            {
                holdable = null;
            }
        }
    }
}