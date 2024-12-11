using UnityEngine;
using UnityEngine.UI;

public class CameraFeed_General : MonoBehaviour
{
    public RawImage rawImage;         // The RawImage to display the feed
    public Texture2D fallbackTexture; // Fallback image if the camera feed fails
    public AspectRatioFitter aspectFitter; // Optional: Maintain aspect ratio for the video feed

    private WebCamTexture webcamTexture;

    void Start()
    {
        // Check for available cameras
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            try
            {
                // Use the first available camera
                string selectedCamera = devices[1].name;

                // Initialize the WebCamTexture
                webcamTexture = new WebCamTexture(selectedCamera);
                rawImage.texture = webcamTexture; // Assign the webcam feed to the RawImage
                webcamTexture.Play(); // Start the camera

                // Adjust the aspect ratio to match the feed
                if (aspectFitter != null)
                {
                    aspectFitter.aspectRatio = (float)webcamTexture.width / webcamTexture.height;
                }

                Debug.Log("Camera feed started successfully.");
            }
            catch
            {
                Debug.LogError("Failed to start camera feed. Using fallback image.");
                UseFallbackImage();
            }
        }
        else
        {
            Debug.LogError("No camera detected. Using fallback image.");
            UseFallbackImage();
        }
    }

    private void UseFallbackImage()
    {
        // Assign the fallback image to the RawImage
        rawImage.texture = fallbackTexture;

        // Adjust the aspect ratio to match the fallback image
        if (aspectFitter != null && fallbackTexture != null)
        {
            aspectFitter.aspectRatio = (float)fallbackTexture.width / fallbackTexture.height;
        }
    }

    void OnDisable()
    {
        // Stop the webcam feed if it's playing
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamTexture.Stop();
        }
    }
}
