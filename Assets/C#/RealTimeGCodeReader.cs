using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RealTimeGCodeReader : MonoBehaviour
{
    public string logFilePath = "C:\\OctoPrint\\basedir\\5000\\logs\\serial.log";
    public Queue<GCodeCommand> commandQueue = new Queue<GCodeCommand>();

    // Temperature variables
    public float NozzleTemperature { get; private set; } = 0f;
    public float BedTemperature { get; private set; } = 0f;

    private GenericGCodeParser parser;
    private CancellationTokenSource cancellationTokenSource;
    private long lastFilePosition = 0; // Track the last file position

    void Start()
    {
        parser = new GenericGCodeParser();
        cancellationTokenSource = new CancellationTokenSource();

        // Initialize lastFilePosition to the end of the file to start reading new entries only
        if (File.Exists(logFilePath))
        {
            lastFilePosition = new FileInfo(logFilePath).Length;
        }
        else
        {
            Debug.LogError($"Log file not found at path: {logFilePath}");
            return;
        }

        StartReadingLog(cancellationTokenSource.Token);
    }

    private void StartReadingLog(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    long fileLength = new FileInfo(logFilePath).Length;

                    if (fileLength < lastFilePosition)
                    {
                        // File was truncated or rotated; set lastFilePosition to current length
                        lastFilePosition = fileLength;
                    }

                    if (fileLength > lastFilePosition)
                    {
                        using (var fileStream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            fileStream.Seek(lastFilePosition, SeekOrigin.Begin);
                            using (var reader = new StreamReader(fileStream))
                            {
                                while (!reader.EndOfStream)
                                {
                                    string line = reader.ReadLine();
                                    lastFilePosition = fileStream.Position;

                                    // Process the line
                                    if (!string.IsNullOrEmpty(line))
                                    {
                                        ProcessLogLine(line);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // No new data; wait before checking again
                        await Task.Delay(100, cancellationToken);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Task was canceled; do any cleanup if necessary
            }
            catch (Exception ex)
            {
                Debug.LogError("Error during continuous file reading: " + ex.Message);
            }
        }, cancellationToken);
    }

    private void ProcessLogLine(string line)
    {
        // Trim the line to remove any leading/trailing whitespace
        line = line.Trim();

        // Find the last colon ':' in the line
        int colonIndex = line.LastIndexOf(':');
        if (colonIndex == -1 || colonIndex == line.Length - 1)
        {
            // Ignore lines without colon or command
            return;
        }

        // Extract the command after the last colon
        string commandPart = line.Substring(colonIndex + 1).Trim();

        // Check if the commandPart is empty
        if (string.IsNullOrEmpty(commandPart))
        {
            // Ignore lines with empty command
            return;
        }

        // Define valid G-code command prefixes
        string[] validGCodePrefixes = { "G", "M", "T", "N" };

        // Check if the commandPart starts with a valid G-code prefix
        bool isValidGCode = false;
        foreach (var prefix in validGCodePrefixes)
        {
            if (commandPart.StartsWith(prefix))
            {
                isValidGCode = true;
                break;
            }
        }

        if (isValidGCode)
        {
            // If the command starts with 'N', remove 'N' and the number after it
            if (commandPart.StartsWith("N"))
            {
                int spaceIndex = commandPart.IndexOf(' ');
                if (spaceIndex > 0)
                {
                    commandPart = commandPart.Substring(spaceIndex + 1).Trim();
                }
                else
                {
                    // No space after 'N', invalid command, ignore it
                    return;
                }
            }

            // Now, remove anything after '*' (including the '*')
            int asteriskIndex = commandPart.IndexOf('*');
            if (asteriskIndex >= 0)
            {
                commandPart = commandPart.Substring(0, asteriskIndex).Trim();
            }

            EnqueueGCodeCommand(commandPart);
        }
        else if (commandPart.StartsWith("T:") || commandPart.StartsWith("B:"))
        {
            // Handle temperature updates
            UpdateTemperature(commandPart);
        }
        else
        {
            // Ignore non-G-code line
        }
    }

    private void EnqueueGCodeCommand(string commandLine)
    {
        try
        {
            GCodeCommand command = parser.ParseLine(commandLine, 0);
            if (command != null)
            {
                lock (commandQueue)
                {
                    commandQueue.Enqueue(command);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing command: " + ex.Message + " | Command content: " + commandLine);
        }
    }

    private void UpdateTemperature(string line)
    {
        try
        {
            // Example line: "T:200.00 /200.00 B:60.00 /60.00 @:0 B@:0"
            string[] parts = line.Split(' ');

            foreach (string part in parts)
            {
                if (part.StartsWith("T:"))
                {
                    string tempStr = part.Substring(2).Split('/')[0]; // Extract current temperature before '/'
                    NozzleTemperature = float.Parse(tempStr, CultureInfo.InvariantCulture);
                }
                else if (part.StartsWith("B:"))
                {
                    string tempStr = part.Substring(2).Split('/')[0];
                    BedTemperature = float.Parse(tempStr, CultureInfo.InvariantCulture);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error parsing temperature line: " + ex.Message + " | Line content: " + line);
        }
    }

    private void OnDestroy()
    {
        if (cancellationTokenSource != null)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
