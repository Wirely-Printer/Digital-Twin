using UnityEngine;
using TMPro; // For TextMeshPro
using System.IO; // For FileStream
using System.Collections;
using UnityEngine.UI; // Required for ScrollRect

public class OctoPrintTerminal : MonoBehaviour
{
    [Header("OctoPrint Log Settings")]
    public string logFilePath = "C:\\OctoPrint\\basedir\\5000\\logs\\serial.log"; // Path to the OctoPrint log file
    public TextMeshProUGUI octoPrintText; // TextMeshPro UI element for the terminal
    public ScrollRect scrollRect; // ScrollRect for auto-scrolling

    private long lastFilePosition = 0; // Tracks the last position in the log file

    [Header("Update Settings")]
    public float updateInterval = 1.0f; // How often to check for updates (in seconds)

    void Start()
    {
        if (octoPrintText == null || scrollRect == null)
        {
            Debug.LogError("UI elements not assigned in the Inspector!");
            return;
        }

        // Start the coroutine to continuously update the log display
        StartCoroutine(UpdateLog());
    }

    IEnumerator UpdateLog()
    {
        while (true)
        {
            ReadNewLinesFromLogFile();
            yield return new WaitForSeconds(updateInterval);
        }
    }

    void ReadNewLinesFromLogFile()
    {
        try
        {
            if (!File.Exists(logFilePath))
            {
                octoPrintText.text = "Log file not found.";
                return;
            }

            using (FileStream fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // Start reading from the last known position
                fs.Seek(lastFilePosition, SeekOrigin.Begin);

                using (StreamReader sr = new StreamReader(fs))
                {
                    string newContent = sr.ReadToEnd();

                    if (!string.IsNullOrEmpty(newContent))
                    {
                        octoPrintText.text += newContent; // Append new content to the UI
                        AutoScrollToBottom(); // Scroll to the latest log
                        lastFilePosition = fs.Position; // Update file position
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error reading OctoPrint log: " + ex.Message);
        }
    }

    void AutoScrollToBottom()
    {
        Canvas.ForceUpdateCanvases(); // Refresh UI layout
        if (scrollRect != null)
        {
            scrollRect.verticalNormalizedPosition = 0; // Scroll to the bottom
        }
    }
}
