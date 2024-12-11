using UnityEngine;
using OctoprintClient;

public class OctoprintManager : MonoBehaviour
{
    public static OctoprintConnection Instance { get; private set; }

    [Header("OctoPrint Connection Settings")]
    public string octoprintURL = "http://127.0.0.1:5000";
    public string apiKey = "5D60CE27902F4486AFD8112B24449923";

    void Awake()
    {
        // Ensure there's only one instance of this manager
        if (Instance == null)
        {
            Instance = new OctoprintConnection(octoprintURL, apiKey);
            DontDestroyOnLoad(gameObject); // Persist the manager across scenes
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate managers
        }
    }
}
