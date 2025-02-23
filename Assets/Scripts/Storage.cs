using UnityEngine;
using System.Collections.Generic;

public class Storage : Interactable
{
    [Tooltip("The list of holdable items that can be stored in this storage.")]
    [SerializeField] private List<IngredientType> acceptedHoldables = new();

    [Tooltip("The list of states that the items above can be.")]
    [SerializeField] private List<IngredientState> acceptedStates = new();

    [Tooltip("The anchor point where the holdable item will be placed.")]
    [SerializeField] private Transform anchor;

    [SerializeField] private bool canHoldBurgers = false;

    [SerializeField] protected bool canHoldFireExtinguisher = false;

    protected Holdable holdable;

    protected override void OnZ()
    {
        Debug.Log("Z pressed on " + name);
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
                Debug.Log("Took item from " + name);
                // visual feedback for taking item?
            }
        }
    }

    protected virtual bool PlaceItem(Holdable item)
    {
        if (holdable != null) return false;

        if (item is IngredientObject ingredientObject)
        {
            if (acceptedHoldables.Count > 0 && (!acceptedHoldables.Contains(ingredientObject.Type()) || !acceptedStates.Contains(ingredientObject.State()))) return false;
            if (acceptedHoldables.Count > 0 && !acceptedHoldables.Contains(ingredientObject.Type()))
                return false;

            if (ingredientObject.isMoving) return false;
        }
        else if (!canHoldBurgers && item is BurgerObject) return false;
        if (item is FireExtinguisher fet && !canHoldFireExtinguisher) return false;
        if (item is FireExtinguisher fe && canHoldFireExtinguisher) fe.Dropped(this);
        holdable = item;
        holdable.transform.SetParent(anchor.transform);
        holdable.transform.localPosition = Vector3.zero;
        PlayerController.SetHolding(null);

        return true;
    }

    protected virtual bool TakeItem()
    {
        Debug.Log("Trying to take item from " + name);
        if (holdable == null) return false;
        Debug.Log("Item is not null");
        if (PlayerController.HoldingItem()) return false;
        Debug.Log("Player is not holding an item");
        if (PlayerController.GrabItem(holdable) == false) return false;
        Debug.Log("Player grabbed item");
        if (holdable is FireExtinguisher fe) fe.Taken();

        holdable = null;

        return true;
    }

    protected void TryCombine(Holdable playerHolding)
    {
        if (holdable is BurgerObject burger && playerHolding is IngredientObject ingredient)
        {
            if (burger.CanAdd(ingredient))
            {
                burger.Add(ingredient);
                PlayerController.SetHolding(null);
            }
        }

        if (holdable is IngredientObject ingredient1 && playerHolding is IngredientObject ingredient2 && canHoldBurgers)
        {
            GameObject burgerObject = new("Burger", typeof(BurgerObject));
            burgerObject.transform.SetParent(anchor);
            burgerObject.transform.localPosition = Vector3.zero;

            BurgerObject newBurger = burgerObject.GetComponent<BurgerObject>();
            if (newBurger.CanAdd(ingredient1) && newBurger.CanAdd(ingredient2))
            {
                newBurger.Add(ingredient1);
                newBurger.Add(ingredient2);
                PlayerController.SetHolding(null);
                holdable = newBurger;
            }
            else
            {
                Destroy(burgerObject);
            }
        }

        if (holdable is IngredientObject ingredient3 && playerHolding is BurgerObject burger2)
        {
            if (burger2.CanAdd(ingredient3))
            {
                burger2.Add(ingredient3);
                holdable = null;
            }
        }
    }
    public void SetItem(Holdable item)
    {
        if (holdable != null) return;
        holdable = item;
        holdable.transform.SetParent(anchor.transform);
        holdable.transform.localPosition = Vector3.zero;
    }
    public Holdable GetItem()
    {
        return holdable;
    }
}