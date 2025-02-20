using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

[System.Serializable]
public class SpriteSpace {
    public Sprite sprite;
    public float spacing;
    public SpriteSpace(Sprite sprite, float spacing)
    {
        this.sprite = sprite;
        this.spacing = spacing;
    }
}
public class BurgerRenderer : MonoBehaviour
{
    public Transform topbun;
    public Transform bottombun;
    public Transform ingredientsParent;
    public Transform plate;
    public List<Ingredient> ingredientNames;
    public List<Sprite> ingredientSprites;
    private Dictionary<Ingredient, Sprite> ingredientImages;
    public float nextIngredientPosition = 0;
    public float spacing = 0.1f;

    private Burger burger;
    private void Start()
    {
        // Popoulate Dictionary
        burger = new Burger();
        AddItem(Ingredient.Bun, true);
        ingredientImages = new Dictionary<Ingredient, Sprite>();
        for (int i = 0; i < ingredientSprites.Count; i++)
        {
            ingredientImages.Add(ingredientNames[i], ingredientSprites[i]);
        }
    }

    public void AddItem(Ingredient ingredient, bool addPlate=false)
    {
        Debug.Log("adding ingredient: " + ingredient);
        if (addPlate){
            if (plate != null && !plate.gameObject.activeSelf){
                plate.gameObject.SetActive(true);
            }
        }
        if (ingredient == Ingredient.Bun){
            if (topbun != null && !topbun.gameObject.activeSelf){
                topbun.gameObject.SetActive(true);
            }
            if (bottombun != null && !bottombun.gameObject.activeSelf){
                bottombun.gameObject.SetActive(true);
            }

        } else {
            GameObject ingredientObject = new GameObject();
            SpriteRenderer sr = ingredientObject.AddComponent<SpriteRenderer>();
            sr.sprite = ingredientImages[ingredient];
            ingredientObject.transform.SetParent(ingredientsParent);
            ingredientObject.transform.localPosition = new Vector3(0, nextIngredientPosition, 0);
            ingredientObject.transform.localScale = new Vector3(1f, 1f, 1f);
            //move topbun transfor in y by spacing
            if (burger.ingredients.Count > 1)
            {
                topbun.transform.localPosition = new Vector3(0, topbun.transform.localPosition.y + spacing, 0);
            }
            nextIngredientPosition += spacing;

        }
        burger.AddIngredient(ingredient);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)){
            AddItem(ingredientNames[Random.Range(0, ingredientNames.Count)]);
        }
    }



    public void Trash()
    {
        
    }
}


//.03