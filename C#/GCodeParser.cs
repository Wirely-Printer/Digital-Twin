using System;
using System.Collections.Generic;
using System.IO;

public class GenericGCodeParser
{
    // Method to parse an entire G-code file and return a list of relevant GCodeCommand objects
    public List<GCodeCommand> Parse(TextReader input)
    {
        List<GCodeCommand> commands = new List<GCodeCommand>();
        int lineNumber = 0;

        while (input.Peek() >= 0)
        {
            string line = input.ReadLine();
            lineNumber++;

            GCodeCommand command = ParseLine(line, lineNumber);
            if (command != null)
            {
                commands.Add(command); // Add only relevant commands (G0, G1, G28)
            }
        }

        return commands;
    }

    // Method to parse a single line and return a GCodeCommand object
    public GCodeCommand ParseLine(string line, int lineNumber)
    {
        if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";"))
            return null; // Ignore empty lines or comments

        string[] tokens = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length == 0) return null;

        // Initialize GCodeCommand object
        GCodeCommand command = new GCodeCommand
        {
            CommandType = tokens[0],
            Code = (tokens[0].Length > 1) ? int.Parse(tokens[0].Substring(1)) : 0
        };

        // Only process relevant commands: G0, G1, and G28
        if (command.CommandType != "G0" && command.CommandType != "G1" && command.CommandType != "G28")
            return null;

        // Parse the command parameters (X, Y, Z, F)
        for (int i = 1; i < tokens.Length; i++)
        {
            string token = tokens[i];
            double value;

            // Try parsing each parameter; skip if parsing fails
            if (token.StartsWith("X") && double.TryParse(token.Substring(1), out value))
            {
                command.X = value;
            }
            else if (token.StartsWith("Y") && double.TryParse(token.Substring(1), out value))
            {
                command.Y = value;
            }
            else if (token.StartsWith("Z") && double.TryParse(token.Substring(1), out value))
            {
                command.Z = value;
            }
            else if (token.StartsWith("F") && double.TryParse(token.Substring(1), out value))
            {
                command.FeedRate = value;
            }
        }

        return command;
    }
}
