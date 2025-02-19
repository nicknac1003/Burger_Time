using UnityEngine;
using TMPro;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int day = 1;
    public bool canCreateCustomers = true;
    private int currentCustomerCount = 0;
    private float totalMinutes = 0f;
    private int currentHour = 0;
    private int currentMinute = 0;
    public int startHour = 9;
    public int startMinute = 00;
    public int endHour = 21;
    public int endMinute = 00;
    public float dayDurationMinutes = 5;
    private  float dayDuration; // Duration of a day in seconds 
    private float elapsedTime = 0f;
    private bool dayOver = false;
    private bool dayStarting = false;
    public bool logTime = false;
    public float dayDelay = 6f;

    public float initialRating = 5f;
    private float rating;
    public float ratingScale = 0.3f;

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        dayDuration = dayDurationMinutes * 60f; // Convert minutes to seconds
    }

    void Start()
    {
        currentHour = startHour;
        currentMinute = startMinute;
        rating = initialRating;
    }

    void Update()
    {
        // Update the elapsed time
        elapsedTime += Time.deltaTime;

        totalMinutes = elapsedTime / dayDuration * (endHour * 60f + endMinute - (startHour * 60f + startMinute)); // 1440 minutes in a day
        currentHour = startHour + Mathf.FloorToInt((totalMinutes + startMinute) / 60f);
        currentMinute =  Mathf.FloorToInt((startMinute + totalMinutes) % 60f);
        DisplayTime();

        // Check if the day duration has been exceeded
        if (elapsedTime >= dayDuration)
        {
            dayOver = true;
            canCreateCustomers = false;
        }
        if (dayOver && currentCustomerCount == 0 && !dayStarting)
        {
            dayStarting = true;
            endDay();

        }
    }

    public void HandlePauseGame()
    {
        if(Mathf.Approximately(Time.timeScale, 1f))
        {
            PauseGame();
        }
        else
        {
            UnpauseGame();
        }
    }
    private void PauseGame()
    {
        Time.timeScale = 0f;
    }
    private void UnpauseGame()
    {
        Time.timeScale = 1f;
    }

    public void endDay()
    {
        // End of the day logic
        Debug.Log("End of Day " + day);
        
        //start new day after daydelay seconds
        Invoke(nameof(StartNewDay), dayDelay);

    }

    // Call this method to start a new day
    public void StartNewDay()
    {
        day++;
        elapsedTime = 0f;
        canCreateCustomers = true;
        dayOver = false;
        dayStarting = false;
        DisplayDay();
    }

     // Method to display the time as a 24-hour clock probably will eventual involve a UI element
    private void DisplayTime()
    {
        string timeString;
        if (currentHour >= 24)
        {
            timeString = "12:00 PM";
        }
        else{
            // Format the time as HH:MM
            string ampm = currentHour >= 12 ? "PM" : "AM";

            string hourString = currentHour > 12 ? (currentHour - 12).ToString() : currentHour.ToString();
            timeString = $"{hourString}:{currentMinute:D2} {ampm}";
        }

        // Display the time (for example, using Debug.Log or a UI element)
        if (logTime)
            Debug.Log("Current Time: " + timeString ); //+ "(" + elapsedTime / dayDuration +")"
        if (timeText != null)
            timeText.text = timeString;
    }
    private void DisplayDay()
    {
        if (dayText != null)
            dayText.text = "Day " + day;
            //i want the text to fade in and out a couple times
            int fadeCount = 4;
            float fadeDuration = 0.8f;
            StartCoroutine(FadeText(dayText, fadeCount, fadeDuration));
    }
        private IEnumerator FadeText(TextMeshProUGUI text, int fadeCount, float duration)
    {
        for (int i = 0; i < fadeCount; i++)
        {
            // Fade out
            yield return StartCoroutine(Fade(text, 1f, 0f, duration));
            // Fade in
            yield return StartCoroutine(Fade(text, 0f, 1f, duration));
        }
    }

    private IEnumerator Fade(TextMeshProUGUI text, float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;
        Color color = text.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsedTime / duration);
            text.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }

        text.color = new Color(color.r, color.g, color.b, endAlpha);
    }
    public void WelpReview(float review)
    {
        Debug.Log("Review: " + review);
        float mid = 2.5f;
        float diff = review - mid;
        float scaledDiff = diff / (1 + Mathf.Abs(diff));
        Debug.Log(scaledDiff);
        rating += scaledDiff * ratingScale;
        rating = Mathf.Clamp(rating, 0f, initialRating);
        Debug.Log("Rating: " + rating);

    }
    public void addCustomer()
    {
        currentCustomerCount++;
    }
    public void removeCustomer()
    {
        currentCustomerCount--;
    }
    public int getCurrentCustomerCount()
    {
        return currentCustomerCount;
    }
}
 
