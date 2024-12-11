using UnityEngine;

public class Printer_Controller : MonoBehaviour
{
    public Transform printHead;          // The Print Head that moves along the X-axis
    public Transform zAxisComponent;     // The Z-axis component that moves along the Z-axis (vertical)
    public Transform bed;                // The Bed that moves along the Y-axis
    public float speed = 5.0f;           // Speed for all movements

    void Update()
    {
        // Move the Print Head along the X-axis (left and right arrow keys)
        float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        printHead.Translate(-moveX, 0, 0);

        // Move the Z-axis component along the Z-axis (W and S keys)
        if (Input.GetKey(KeyCode.W))
        {
            zAxisComponent.Translate(0, 0, -speed * Time.deltaTime);  // Move up (Z-axis)
        }
        if (Input.GetKey(KeyCode.S))
        {
            zAxisComponent.Translate(0, 0, speed * Time.deltaTime);  // Move down (Z-axis)
        }

        // Move the Bed along the Y-axis (up and down arrow keys)
        if (Input.GetKey(KeyCode.UpArrow))
        {
            bed.Translate(0, speed * Time.deltaTime, 0);  // Move up (Y-axis)
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            bed.Translate(0, -speed * Time.deltaTime, 0);  // Move down (Y-axis)
        }
    }
}
