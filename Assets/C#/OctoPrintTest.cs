using UnityEngine;
using OctoprintClient;
using Newtonsoft.Json.Linq;

public class OctoPrintTemperatureTest : MonoBehaviour
{
    private OctoprintConnection connection;

    private string octoprintURL = "http://127.0.0.1:5000";
    private string apiKey = "5D60CE27902F4486AFD8112B24449923";

    void Start()
    {
        connection = new OctoprintConnection(octoprintURL, apiKey);
        GetTemperatureData();
        //HomePrinthead();
        
    }

    private void GetTemperatureData()
    {
        try
        {
            if (connection == null)
            {
                Debug.LogError("Connection is null.");
                return;
            }

            var printerTracker = connection.Printers;
            if (printerTracker == null)
            {
                Debug.LogError("Printer tracker is null.");
                return;
            }

            // Manually fetch the temperature information using a direct API call
            string response = connection.Get("api/printer?exclude=state,sd"); // Exclude other data
            if (!string.IsNullOrEmpty(response))
            {
                Debug.Log("Raw Temperature Data: " + response);

                JObject data = JObject.Parse(response);
                if (data["temperature"] != null)
                {
                    var bedTemp = data["temperature"]["bed"]["actual"];
                    var nozzleTemp = data["temperature"]["tool0"]["actual"];

                    Debug.Log("Bed Temperature: " + bedTemp + "°C");
                    Debug.Log("Nozzle Temperature: " + nozzleTemp + "°C");
                }
                else
                {
                    Debug.LogError("Temperature data is missing in the response.");
                }
            }
            else
            {
                Debug.LogError("No response received from OctoPrint.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error retrieving temperature data: " + ex.Message);
        }
    }
    private void HomePrinthead()
    {
        try
        {
            if (connection == null)
            {
                Debug.LogError("Connection is null.");
                return;
            }

            var printerTracker = connection.Printers;
            if (printerTracker == null)
            {
                Debug.LogError("Printer tracker is null.");
                return;
            }

            // Send the printhead to the home position on all axes
            string[] axes = { "x", "y", "z" }; // Specify which axes to home
            string response = printerTracker.MakePrintheadHome(axes);

            // Display response from the server
            if (!string.IsNullOrEmpty(response))
            {
                Debug.Log("Home Command Response: " + response);
            }
            else
            {
                Debug.LogError("Failed to send home command or received an empty response.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error sending home command: " + ex.Message);
        }
    }
        private void MoveToAbsolutePosition(DoubleVector3 position, int feedRate)
    {
        try
        {
            if (connection == null)
            {
                Debug.LogError("Connection is null.");
                return;
            }

            var printerTracker = connection.Printers;
            if (printerTracker == null)
            {
                Debug.LogError("Printer tracker is null.");
                return;
            }

            // Send the command to move the printhead to the specified precise coordinates
            bool isAbsolute = true;
            string response = printerTracker.MakePrintheadJog((float)position.x, (float)position.y, (float)position.z, isAbsolute, feedRate);

            // Display response from the server
            if (!string.IsNullOrEmpty(response))
            {
                Debug.Log("Move Command Response: " + response);
            }
            else
            {
                Debug.LogError("Failed to send move command or received an empty response.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error sending move command: " + ex.Message);
        }
    }
}
