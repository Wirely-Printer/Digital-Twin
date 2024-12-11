using UnityEngine;
using Radishmouse; // For UILineRenderer
using System.Collections.Generic;

public class LateralSpeedGraph : MonoBehaviour
{
    [Header("Graph Components")]
    public UILineRenderer uiLineRenderer; // Assign the UILineRenderer in the Inspector
    public RectTransform lineGraphRectTransform; // RectTransform of the graph panel

    [Header("Graph Settings")]
    public float updateInterval = 0.5f; // Interval to update the graph (in seconds)
    public float unityGraphMaxY = 130f; // Max Y value (Unity graph space)
    public float unityGraphMaxX = 285f; // Max X value (Unity graph space)
    public float maxYChange = 2f; // Maximum change in Y value per update
    public float lineThickness = 1f; // Thickness of the line

    private List<Vector2> graphPoints = new List<Vector2>(); // Points for the graph
    private float elapsedTime = 0f; // Timer for update interval
    private float graphElapsedTime = 0f; // Tracks X-axis position
    private float lastYValue = 65f; // Start with a middle Y value (can be adjusted)

    void Start()
    {
        if (uiLineRenderer == null || lineGraphRectTransform == null)
        {
            Debug.LogError("UILineRenderer or RectTransform is not assigned!");
            return;
        }

        // Set the thickness of the line
        uiLineRenderer.thickness = lineThickness;
        Debug.Log("Graph initialized with empty points.");
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        // Update the graph at regular intervals
        if (elapsedTime >= updateInterval)
        {
            elapsedTime = 0f; // Reset the timer
            AddMinimalChangePointToGraph(); // Add a new point with minimal Y change
        }
    }

    private void AddMinimalChangePointToGraph()
    {
        // Generate a new Y value with minimal change (±2 from the last value)
        float randomChange = Random.Range(-maxYChange, maxYChange);
        float newYValue = Mathf.Clamp(lastYValue + randomChange, 0f, unityGraphMaxY);
        lastYValue = newYValue; // Update the last Y value

        // Update X-axis position
        if (graphElapsedTime < unityGraphMaxX)
        {
            graphElapsedTime += 1f; // Increment X
        }
        else
        {
            graphElapsedTime = 0f; // Reset X to 0
            graphPoints.Clear(); // Clear all points when restarting
        }

        // Add the new point to the graph
        graphPoints.Add(new Vector2(graphElapsedTime, newYValue));

        // Update the UILineRenderer
        uiLineRenderer.points = graphPoints.ToArray();
        uiLineRenderer.SetVerticesDirty(); // Force redraw
    }
}
