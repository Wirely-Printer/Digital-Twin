using UnityEngine;

public class CameraToggleController : MonoBehaviour
{
    public Camera feedCamera; // Reference to the original feed camera
    public Camera freeMoveCamera; // Reference to the 360 free-moving camera

    private bool is360ViewActive = false; // Toggle state

    public void ToggleView()
    {
        if (is360ViewActive)
        {
            // Switch to the original feed view
            freeMoveCamera.gameObject.SetActive(false);
            feedCamera.gameObject.SetActive(true);

            // Enable the audio listener on the feed camera
            freeMoveCamera.GetComponent<AudioListener>().enabled = false;
            feedCamera.GetComponent<AudioListener>().enabled = true;

            Debug.Log("Switched to Feed Camera");
        }
        else
        {
            // Switch to the 360 view
            feedCamera.gameObject.SetActive(false);
            freeMoveCamera.gameObject.SetActive(true);

            // Enable the audio listener on the 360 camera
            feedCamera.GetComponent<AudioListener>().enabled = false;
            freeMoveCamera.GetComponent<AudioListener>().enabled = true;

            Debug.Log("Switched to 360 Camera");
        }

        // Toggle the state
        is360ViewActive = !is360ViewActive;
    }
}
