using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ConsoleLogViewer : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("TextMeshProUGUI component to display the console logs.")]
    public TextMeshProUGUI consoleText;

    [Tooltip("ScrollRect component controlling the scrolling behavior.")]
    public ScrollRect scrollRect; // Make this public to assign in the Inspector

    void OnEnable()
    {
        // Register the log message received event
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        // Unregister the event when the object is disabled or destroyed
        Application.logMessageReceived -= HandleLog;
    }

    void Start()
    {
        if (consoleText == null)
        {
            Debug.LogError("Console TextMeshProUGUI component is not assigned.");
            return;
        }

        if (scrollRect == null)
        {
            Debug.LogWarning("ScrollRect is not assigned. Please assign it in the Inspector.");
            // Optionally, you can attempt to find it in the hierarchy
            // scrollRect = consoleText.GetComponentInParent<ScrollRect>();
            // if (scrollRect == null)
            // {
            //     Debug.LogError("ScrollRect not found in the parent hierarchy of consoleText.");
            //     return;
            // }
        }
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Format the log message
        string formattedMessage = FormatLogMessage(logString, stackTrace, type);

        // Append the log message to the consoleText
        consoleText.text += formattedMessage + "\n";

        // Force the layout to rebuild and the canvas to update
        LayoutRebuilder.ForceRebuildLayoutImmediate(consoleText.rectTransform);
        Canvas.ForceUpdateCanvases();

        // Scroll to the bottom to show the latest log
        ScrollToBottom();
    }

    string FormatLogMessage(string logString, string stackTrace, LogType type)
    {
        // Customize the formatting based on the log type
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                return $"<color=red>[Error]</color> {logString}\n{stackTrace}";
            case LogType.Warning:
                return $"<color=yellow>[Warning]</color> {logString}";
            default:
                return $"[Log] {logString}";
        }
    }

    void ScrollToBottom()
    {
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();

            // Option 1: Use verticalNormalizedPosition
            scrollRect.verticalNormalizedPosition = 0f;

            // Option 2: Adjust the content's anchored position
            // RectTransform contentRect = scrollRect.content;
            // contentRect.anchoredPosition = new Vector2(contentRect.anchoredPosition.x, 0);

            Canvas.ForceUpdateCanvases();
        }
        else
        {
            Debug.LogWarning("ScrollRect is null. Cannot scroll to bottom.");
        }
    }
}
