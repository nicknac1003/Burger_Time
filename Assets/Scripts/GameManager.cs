using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int day = 1;
    private float totalMinutes = 0f;

    private int currentHour = 0;
    private int currentMinute = 0;

    public int startHour = 9;
    public int startMinute = 00;

    public int endHour = 21;
    public int endMinute = 00;

    public float dayDurationMinutes = 5;
    private float dayDuration; // Duration of a day in seconds 
    private float elapsedTime = 0f;
    public bool logTime = false;
    public float dayDelay = 6f;

    public float initialRating = 5f;
    private float rating;
    public float ratingScale = 0.3f;

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI timeText;

    [SerializeField] private GameObject pauseMenu;
    private bool paused = false;

    public static bool  GamePaused()        => Instance.paused;
    public static int   GetHour()           => Instance.currentHour;
    public static float GetHourCompletion() => Instance.currentMinute / 60f;
    public static int   GetDay()            => Instance.day;
    public static bool  Open()              => BetweenTimes(Instance.currentHour, Instance.currentMinute, Instance.startHour, Instance.startMinute, Instance.endHour, Instance.endMinute);

    public static float TimeAsFloat(int hour, float minutes) => hour + minutes / 60f;
    public static bool  BetweenTimes(int hour, float minutes, int hourA, float minutesA, int hourB, float minutesB) => TimeAsFloat(hour, minutes) >= TimeAsFloat(hourA, minutesA) && TimeAsFloat(hour, minutes) <= TimeAsFloat(hourB, minutesB);

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
        elapsedTime  += Time.deltaTime;
        totalMinutes  = elapsedTime / dayDuration * (endHour * 60f + endMinute - (startHour * 60f + startMinute)); // 1440 minutes in a day
        currentHour   = startHour + Mathf.FloorToInt((totalMinutes + startMinute) / 60f);
        currentMinute = Mathf.FloorToInt((startMinute + totalMinutes) % 60f);
        DisplayTime();

        if (Open() == false && CustomerManager.Customers() == 0)
        {
            EndDay();
        }
    }

    public void HandlePauseGame()
    {
        if(GamePaused())
        {
            UnpauseGame();
        }
        else
        {
            PauseGame();
        }
    }
    private void PauseGame()
    {
        paused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }
    private void UnpauseGame()
    {
        paused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void EndDay()
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
            yield return StartCoroutine(Fade(text, 1f, 0f, duration));
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
    public static void WelpReview(float review)
    {
        float mid = 2.5f;
        float diff = review - mid;
        float scaledDiff = diff / (1 + Mathf.Abs(diff));
        Debug.Log(scaledDiff);
        Instance.rating += scaledDiff * Instance.ratingScale;
        Instance.rating = Mathf.Clamp(Instance.rating, 0f, Instance.initialRating);
        Debug.Log("Rating: " + Instance.rating);

    }
}
 
