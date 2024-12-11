using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiSceneLoader : MonoBehaviour
{
    void Start()
    {
        // Load the Simulation Scene (SampleScene) additively
        if (!SceneManager.GetSceneByName("SampleScene").isLoaded)
        {
            SceneManager.LoadScene("SampleScene", LoadSceneMode.Additive);
        }

        // Load the UI Scene (UIScene) additively
        if (!SceneManager.GetSceneByName("UIScene").isLoaded)
        {
            SceneManager.LoadScene("UIScene", LoadSceneMode.Additive);
        }
    }
}
