using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OrderGenerator : MonoBehaviour
{
    [SerializeField] private GameObject receiptChunk;
    [SerializeField] private float receiptSizeScale = 1f;
    [SerializeField] private OrderManager ticketManager;

    [Header("Serialize Dictionary -- Ignore")]
    [SerializeField] private List<Sprite> ingredientSprites;
    [SerializeField] private List<Ingredient> ingredientNames;

    private Dictionary<Ingredient, Sprite> ingredientImages;
    private float receiptWidth;

    void Start()
    {
        // Popoulate Dictionary
        ingredientImages = new Dictionary<Ingredient, Sprite>();
        for (int i = 0; i < ingredientSprites.Count; i++)
        {
            ingredientImages.Add(ingredientNames[i], ingredientSprites[i]);
        }

        if (receiptChunk != null)
        {
            receiptWidth = receiptChunk.GetComponent<RectTransform>().rect.width;
        }
    }

    public GameObject GenerateReceipt(Burger burger)
    {
        List<IngredientType> ingredients = burger.ingredients;
        GameObject receiptParent = new GameObject();
        if (ticketManager != null)
        {
            //Determine total width of receipt and apply to preferredWidth of LayoutElement
            float totalReceiptWidth = receiptWidth * receiptSizeScale * ((ingredients.Count / 2) + ingredients.Count % 2);
            LayoutElement layoutElement = receiptParent.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = totalReceiptWidth;

            PopulateReceipt(receiptParent, ingredients);
            ticketManager.AddTicket(receiptParent);
        }
        return receiptParent;
    }

    void PopulateReceipt(GameObject receiptParent, List<IngredientType> ingredients)
    {
        GameObject curReceipt = null;
        // Shift spawn of receipt left half of the receipt width for every receipt over 1
        float startX = (-1 * receiptWidth * receiptSizeScale) * (((ingredients.Count / 2) + ingredients.Count % 2) - 1) / 2;
        Vector2 spawnPosition = new Vector2(startX, 0);

        foreach (IngredientType ingredient in ingredients)
        {
            if (curReceipt == null)
            {
                curReceipt = Instantiate(receiptChunk, transform);
                curReceipt.transform.SetParent(receiptParent.transform);

                RectTransform rectTransform = curReceipt.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = spawnPosition;
                spawnPosition += new Vector2(receiptWidth, 0);
                Debug.Log("Spawn Position: " + spawnPosition);
                Debug.Log("Receipt Width: " + receiptWidth);
            }

            Image topImage = curReceipt.transform.Find("Top Image").GetComponent<Image>();
            Image bottomImage = curReceipt.transform.Find("Bottom Image").GetComponent<Image>();

            /* COMMENTING OUT FOR COMPILE
            
            if (topImage.sprite == null)
            {
                topImage.sprite = ingredientImages[ingredient];
            }
            else
            {
                bottomImage.sprite = ingredientImages[ingredient];
                curReceipt = null;
            }
            */ 
        }
    }



}
