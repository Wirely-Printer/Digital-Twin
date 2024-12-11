using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Math = System.Math;

public class GCodeExecutor : MonoBehaviour
{
    public Transform origin;
    public Transform referencePoint;
    public Transform printHead;
    public Transform bed;
    public Transform zAxisComponent;

    private RealTimeGCodeReader logReader; // Reference to RealTimeGCodeReader
    private Queue<GCodeCommand> commandQueue; // Reference to the shared commandQueue in RealTimeGCodeReader
    private DoubleVector3 printHeadTargetPos;
    private DoubleVector3 bedTargetPos;
    private DoubleVector3 zAxisTargetPos;
    private double movementSpeed;
    private const double G0_RapidSpeed = 4800.0 / 60.0;
    private const double HomingSpeed = 1500.0 / 60;
    private const double ZAxisSpeedMultiplier = 1.0;
    private double lastFeedRate = 1500.0;
    private bool isExecuting = false;

    private double xSpeed = 0; // Speed for X-axis
    private double ySpeed = 0; // Speed for Y-axis
    private double zSpeed = 0; // Speed for Z-axis
    private const double DefaultAcceleration_XY = 500.0; // Default acceleration for all axes
    private const double DefaultAcceleration_Z = 100.0; // Default acceleration for Z-axis


    void Start()
    {
        logReader = gameObject.AddComponent<RealTimeGCodeReader>(); // Initialize RealTimeGCodeReader
        commandQueue = logReader.commandQueue; // Reference the command queue from RealTimeGCodeReader
        Debug.Log("GCodeExecutor initialized and ready to process commands.");
    }

    void FixedUpdate()
    {
        // Continuously check and execute commands if available
        if (!isExecuting && commandQueue.Count > 0)
        {
            ExecuteNextCommand();
            //Thread.Sleep(1500);
        }

        // Smoothly move each component toward its target position
        MoveComponents();

        // Check if target positions are reached, then execute the next command
        if (HasReachedTargetPositions() && isExecuting)
        {
            isExecuting = false; // Mark command as complete
            ExecuteNextCommand(); // Execute the next command if available
        }
    }

    void ExecuteNextCommand()
    {
        if (commandQueue.Count == 0)
        {
            Debug.Log("All commands executed for now.");
            return;
        }

        GCodeCommand command = commandQueue.Dequeue();
        isExecuting = true;

        if (command.CommandType == "G1")
        {
            SetTargetPosition(command, isRapid: false);
        }
        else if (command.CommandType == "G0")
        {
            SetTargetPosition(command, isRapid: true);
        }
        else if (command.CommandType == "G28")
        {
            MoveToHomePosition();
            return;
        }

        Debug.Log($"Executing Command: {command.CommandType} X:{command.X} Y:{command.Y} Z:{command.Z} FeedRate:{command.FeedRate}");
    }

void MoveToHomePosition()
{
    // Define homing commands
    List<GCodeCommand> homingCommands = new List<GCodeCommand>
    {
        new GCodeCommand { CommandType = "G1", X = 0, FeedRate = 10000 },
        new GCodeCommand { CommandType = "G1", Y = 0, FeedRate = 10000 },
        new GCodeCommand { CommandType = "G1", Z = 1, FeedRate = 50000 },
        new GCodeCommand { CommandType = "G1", Z = 0, FeedRate = 50000 }
    };

    lock (commandQueue)
    {
        // Dequeue all existing commands into a temporary list
        List<GCodeCommand> existingCommands = new List<GCodeCommand>();
        while (commandQueue.Count > 0)
        {
            existingCommands.Add(commandQueue.Dequeue());
        }

        // Enqueue homing commands first
        foreach (var cmd in homingCommands)
        {
            commandQueue.Enqueue(cmd);
        }

        // Enqueue the existing commands after
        foreach (var cmd in existingCommands)
        {
            commandQueue.Enqueue(cmd);
        }
    }

    ExecuteNextCommand();
}


