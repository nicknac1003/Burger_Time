using UnityEngine;

public class Trash : Interactable
{
    [SerializeField] private Ingredient ingredient;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip throwAwaySound;
    [Range(0, 1)]
    [SerializeField] private float volume = 0.5f;
    protected override void OnZ()
    {
        if (PlayerController.HoldingItem())
        {
            Holdable playerHolding = PlayerController.GetItem();
            if (playerHolding is BurgerObject burgerObject)
            {
                if (burgerObject.GetBurger().Contains(IngredientType.Plate) && PlayerController.DestroyItem())
                {
                    IngredientObject plate = IngredientObject.Instantiate(ingredient);
                    plate.ChangeState(IngredientState.Cooked);
                    PlayerController.GrabItem(plate);
                    PlayThrowAwaySound();
                }
            }
            else
            {
                if (playerHolding is IngredientObject ingredientObject && ingredientObject.Type() != IngredientType.Plate && PlayerController.DestroyItem()) //save the plates!
                {
                    PlayThrowAwaySound();
                }
                if (playerHolding is FireExtinguisher fireExtinguisher && PlayerController.DestroyItem())
                {
                    PlayThrowAwaySound();
                }
            }
        }
    }
    private void PlayThrowAwaySound()
    {
        audioSource.PlayOneShot(throwAwaySound, volume);
    }
}