using UnityEngine;
using System.Collections.Generic;


public class TicketManager : MonoBehaviour
{
    public List<GameObject> tickets;
    public CanvasGroup background;
    [SerializeField]
    public float scaleSize = 2.5f; // possibly best to set scalesize based on number of tickets?
    private RectTransform ticketManagerTransform;
    private RectTransform canavsTransform;
    private Vector2 openPosition = new Vector2(0.0f, 0.0f); // center screen
    private Vector2 closedPosition = new Vector2(-0.3f, -0.5f); // left third, bottom screen
    private float totalTicketWidth = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ticketManagerTransform = GetComponent<RectTransform>();
        canavsTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    public void AddTicket(GameObject ticket)
    {
        tickets.Add(ticket);
        ticket.transform.SetParent(transform);
        totalTicketWidth += ticket.GetComponent<RectTransform>().rect.width;
    }


    [ContextMenu("Open")]
    public void Open()
    {
        Vector2 targetPosition = new Vector2(
            canavsTransform.rect.width * openPosition.x,
            canavsTransform.rect.height * openPosition.y
        );
        //
        LeanTween.value(gameObject, changeWidth, ticketManagerTransform.sizeDelta.x, canavsTransform.rect.width, 1.2f).setEaseInCubic(); // 50% of canvas width
        LeanTween.value(gameObject, scaleTickets, 1.0f, scaleSize, 1.0f).setEaseInCubic();

        ticketManagerTransform.LeanMoveLocal(targetPosition, 0.7f).setEaseInCirc();
        LeanTween.alphaCanvas(background, 0.45f, 0.7f).setEaseInCirc();
    }

    [ContextMenu("Close")]
    public void Close()
    {
        Vector2 targetPosition = new Vector2(
            canavsTransform.rect.width * closedPosition.x,
            canavsTransform.rect.height * closedPosition.y
        );

        LeanTween.value(gameObject, changeWidth, ticketManagerTransform.sizeDelta.x,
            canavsTransform.rect.width * 0.1f, 1f).setEaseInBack(); // 10% of canvas width
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
