using UnityEngine;

public class AnimatorScript : MonoBehaviour
{
    // This butt ass script ONLY calls functions on player controller but it's easier than keeping the animator controller on the player!!!!!!!!!!!
    public void LockPlayer()
    {
        PlayerController.LockPlayer();
    }
    public void UnlockPlayer()
    {
        PlayerController.UnlockPlayer();
    }

    public void SetPlayerDirection(int dir)
    {
        PlayerController.Instance.SetPlayerDirection(dir);
    }
}
