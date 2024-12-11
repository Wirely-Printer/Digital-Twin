using UnityEngine;

public class MouseOrbitCamera : MonoBehaviour
{
    public float forwardSpeed = 1000.0f; // Speed for forward/backward movement
    public float sidewaysSpeed = 700.0f; // Speed for left/right movement
    public float verticalSpeed = 500.0f; // Speed for up/down movement
    public float rotationSpeed = 150.0f; // Mouse rotation speed

    private float yaw = 0.0f; // Horizontal rotation
    private float pitch = 0.0f; // Vertical rotation

    void Start()
    {
        // Initialize yaw and pitch based on the current camera rotation
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;
    }

    void Update()
    {
        // Handle mouse rotation
        if (Input.GetMouseButton(1)) // Right mouse button for rotation
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;

            // Clamp vertical rotation angle
            pitch = Mathf.Clamp(pitch, -80f, 80f);

            transform.rotation = Quaternion.Euler(pitch, yaw, 0);
        }

        // Handle keyboard movement with different speeds for each axis
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(KeyCode.W)) direction += transform.forward * forwardSpeed; // Move forward
        if (Input.GetKey(KeyCode.S)) direction -= transform.forward * forwardSpeed; // Move backward
        if (Input.GetKey(KeyCode.A)) direction -= transform.right * sidewaysSpeed;  // Move left
        if (Input.GetKey(KeyCode.D)) direction += transform.right * sidewaysSpeed;  // Move right
        if (Input.GetKey(KeyCode.E)) direction += transform.up * verticalSpeed;     // Move up
        if (Input.GetKey(KeyCode.Q)) direction -= transform.up * verticalSpeed;     // Move down

        // Move camera
        transform.position += direction * Time.deltaTime;
    }
}
