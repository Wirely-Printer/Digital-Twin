using UnityEngine;
using UnityEngine.UI;
using OctoprintClient;

public class OctoPrint_Buttons : MonoBehaviour
{
    private OctoprintConnection connection;

    [Header("OctoPrint Connection Settings")]
    public string octoprintURL = "http://127.0.0.1:5000";
    public string apiKey = "5D60CE27902F4486AFD8112B24449923";

    [Header("UI Buttons")]
    public Button playButton;
    public Button pauseButton;
    public Button stopButton;

    void Start()
    {
        // Initialize OctoPrint connection
        connection = OctoprintManager.Instance;
        
        // Attach button click listeners
        playButton.onClick.AddListener(OnPlayClicked);
        pauseButton.onClick.AddListener(OnPauseClicked);
        stopButton.onClick.AddListener(OnStopClicked);
    }

    void OnPlayClicked()
    {
        try
        {
            // Send command to start/resume printing
            string response = connection.Jobs.StartJob();
            Debug.Log("Play button clicked. Response: " + response);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error starting print job: " + ex.Message);
        }
    }

    void OnPauseClicked()
    {
        try
        {
            // Send command to pause the current job
            string response = connection.Jobs.PauseJob();
            Debug.Log("Pause button clicked. Response: " + response);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error pausing print job: " + ex.Message);
        }
    }

    void OnStopClicked()
    {
        try
        {
            // Send command to cancel the current job
            string response = connection.Jobs.CancelJob();
            Debug.Log("Stop button clicked. Response: " + response);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error stopping print job: " + ex.Message);
        }
    }
}
