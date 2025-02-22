using UnityEngine;

public class AnimatorScript : MonoBehaviour
{
    // This butt ass script ONLY calls functions on player controller but it's easier than keeping the animator controller on the player!!!!!!!!!!!

    public void holdingPosDown()
    {
        PlayerController.Instance.holdingPosDown();
    }
    public void holdingPosUp()
    {
        PlayerController.Instance.holdingPosUp();
    }
    public void holdingPosLeft()
    {
        PlayerController.Instance.holdingPosLeft();
    }
    public void holdingPosRight()
    {
        PlayerController.Instance.holdingPosRight();
    }
}
