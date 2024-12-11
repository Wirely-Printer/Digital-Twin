using UnityEngine;
using System.Collections.Generic;

public class RealTimeGraph : MonoBehaviour
{
    public LineRenderer flowRateGraph;
    public LineRenderer lateralSpeedGraph;
    public LineRenderer zOffsetGraph;
    public LineRenderer hotendTempGraph;

    private List<Vector3> flowRatePoints = new List<Vector3>();
    private List<Vector3> lateralSpeedPoints = new List<Vector3>();
    private List<Vector3> zOffsetPoints = new List<Vector3>();
    private List<Vector3> hotendTempPoints = new List<Vector3>();

    private float xOffset = 0f;
    private const float xSpacing = 0.1f; // Space between points

    public void UpdateGraph(float flowRate, float lateralSpeed, float zOffset, float hotendTemp)
    {
        // Add new points
        flowRatePoints.Add(new Vector3(xOffset, flowRate, 0));
        lateralSpeedPoints.Add(new Vector3(xOffset, lateralSpeed, 0));
        zOffsetPoints.Add(new Vector3(xOffset, zOffset, 0));
        hotendTempPoints.Add(new Vector3(xOffset, hotendTemp, 0));

        // Update line renderers
        UpdateLineRenderer(flowRateGraph, flowRatePoints);
        UpdateLineRenderer(lateralSpeedGraph, lateralSpeedPoints);
        UpdateLineRenderer(zOffsetGraph, zOffsetPoints);
        UpdateLineRenderer(hotendTempGraph, hotendTempPoints);

        xOffset += xSpacing;

        // Limit number of points to avoid memory overflow
        if (flowRatePoints.Count > 100)
        {
            flowRatePoints.RemoveAt(0);
            lateralSpeedPoints.RemoveAt(0);
            zOffsetPoints.RemoveAt(0);
            hotendTempPoints.RemoveAt(0);
        }
    }

    private void UpdateLineRenderer(LineRenderer lineRenderer, List<Vector3> points)
    {
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }
}
