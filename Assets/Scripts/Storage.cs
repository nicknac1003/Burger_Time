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

    protected bool PlaceItem(Holdable item)
    {
        Debug.Log("Trying to place item in " + name);

        if (holdable != null) return false;
        
        Debug.Log("Storage is empty");

        if (item is IngredientObject ingredientObject)
        {
            Debug.Log("Item is ingredient object");

            if(acceptedHoldables.Count > 0) Debug.Log("Holdables are restricted.");
            if(acceptedHoldables.Contains(ingredientObject.Type())) Debug.Log("Item is an accepted holdable.");
            if(acceptedStates.Contains(ingredientObject.State())) Debug.Log("Item is in an accepted state.");

            if (acceptedHoldables.Count > 0 && (!acceptedHoldables.Contains(ingredientObject.Type()) || !acceptedStates.Contains(ingredientObject.State()))) return false;
        }
        else if (!canHoldBurgers && item is BurgerObject)
        {
            Debug.Log("Item is a burger object and storage cannot hold burgers.");
            return false;
        }
        
        Debug.Log("Item is allowed");

        if (item is FireExtinguisher && !canHoldFireExtinguisher) return false;
        if (item is FireExtinguisher fe && canHoldFireExtinguisher) fe.Dropped();
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
        if (holdable is FireExtinguisher fe) fe.Taken();
        Debug.Log("Took " + holdable.name + " from " + name);

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
                PlayerController.Instance.SetHolding(null);
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
                PlayerController.Instance.SetHolding(null);
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
}