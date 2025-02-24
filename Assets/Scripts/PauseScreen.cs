using UnityEngine;
public class PauseScreen : MonoBehaviour
{
    public void LoadStart()
    {
        // Calls a method on the singleton instance
        SceneManagerScript.Instance.LoadStartScreen();
    }
}
