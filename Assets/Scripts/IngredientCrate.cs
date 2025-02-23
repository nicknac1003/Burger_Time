using UnityEngine;

public class IngredientCrate: Interactable {
    public IngredientType ingredientType;

    public static Animation pickupAnimation;

    public AudioSource pickupSound;

    protected override void OnZ() {
        if(PlayerController.HoldingItem()) return;

        IngredientObject ingredient = IngredientObject.Instantiate(new Ingredient(ingredientType, IngredientState.Raw));
        // ingredient.transform.localScale = new Vector3(0.75f, 0.75f * Mathf.Sqrt(2), 0.75f);
        Debug.Log(ingredient.transform.localScale);

        PlayerController.GrabItem(ingredient);

        // pickupAnimation.Play();
        // AudioSource.PlayClipAtPoint(pickupSound, transform.position);
    }
}