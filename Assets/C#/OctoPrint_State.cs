
using UnityEngine;
using OctoprintClient;
using TMPro; // For TextMeshPro support
using Newtonsoft.Json.Linq;
using System.Collections;

public class OctoPrint_State : MonoBehaviour
{
    private OctoprintConnection connection;

    [Header("OctoPrint Connection Settings")]
    public string octoprintURL = "http://127.0.0.1:5000";
    public string apiKey = "5D60CE27902F4486AFD8112B24449923";

    [Header("UI Text Placeholders")]
    public TextMeshProUGUI machineStateText;
    public TextMeshProUGUI fileNameText;
    public TextMeshProUGUI materialText;
    public TextMeshProUGUI timeLeftText;
    public TextMeshProUGUI axisPositionText;

    [Header("Update Settings")]
    public float updateInterval = 0.5f; // How often to check for updates (in seconds)

    void Start()
    {
        connection = OctoprintManager.Instance;
        StartCoroutine(CheckForUpdates());
    }

    IEnumerator CheckForUpdates()
    {
        while (true)
        {
            RetrieveOctoPrintState(); // Fetch updates from OctoPrint
            yield return new WaitForSeconds(updateInterval); // Wait before the next update
        }
    }

    void RetrieveOctoPrintState()
    {
        try
        {
            if (connection == null)
            {
                Debug.LogError("Connection is null.");
                return;
            }

            string response = connection.Get("api/job"); // Get print job info
            if (!string.IsNullOrEmpty(response))
            {
                JObject data = JObject.Parse(response);

                // Retrieve Machine State
                string state = data["state"]?.ToString() ?? "Unknown";
                machineStateText.text = state;

                // Retrieve File Name
                string fileName = data["job"]["file"]["name"]?.ToString() ?? "None";
                fileNameText.text = fileName;

                // Set Material (Static for now)
                materialText.text = " PLA"; // Replace with dynamic data if available.

                // Retrieve Approx Time Left
                string timeLeft = data["progress"]["printTimeLeft"]?.ToString() ?? "N/A";
                timeLeftText.text = FormatTime(timeLeft);

                // Retrieve Axis Position
                RetrieveAxisPosition();
            }
            else
            {
                Debug.LogError("No response received from OctoPrint.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error retrieving OctoPrint state: " + ex.Message);
        }
    }

void RetrieveAxisPosition()
{
    try
    {
        // Initialize OctoprintPosTracker
        OctoprintPosTracker tracker = new OctoprintPosTracker(connection);

        // Get the current position synchronously
        float[] position = tracker.GetCurrentPosSync();

        // Display the position in the UI
        axisPositionText.text = $"X={position[0]:F2}, Y={position[1]:F2}, Z={position[2]:F2}";
    }
    catch (System.Exception ex)
    {
        axisPositionText.text = "Error";
    }
}

    string FormatTime(string timeInSeconds)
    {
        if (int.TryParse(timeInSeconds, out int seconds))
        {
            int hours = seconds / 3600;
            int minutes = (seconds % 3600) / 60;
            return $"{hours}h {minutes}m";
        }
        return "N/A";
    }
}
