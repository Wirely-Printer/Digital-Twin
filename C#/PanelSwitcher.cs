using UnityEngine;

public class PanelSwitcher : MonoBehaviour
{
    public GameObject panelA; // Reference to Panel A
    public GameObject panelB; // Reference to Panel B

    public void SwitchToPanelB()
    {
        panelA.SetActive(false); // Hide Panel A
        panelB.SetActive(true);  // Show Panel B
    }

    public void SwitchToPanelA()
    {
        panelB.SetActive(false); // Hide Panel B
        panelA.SetActive(true);  // Show Panel A
    }
}
