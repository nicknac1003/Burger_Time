using UnityEngine;
public class PauseScreen : MonoBehaviour
{
    public void LoadStart()
    {
        // Calls a method on the singleton instance
        SceneManagerScript.Instance.LoadStartScreen();
    }
    public void LoadGame()
    {
        // Calls a method on the singleton instance
        SceneManagerScript.Instance.LoadGameScene();
    }
}
