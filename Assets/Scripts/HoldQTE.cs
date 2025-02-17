using UnityEngine;

[System.Serializable]
public class HoldQTE : QuickTimeEvent
{
    [SerializeField] private float time  = 4f;   // How long it takes to repair
    [SerializeField] private float drain = 0.5f; // If stopped repairing, speed progress depletes
    private float progress;

    public HoldQTE(float holdTime, float drainRate)
    {
        time     = holdTime;
        drain    = drainRate;
        progress = 0f;
    }

    public override float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput)
    {
        if(zPressed == false)
        {
            if(progress > 0f)
            {
                progress = Mathf.Max(progress - Time.deltaTime * drain, 0f); // deplete progress by repairDrain per second

                Debug.Log("Progress: " + progress + " / " + time);
            }
            return 0f;
        }

        Debug.Log("Progress: " + progress + " / " + time);

        if(progress >= time)
        {
            progress = 0f;
            return 1f;
        }

        progress += Time.deltaTime;

        return 0f;
    }
}