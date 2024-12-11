using UnityEngine;
using Radishmouse; // Ensure UILineRenderer is imported

public class BasicGraphTest : MonoBehaviour
{
    public UILineRenderer uiLineRenderer; // Assign the UILineRenderer in the Inspector

    void Start()
    {
        if (uiLineRenderer == null)
        {
            Debug.LogError("UILineRenderer is not assigned!");
            return;
        }

        // Set static points manually (hardcoded for simplicity)
        uiLineRenderer.points = new Vector2[]
        {
            new Vector2(0, 0),       // Start at bottom-left
            new Vector2(50, 50),     // Move up diagonally
            new Vector2(100, 20),    // Slightly down
            new Vector2(150, 75),    // Up again
            new Vector2(200, 100)    // Top-right
        };

        // Set the line thickness and color
        uiLineRenderer.thickness = 2f;
        uiLineRenderer.color = Color.red;

        Debug.Log("Basic graph rendered with static points.");
    }
}
