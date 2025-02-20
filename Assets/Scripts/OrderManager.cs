using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class OrderManager : MonoBehaviour
{
    public List<GameObject> tickets;
    public bool isOpen = false;

    [SerializeField]
    public float scaleSize = 2.5f; // possibly best to set scalesize based on number of tickets?
    private RectTransform ticketManagerTransform;
    private Vector2 openPosition = new Vector2(0.0f, 0.0f); // center screen
    private Vector2 closedPosition = new Vector2(0.00f, -0.43f); // bottom center
    private float totalTicketWidth = 0.0f;

    //Gameplay Variables
    public CanvasGroup background;

    private RectTransform canavsTransform;
    private HorizontalLayoutGroup layoutGroup;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Get gameplay variables
        ticketManagerTransform = GetComponent<RectTransform>();
        canavsTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        layoutGroup = GetComponent<HorizontalLayoutGroup>();

        // Set initial position
        ticketManagerTransform.anchoredPosition = new Vector2(
            canavsTransform.rect.width * closedPosition.x,
            canavsTransform.rect.height * closedPosition.y
        );

        //Set initial Width
        //ticketManagerTransform.sizeDelta = new Vector2(canavsTransform.rect.width, ticketManagerTransform.sizeDelta.y);
    }

    public void AddTicket(GameObject ticket)
    {
        tickets.Add(ticket);
        ticket.transform.SetParent(transform);
        totalTicketWidth += ticket.GetComponent<LayoutElement>().preferredWidth + layoutGroup.spacing;

        ticketManagerTransform.sizeDelta = new Vector2(totalTicketWidth, ticketManagerTransform.sizeDelta.y);
    }
    public void PopTicket()
    {
        if (tickets.Count == 0)
        {
            return;
        }

        GameObject ticket = tickets[0];
        tickets.RemoveAt(0);
        totalTicketWidth -= ticket.GetComponent<RectTransform>().rect.width + layoutGroup.spacing;
        Destroy(ticket);

        ticketManagerTransform.sizeDelta = new Vector2(totalTicketWidth, ticketManagerTransform.sizeDelta.y);
    }


    [ContextMenu("Open")]
    public void Open()
    {
        if (tickets.Count == 0)
        {
            return;
        }

        isOpen = true;

        Vector2 targetPosition = new Vector2(
            canavsTransform.rect.width * openPosition.x,
            canavsTransform.rect.height * openPosition.y
        );

        LeanTween.value(gameObject, changeWidth, ticketManagerTransform.sizeDelta.x, canavsTransform.rect.width, 1.2f).setEaseInCubic(); // 50% of canvas width
        LeanTween.value(gameObject, scaleTickets, 1.0f, scaleSize, 1.0f).setEaseInCubic();

        ticketManagerTransform.LeanMoveLocal(targetPosition, 0.7f).setEaseInCirc();
        LeanTween.alphaCanvas(background, 0.45f, 0.7f).setEaseInCirc();
    }

    [ContextMenu("Close")]
    public void Close()
    {
        isOpen = false;
        //layoutGroup.childForceExpandWidth = false;

        Vector2 targetPosition = new Vector2(
            canavsTransform.rect.width * closedPosition.x,
            canavsTransform.rect.height * closedPosition.y
        );

        LeanTween.value(gameObject, changeWidth, ticketManagerTransform.sizeDelta.x, totalTicketWidth, 1f).setEaseInBack(); // 10% of canvas width

        ticketManagerTransform.LeanMoveLocal(targetPosition, 1f).setEaseInQuart();

        LeanTween.alphaCanvas(background, 0f, 1.0f).setEaseInQuart();
        LeanTween.value(gameObject, scaleTickets, scaleSize, 1.0f, 1.0f).setEaseInCubic();
    }

    private void changeWidth(float val)
    {
        //Debug.Log("tweened value:"+val+" set this to whatever variable you are tweening...");
        ticketManagerTransform.sizeDelta = new Vector2(val, ticketManagerTransform.sizeDelta.y);
    }

    private void scaleTickets(float scale)
    {
        foreach (GameObject ticket in tickets)
        {
            RectTransform ticketTransform = ticket.GetComponent<RectTransform>();
            var localScale = ticketTransform.localScale;
            localScale.x = scale;
            localScale.y = scale;
            ticketTransform.localScale = localScale;
        }
    }
}
