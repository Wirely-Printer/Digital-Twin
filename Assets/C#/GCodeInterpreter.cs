using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using gs;

public class GCodeInterpreter : MonoBehaviour
{
    public string gcodeFilePath = "Assets/GCode/test.gcode"; // Set the path to your G-code file
    private Queue<GCodeCommand> commandQueue; // Queue to store parsed commands (G0, G1, G28)
    private GenericGCodeParser parser;

    // Method to initialize and parse commands from the file
    public void InitializeAndParse()
    {
        parser = new GenericGCodeParser();
        commandQueue = new Queue<GCodeCommand>();

        // Verify the G-code file path and parse it
        if (!File.Exists(gcodeFilePath))
        {
            Debug.LogError("G-code file not found: " + gcodeFilePath);
            return;
        }

        // Read and parse the G-code file
        ParseGCodeFile(gcodeFilePath);
        Debug.Log("Finished parsing G-code file.");
    }

    // Method to parse the G-code file and enqueue relevant commands
    private void ParseGCodeFile(string filePath)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            int lineNumber = 0;
            while (reader.Peek() >= 0)
            {
                string line = reader.ReadLine();
                lineNumber++;

                GCodeCommand command = parser.ParseLine(line, lineNumber);

                // Filter for only G0, G1, and G28 commands
                if (command != null && (command.CommandType == "G0" || command.CommandType == "G1" || command.CommandType == "G28"))
                {
                    commandQueue.Enqueue(command);
                }
            }
        }
    }

    // Public method to return the parsed commands as a list or queue
    public Queue<GCodeCommand> GetParsedCommands()
    {
        return new Queue<GCodeCommand>(commandQueue); // Return a copy of the queue
    }
}
