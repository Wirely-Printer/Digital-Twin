using UnityEngine;

public class DeepLearning_PanelSwitcher : MonoBehaviour
{
    public GameObject panelA; // Reference to Panel A
    public GameObject panelB; // Reference to Panel B

    private bool isPanelAActive = true; // Track which panel is currently active

    void Start()
    {
        // Ensure the initial state: Panel A active, Panel B inactive
        if (panelA != null && panelB != null)
        {
            panelA.SetActive(true);
            panelB.SetActive(false);
        }
        else
        {
            Debug.LogError("Panel references not assigned in the Inspector!");
        }
    }

    public void TogglePanels()
    {
        if (panelA != null && panelB != null)
        {
            if (isPanelAActive)
            {
                // Deactivate Panel A and activate Panel B
                panelA.SetActive(false);
                panelB.SetActive(true);
            }
            else
            {
                // Deactivate Panel B and activate Panel A
                panelB.SetActive(false);
                panelA.SetActive(true);
            }

            // Toggle the state
            isPanelAActive = !isPanelAActive;
        }
        else
        {
            Debug.LogError("Panel references not assigned in the Inspector!");
        }
    }
}
