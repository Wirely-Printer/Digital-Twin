using UnityEngine;
using TMPro;
using System;

public class DateTimeUpdater : MonoBehaviour
{
    public TextMeshProUGUI dateTimeText; // Assign your TextMeshPro Text component here

    void Update()
    {
        // Get the current date and time
        DateTime now = DateTime.Now;

        // Format it as needed
        string formattedDateTime = now.ToString("dddd, MMMM dd yyyy - HH:mm:ss");

        // Update the TextMeshPro component
        dateTimeText.text = formattedDateTime;
    }
}
