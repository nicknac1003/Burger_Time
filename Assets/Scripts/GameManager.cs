using System.Runtime.InteropServices;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
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
    public bool logTime = false;
    void Awake()
    {
        dayDuration = dayDurationMinutes * 60f; // Convert minutes to seconds
    }

    void Start()
    {
        currentHour = startHour;
        currentMinute = startMinute;

    }

    void Update()
    {
        
        // Update the elapsed time
        elapsedTime += Time.deltaTime;

        totalMinutes = elapsedTime / dayDuration * ((endHour * 60f + endMinute) - (startHour * 60f + startMinute)); // 1440 minutes in a day
        currentHour = startHour + Mathf.FloorToInt((totalMinutes + startMinute) / 60f);
        currentMinute =  Mathf.FloorToInt((startMinute + totalMinutes) % 60f);
        DisplayTime();

        // Check if the day duration has been exceeded
        if (elapsedTime >= dayDuration)
        {
            dayOver = true;
            canCreateCustomers = false;
        }
        if (dayOver && currentCustomerCount == 0)
        {
            endDay();
        }

        // Example of stopping customer creation
        if (canCreateCustomers)
        {
            // Your logic to create customers
            //probably some call to a customer spawner or something
        }

    }

    public void endDay()
    {
        // End of the day logic
        Debug.Log("End of Day " + day);
        Debug.Break();
        // Stop creating customers

    }

    // Call this method to start a new day
    public void StartNewDay()
    {
        day++;
        elapsedTime = 0f;
        canCreateCustomers = true;
        dayOver = false;
    }

     // Method to display the time as a 24-hour clock probably will eventual involve a UI element
    private void DisplayTime()
    {
        // Format the time as HH:MM
        string timeString = $"{currentHour:D2}:{currentMinute:D2}";

        // Display the time (for example, using Debug.Log or a UI element)
        if (logTime)
            Debug.Log("Current Time: " + timeString ); //+ "(" + elapsedTime / dayDuration +")"
    }
    public void addCustomer()
    {
        currentCustomerCount++;
    }
    public void removeCustomer()
    {
        currentCustomerCount--;
    }
}
 
