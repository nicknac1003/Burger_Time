using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneManagerScript : MonoBehaviour
{
    // Singleton instance
    public static SceneManagerScript Instance;

    private void Awake()
    {
        // If an instance doesn't already exist, set it and persist across scenes.
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadStartScreen()
    {
        Debug.Log("Loading Start Screen");
        SceneManager.LoadScene(0);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(1);
    }
}
