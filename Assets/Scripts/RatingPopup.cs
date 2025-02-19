using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class RatingPopup : MonoBehaviour
{
    public TextMeshProUGUI ratingText;
    public GameObject iconObject;
    private Image starIcon;

    private float rating = 5f;

    public float riseDistance = 2f;
    public float riseDuration = 2f;

    public void SetRating(float newRating)
    {
        rating = newRating;
    }

    private void Awake()
    {
        starIcon = iconObject.GetComponent<Image>();
    }
    void Start()
    {
        ratingText.text = $"{rating:F1}";
        setTextColor();
        StartCoroutine(RiseAndFade());
    }

    private void setTextColor(){
        if (rating >= 5f) {
            ratingText.color = new Color(1f, 0.84f, 0f);
        } else {
            ratingText.color = Color.Lerp(Color.red, Color.green, rating / 5f);
        }
    }

    private IEnumerator RiseAndFade()
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + Vector3.up * riseDistance;

        while (elapsedTime < riseDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / riseDuration;
            float easedT = EaseOutQuad(t);
            transform.position = Vector3.Lerp(startPosition, endPosition, easedT);
            //fade
            ratingText.color = new Color(ratingText.color.r, ratingText.color.g, ratingText.color.b, 1 - t);
            starIcon.color = new Color(starIcon.color.r, starIcon.color.g, starIcon.color.b, 1 - t);
            yield return null;
        }

        // Ensure the final position is set
        Destroy(gameObject);
    }

    private float EaseOutQuad(float t)
    {
        return t * (2 - t);
    }
}