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
    private bool endOfDay = false;

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI dayTextShadow;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI timeTextShadow;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject MainGameUI;
    [SerializeField] private GameObject GameOverUI;
    [SerializeField] private TextMeshProUGUI daysLastedText;
    [SerializeField] private Jukebox MusicPlayer;
    [SerializeField] private TextMeshProUGUI openText;
    private bool paused = false;

    private bool gameEnded = false;

    public static bool GamePaused() => Instance.paused;
    public static bool GameEnded() => Instance.gameEnded;
    public static int GetHour() => Instance.currentHour;
    public static float GetHourCompletion() => Instance.currentMinute / 60f;
    public static int GetDay() => Instance.day;
    public static bool Open() => BetweenTimes(Instance.currentHour, Instance.currentMinute, Instance.startHour, Instance.startMinute, Instance.endHour, Instance.endMinute);

    public static float TimeAsFloat(int hour, float minutes) => hour + minutes / 60f;
    public static bool BetweenTimes(int hour, float minutes, int hourA, float minutesA, int hourB, float minutesB) => TimeAsFloat(hour, minutes) >= TimeAsFloat(hourA, minutesA) && TimeAsFloat(hour, minutes) <= TimeAsFloat(hourB, minutesB);

    void Awake()
    {
        if (Instance == null)
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
        RatingUI.UpdateRating(rating / 5f);
    }

    void Update()
    {
        if (gameEnded) return;
        elapsedTime += Time.deltaTime;
        totalMinutes = elapsedTime / dayDuration * (endHour * 60f + endMinute - (startHour * 60f + startMinute)); // 1440 minutes in a day
        currentHour = startHour + Mathf.FloorToInt((totalMinutes + startMinute) / 60f);
        currentMinute = Mathf.FloorToInt((startMinute + totalMinutes) % 60f);
        DisplayTime();
        DisplayOpen();

        if (Open() == false && CustomerManager.InBuilding() == 0)
        {
            EndDay();
        }
        if (rating <= 0f && !gameEnded)
        {
            EndGame();
        }
    }

    public void HandlePauseGame()
    {
        if (GamePaused())
        {
            UnpauseGame();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

        }
        else
        {
            PauseGame();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
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
        if (endOfDay) return;
        // End of the day logic
        Debug.Log("End of Day " + day);
        endOfDay = true;

        //start new day after daydelay seconds
        Invoke(nameof(StartNewDay), dayDelay);
    }

    // Call this method to start a new day
    public void StartNewDay()
    {
        day++;
        elapsedTime = 0f;
        endOfDay = false;
        CustomerManager.spawnAt9 = true;
        DisplayDay();
    }

    // Method to display the time as a 24-hour clock probably will eventual involve a UI element
    private void DisplayTime()
    {
        string timeString;
        if (currentHour >= 24)
        {
            timeString = "12:00PM";
        }
        else
        {
            // Format the time as HH:MM
            string ampm = currentHour >= 12 ? "PM" : "AM";

            string hourString = currentHour > 12 ? (currentHour - 12).ToString() : currentHour.ToString();
            timeString = $"{hourString}:{currentMinute:D2}{ampm}";
        }

        // Display the time (for example, using Debug.Log or a UI element)
        if (logTime)
            Debug.Log("Current Time: " + timeString); //+ "(" + elapsedTime / dayDuration +")"
        if (timeText != null)
        {
            timeString = "<mspace=48>" + timeString + "</mspace>";
            timeText.text = timeString;
            timeTextShadow.text = timeString;
        }
    }
    private void DisplayDay()
    {
        if (dayText != null)
        {
            string text = "<mspace=mspace=48>Day:" + day + "</mspace>";
            dayText.text = text;
            dayTextShadow.text = text;
        }
        //i want the text to fade in and out a couple times
        int fadeCount = 4;
        float fadeDuration = 0.8f;
        StartCoroutine(FadeText(dayText, fadeCount, fadeDuration));
        StartCoroutine(FadeText(dayTextShadow, fadeCount, fadeDuration));
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

    public static void WelpReview(Customer customer, float review)
    {
        Instantiate(GlobalConstants.reviewPopup, customer.transform.position, Quaternion.identity).GetComponent<RatingPopup>().SetRating(review);
        
        float diff       = review - 2.5f;
        float scaledDiff = diff / (1 + Mathf.Abs(diff));
        Instance.rating += scaledDiff * 1.4f; // 0 = -1 star, 5 = +1 star
        Instance.rating  = Mathf.Clamp(Instance.rating, 0f, 5f);

        RatingUI.UpdateRating(Instance.rating / 5f); // normalize for the rating bar
    }
    private void EndGame()
    {
        gameEnded = true;
        Time.timeScale = 0f;
        Jukebox.ToggleMusic();
        MusicPlayer.SetOn(false);
        MainGameUI.SetActive(false);
        SetEndText();
        //fade in game over screen
        StartCoroutine(FadeInGameOverUI());
    }
    private IEnumerator FadeInGameOverUI()
    {
        CanvasGroup canvasGroup = GameOverUI.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            yield break;
        }

        GameOverUI.SetActive(true);
        float duration = 1f; // Duration of the fade-in
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.unscaledDeltaTime; // Use unscaled time to keep the fade effect working even when Time.timeScale is 0
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration);
            yield return null;
        }

        canvasGroup.alpha = 1f;

    }
    private void SetEndText()
    {
        string newText = "Lost on Day: " + day;
        daysLastedText.text = newText;
    }
    private void DisplayOpen()
    {
        if (openText != null)
        {
            string text = Open() ? "Open" : "Closed";
            openText.text = text;
            Color color = Open() ? Color.green : Color.red;
            openText.color = color;
        }
    }
}