    void SetTargetPosition(GCodeCommand command, bool isRapid)
    {
        if (!double.IsNaN(command.FeedRate) && command.FeedRate > 0)
        {
            lastFeedRate = command.FeedRate;
        }

        movementSpeed = isRapid ? G0_RapidSpeed : lastFeedRate / 60.0;

        if (!double.IsNaN(command.X))
        {
            printHeadTargetPos = new DoubleVector3(referencePoint.localPosition.x, referencePoint.localPosition.y, -command.X);
        }
        else
        {
            printHeadTargetPos = referencePoint.localPosition;
        }

        if (!double.IsNaN(command.Y))
        {
            bedTargetPos = new DoubleVector3(bed.localPosition.x, 415.0 - command.Y, bed.localPosition.z);
        }
        else
        {
            bedTargetPos = bed.localPosition;
        }

        if (!double.IsNaN(command.Z))
        {
            zAxisTargetPos = new DoubleVector3(220.0 - command.Z, zAxisComponent.localPosition.y, zAxisComponent.localPosition.z);
        }
        else
        {
            zAxisTargetPos = zAxisComponent.localPosition;
        }

        Debug.Log($"Setting Target Position: X={command.X}, Y={command.Y}, Z={command.Z}, Speed={movementSpeed} mm/s");
    }

void MoveComponents()
{
    // Calculate remaining distances
    double remainingDistanceX = Math.Abs(printHeadTargetPos.z - referencePoint.localPosition.z);
    double remainingDistanceY = Math.Abs(bedTargetPos.y - bed.localPosition.y);
    double remainingDistanceZ = Math.Abs(zAxisTargetPos.x - zAxisComponent.localPosition.x);

    // Calculate diagonal distance for synchronization
    double totalDistance = Math.Sqrt(
        Math.Pow(remainingDistanceX, 2) +
        Math.Pow(remainingDistanceY, 2) +
        Math.Pow(remainingDistanceZ, 2)
    );

    // Avoid division by zero for zero total distance
    if (totalDistance == 0)
    {
        Debug.Log("All axes have reached their target positions.");
        return;
    }

    // Calculate scaling factors for each axis
    double scalingFactorX = remainingDistanceX / totalDistance;
    double scalingFactorY = remainingDistanceY / totalDistance;
    double scalingFactorZ = remainingDistanceZ / totalDistance;

    

    // Update speeds dynamically
    xSpeed = UpdateSpeed(xSpeed, movementSpeed * scalingFactorX, remainingDistanceX, DefaultAcceleration_XY);
    ySpeed = UpdateSpeed(ySpeed, movementSpeed * scalingFactorY, remainingDistanceY, DefaultAcceleration_XY);

    if (movementSpeed == 50000.0 / 60.0) // Homing speed exception
    {
        zSpeed = UpdateSpeed(zSpeed, movementSpeed, remainingDistanceZ, DefaultAcceleration_Z);
    }
    else
    {
        zSpeed = UpdateSpeed(zSpeed, Math.Min(movementSpeed * ZAxisSpeedMultiplier, 5.0) * scalingFactorZ, remainingDistanceZ, DefaultAcceleration_Z);
    }

    // Move components
    referencePoint.localPosition = (Vector3)DoubleVector3.MoveTowards(referencePoint.localPosition, printHeadTargetPos, xSpeed * Time.deltaTime);
    bed.localPosition = (Vector3)DoubleVector3.MoveTowards(bed.localPosition, bedTargetPos, ySpeed * Time.deltaTime);
    zAxisComponent.localPosition = (Vector3)DoubleVector3.MoveTowards(zAxisComponent.localPosition, zAxisTargetPos, zSpeed * Time.deltaTime);

//     // Debugging remaining distances, scaling factors, and speeds
//     Debug.Log($"Remaining Distances -> X: {remainingDistanceX}, Y: {remainingDistanceY}, Z: {remainingDistanceZ}");
//     Debug.Log($"Scaling Factors -> X: {scalingFactorX:F2}, Y: {scalingFactorY:F2}, Z: {scalingFactorZ:F2}");
//     Debug.Log($"Speeds -> X: {xSpeed:F2}, Y: {ySpeed:F2}, Z: {zSpeed:F2}");
    }



    bool HasReachedTargetPositions()
    {
        return referencePoint.localPosition == (Vector3)printHeadTargetPos &&
               bed.localPosition == (Vector3)bedTargetPos &&
               zAxisComponent.localPosition == (Vector3)zAxisTargetPos;
    }
    double UpdateSpeed(double currentSpeed, double maxSpeed, double remainingDistance, double acceleration)
{
    double stoppingDistance = Math.Pow(currentSpeed, 2) / (2 * acceleration);

    if (remainingDistance <= stoppingDistance)
    {
        // Decelerate to stop
        return Math.Max(currentSpeed - acceleration * Time.deltaTime, 0);
    }
    else if (currentSpeed < maxSpeed)
    {
        // Accelerate toward the target speed
        return Math.Min(currentSpeed + acceleration * Time.deltaTime, maxSpeed);
    }
    else
    {
        // Maintain current speed
        return maxSpeed;
    }
}

}