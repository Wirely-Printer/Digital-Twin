using UnityEngine;
using TMPro;

public class TerminalSwitcher : MonoBehaviour
{
    [Header("UI Components")]
    [Tooltip("Dropdown component to select panels.")]
    public TMP_Dropdown panelDropdown;

    [Tooltip("List of panels to switch between.")]
    public GameObject[] panels;

    void Start()
    {
        if (panelDropdown == null)
        {
            Debug.LogError("Panel Dropdown is not assigned.");
            return;
        }

        if (panels == null || panels.Length == 0)
        {
            Debug.LogError("No panels assigned to PanelSwitcher.");
            return;
        }

        // Ensure the dropdown has the correct number of options
        if (panelDropdown.options.Count != panels.Length)
        {
            Debug.LogWarning("The number of dropdown options doesn't match the number of panels.");
        }

        // Add listener for when the dropdown value changes
        panelDropdown.onValueChanged.AddListener(delegate { SwitchPanel(panelDropdown.value); });

        // Initialize the panels based on the starting value of the dropdown
        SwitchPanel(panelDropdown.value);
    }

    public void SwitchPanel(int index)
    {
        // Validate index
        if (index < 0 || index >= panels.Length)
        {
            Debug.LogError("Panel index out of range.");
            return;
        }

        // Activate the selected panel and deactivate others
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null)
            {
                panels[i].SetActive(i == index);
            }
            else
            {
                Debug.LogWarning($"Panel at index {i} is not assigned.");
            }
        }
    }
}
