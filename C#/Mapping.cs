using UnityEngine;

public class Mapping : MonoBehaviour
{
    public Transform origin;            // Custom Origin (global reference point)
    public Transform referencePoint;    // The main positional anchor (nozzle tip)
    public Transform printHead;         // Print head moves along X-axis
    public Transform bed;               // Bed moves along Y-axis
    public Transform zAxisComponent;    // Z-axis component moves along Z-axis
    public DoubleVector3 targetPosition; // Target position from G-code (double precision)
    public double speed = 5.0;           // Speed factor for smooth movement (double precision)

    private DoubleVector3 printHeadTargetPos;
    private DoubleVector3 bedTargetPos;
    private DoubleVector3 zAxisTargetPos;

    void Start()
    {
        // Set initial target position
        SetTargetPosition(20.4, 235, 118,speed); 
    }

    void Update()
    {
        // Smoothly move each component toward its target position
        MoveComponentToReference();
    }

    public void SetTargetPosition(double x, double y, double z, double speed)
    {
        // Set target position in double precision
        targetPosition = new DoubleVector3(x, y, z);
        Debug.Log($"Reference Point Position (relative to Custom Origin): {referencePoint.localPosition}");
        Debug.Log($"Reference Point Position (relative to Origin): {referencePoint.position}");
        
        // Set target positions based on mapping logic using double precision
        printHeadTargetPos = new DoubleVector3(referencePoint.localPosition.x, referencePoint.localPosition.y, -targetPosition.z);
        bedTargetPos = new DoubleVector3(bed.localPosition.x, 415.0 - targetPosition.y, bed.localPosition.z);
        zAxisTargetPos = new DoubleVector3(220.0 - targetPosition.x, zAxisComponent.localPosition.y, zAxisComponent.localPosition.z);
    }

    public void MoveComponentToReference()
    {
        // Move each component smoothly toward its calculated target position using double precision

        // PrintHead movement
        DoubleVector3 newPrintHeadPosition = DoubleVector3.MoveTowards(new DoubleVector3(referencePoint.localPosition.x, referencePoint.localPosition.y, referencePoint.localPosition.z), 
                                                                       printHeadTargetPos, speed * Time.deltaTime);
        referencePoint.localPosition = (Vector3)newPrintHeadPosition;

        // Bed movement
        DoubleVector3 newBedPosition = DoubleVector3.MoveTowards(new DoubleVector3(bed.localPosition.x, bed.localPosition.y, bed.localPosition.z), 
                                                                 bedTargetPos, speed * Time.deltaTime);
        bed.localPosition = (Vector3)newBedPosition;

        // Z-Axis movement
        DoubleVector3 newZaxisPosition = DoubleVector3.MoveTowards(new DoubleVector3(zAxisComponent.localPosition.x, zAxisComponent.localPosition.y, zAxisComponent.localPosition.z), 
                                                                   zAxisTargetPos, speed * Time.deltaTime);
        zAxisComponent.localPosition = (Vector3)newZaxisPosition;

        Debug.Log($"Printhead position (relative to Custom Origin): \n X = {-referencePoint.localPosition.z}, Y =  {-bed.localPosition.y + 415.0}, Z = {-zAxisComponent.localPosition.x + 220.0}");
    }
}
