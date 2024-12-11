using UnityEngine;
using Radishmouse; // For UILineRenderer
using Newtonsoft.Json.Linq; // For parsing JSON
using System.Net.Http;
using System.Collections.Generic;

public class TemperatureGraph : MonoBehaviour
{
    [Header("Graph Components")]
    public UILineRenderer uiLineRenderer; // Assign in Inspector
    public RectTransform lineGraphRectTransform;

    [Header("OctoPrint Connection Settings")]
    public string octoPrintURL = "http://127.0.0.1:5000";
    public string apiKey = "5D60CE27902F4486AFD8112B24449923";

    [Header("Graph Settings")]
    public float fetchInterval = 1f; // Interval to fetch data (seconds)
    public float unityGraphMaxY = 130f; // Max Y value (Unity graph space)
    public float minTemp = 15f; // Minimum temperature (°C)
    public float maxTemp = 230f; // Maximum temperature (°C)
    public int maxGraphPoints = 285; // Maximum number of points to display on the graph

    private List<Vector2> graphPoints = new List<Vector2>();
    private float elapsedTime = 0f; // Time counter for fetching
    private int graphElapsedTime = 0; // Tracks X-axis position (integer)

    private HttpClient httpClient;

    void Start()
    {
        if (uiLineRenderer == null || lineGraphRectTransform == null)
        {
            Debug.LogError("UILineRenderer or RectTransform not assigned!");
            return;
        }

        // Initialize HTTP client
        httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);

        // Debug: Log graph dimensions
        Debug.Log($"Graph RectTransform Dimensions -> Width: {lineGraphRectTransform.rect.width}, Height: {lineGraphRectTransform.rect.height}");
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // Fetch data and update graph at regular intervals
        if (elapsedTime >= fetchInterval)
        {
            elapsedTime = 0f; // Reset the timer
            FetchAndRenderTemperature(); // Fetch temperature and update graph
        }
    }

    private async void FetchAndRenderTemperature()
    {
        try
        {
            string endpoint = $"{octoPrintURL}/api/printer";
            HttpResponseMessage response = await httpClient.GetAsync(endpoint);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(json);

                float hotendTemp = data["temperature"]["tool0"]["actual"]?.Value<float>() ?? -1f;

                Debug.Log($"Raw Hotend Temperature: {hotendTemp}");

                if (hotendTemp < 0)
                {
                    Debug.LogWarning($"Invalid temperature: {hotendTemp}. Skipping.");
                    return;
                }

                // Normalize temperature for Unity graph (15–230 to 0–130)
                float normalizedY = Normalize(hotendTemp, minTemp, maxTemp, 0f, unityGraphMaxY);

                // Increment X-axis position by 1 unit per fetch
                graphElapsedTime += 1;
                float normalizedX = graphElapsedTime;

                Debug.Log($"Normalized Values -> X: {normalizedX}, Y: {normalizedY}");

                // Add point to the graph
                graphPoints.Add(new Vector2(normalizedX, normalizedY));

                // Keep the graph rolling by removing old points if necessary
                if (graphPoints.Count > maxGraphPoints)
                {
                    graphPoints.RemoveAt(0);
                }

                // Update the UILineRenderer
                uiLineRenderer.points = graphPoints.ToArray();
                uiLineRenderer.SetVerticesDirty(); // Force redraw
                Debug.Log("Graph updated with new data point.");
            }
            else
            {
                Debug.LogError($"Failed to fetch temperature. HTTP Status: {response.StatusCode}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error fetching temperature: {ex.Message}");
        }
    }

    private float Normalize(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        value = Mathf.Clamp(value, inputMin, inputMax);
        return (value - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin;
    }

    private void OnDestroy()
    {
        // Dispose of the HTTP client
        httpClient?.Dispose();
    }
}
