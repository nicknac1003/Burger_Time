using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class ReceiptGenerator : MonoBehaviour
{
    public List<string> ingredients;
    [SerializeField] private GameObject receiptChunk;
    [SerializeField] public float receiptSizeScale = 1f;
    [SerializeField] private TicketManager ticketManager;

    [Header("Serialize Dictionary -- Ignore")]
    [SerializeField] public List<Sprite> ingredientSprites;
    [SerializeField] public List<string> ingredientNames;

    private Dictionary<string, Sprite> ingredientImages;
    private float receiptWidth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Popoulate Dictionary
        ingredientImages = new Dictionary<string, Sprite>();
        for (int i = 0; i < ingredientSprites.Count; i++)
        {
            ingredientImages.Add(ingredientNames[i], ingredientSprites[i]);
        }

        if (receiptChunk != null)
        {
            receiptWidth = receiptChunk.GetComponent<RectTransform>().rect.width;
        }
    }

    public void GenerateReceipt(List<string> ingredients)
    {
        if (ticketManager != null)
        {
            GameObject receiptParent = new GameObject();

            //Determine total width of receipt and apply to preferredWidth of LayoutElement
            float totalReceiptWidth = (receiptWidth * receiptSizeScale) * ((ingredients.Count / 2) + ingredients.Count % 2);
            LayoutElement layoutElement = receiptParent.AddComponent<LayoutElement>();
            layoutElement.preferredWidth = totalReceiptWidth;

            PopulateReceipt(receiptParent, ingredients);
            ticketManager.AddTicket(receiptParent);
        }
    }

    void PopulateReceipt(GameObject receiptParent, List<string> ingredients)
    {
        GameObject curReceipt = null;
        // Shift spawn of receipt left half of the receipt width for every receipt over 1
        float startX = (-1 * receiptWidth * receiptSizeScale) * (((ingredients.Count / 2) + ingredients.Count % 2) - 1) / 2;
        Vector2 spawnPosition = new Vector2(startX, 0);

        foreach (string ingredient in ingredients)
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

            if (topImage.sprite == null)
            {
                topImage.sprite = ingredientImages[ingredient];
            }
            else
            {
                bottomImage.sprite = ingredientImages[ingredient];
                curReceipt = null;
            }
        }
    }



}
