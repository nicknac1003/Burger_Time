using UnityEngine;

public class IngredientCrate : Interactable
{
    public IngredientType ingredientType;

    public static Animation pickupAnimation;

    public AudioSource pickupSound;
    public float ingredientScale = 0.75f;

    protected override void OnZ()
    {
        if (PlayerController.HoldingItem()) return;
        PlayerController.LockPlayer();
        IngredientObject ingredient = IngredientObject.Instantiate(new Ingredient(ingredientType, IngredientState.Raw), transform.position);
        ingredient.transform.localScale = new Vector3(ingredientScale, ingredientScale, ingredientScale);
        ingredient.StartMovementAnimation(PlayerController.Instance.holdAnchor.transform, 0.5f, 1f);
        PlayerController.GrabItem(ingredient);

        // pickupAnimation.Play();
        // AudioSource.PlayClipAtPoint(pickupSound, transform.position);
    }
}