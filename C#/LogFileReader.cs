using UnityEngine;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class LogFileReader : MonoBehaviour
{
    [Header("Log File Settings")]
    [Tooltip("Path to the log file.")]
    public string logFilePath = "C:\\OctoPrint\\basedir\\5000\\logs\\serial.log";

    [Tooltip("Time interval between log updates (in seconds).")]
    public float updateInterval = 1f;

    [Header("UI Components")]
    [Tooltip("TextMeshProUGUI component to display the log content.")]
    public TextMeshProUGUI logText;

    [Tooltip("ScrollRect component controlling the scrolling behavior.")]
    public ScrollRect scrollRect; // Added for manual assignment

    private long lastFilePosition = 0;

    void Start()
    {
        if (logText == null)
        {
            Debug.LogError("Log TextMeshProUGUI component is not assigned.");
            return;
        }

        if (scrollRect == null)
        {
            Debug.LogWarning("ScrollRect is not assigned. Please assign it in the Inspector.");
            // Optionally, you can attempt to find it in the hierarchy
            // scrollRect = logText.GetComponentInParent<ScrollRect>();
            // if (scrollRect == null)
            // {
            //     Debug.LogError("ScrollRect not found in the parent hierarchy of logText.");
            //     return;
            // }
        }

        if (!File.Exists(logFilePath))
        {
            Debug.LogError("Log file not found at path: " + logFilePath);
            return;
        }

        // Initialize lastFilePosition to the end of the file
        FileInfo fileInfo = new FileInfo(logFilePath);
        lastFilePosition = fileInfo.Length;
        Debug.Log($"Initial file length: {fileInfo.Length} bytes. Starting from the end of the file.");

        // Start the coroutine to read new log entries
        StartCoroutine(ReadLogUpdates());
    }

    IEnumerator ReadLogUpdates()
    {
        while (true)
        {
            FileInfo fileInfo = new FileInfo(logFilePath);
            long fileLength = fileInfo.Length;

            if (fileLength < lastFilePosition)
            {
                Debug.LogWarning("File length is smaller than last read position. File may have been truncated. Resetting lastFilePosition to 0.");
                lastFilePosition = 0;
            }

            if (fileLength > lastFilePosition)
            {
                try
                {
                    using (FileStream fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        fs.Seek(lastFilePosition, SeekOrigin.Begin);
                        using (StreamReader reader = new StreamReader(fs))
                        {
                            string newLogContent = reader.ReadToEnd();
                            lastFilePosition = fs.Position;

                            if (!string.IsNullOrEmpty(newLogContent))
                            {
                                // Update the UI on the main thread
                                AppendLogContent(newLogContent);
                            }
                        }
                    }
                }
                catch (IOException e)
                {
                    Debug.LogError("Error reading log file: " + e.Message);
                }
            }

            // Wait for the specified update interval before checking for new entries
            yield return new WaitForSeconds(updateInterval);
        }
    }

    void AppendLogContent(string newLogContent)
    {
        logText.text += newLogContent;

        // Force the layout to rebuild and the canvas to update
        LayoutRebuilder.ForceRebuildLayoutImmediate(logText.rectTransform);
        Canvas.ForceUpdateCanvases();

        // Scroll to the bottom to mimic a real terminal
        ScrollToBottom();
    }

    void ScrollToBottom()
    {
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
            Canvas.ForceUpdateCanvases();
        }
        else
        {
            Debug.LogWarning("ScrollRect is not assigned. Cannot scroll to bottom.");
        }
    }
}
