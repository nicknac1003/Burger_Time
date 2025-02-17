using UnityEngine;

[System.Serializable]
public class HoldQTE : QuickTimeEvent
{
    [SerializeField] private float time  = 4f;   // How long it takes to repair
    [SerializeField] private float drain = 0.5f; // Speed progress depletes when not holding
    private float progress;

    private float fillSpeed  = 1f; // Multiplier for how fast progress fills
    private float drainSpeed = 1f; // Multiplier for how fast progress depletes

    public HoldQTE() // default constructor
    {
        time     = 4f;
        drain    = 0.5f;
        progress = 0f;
    }

    public HoldQTE(float holdTime, float drainRate)
    {
        time     = holdTime;
        drain    = drainRate;
        progress = 0f;
    }

    public void SetFillSpeed(float speed)  => fillSpeed = speed;
    public void ResetFillSpeed()           => fillSpeed = 1f;
    public void SetDrainSpeed(float speed) => drainSpeed = speed;
    public void ResetDrainSpeed()          => drainSpeed = 1f;

    public override float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput)
    {
        if(zPressed == false)
        {
            if(progress > 0f)
            {
                progress = Mathf.Max(progress - Time.deltaTime * drain * drainSpeed, 0f); // deplete progress by repairDrain per second

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

        progress += Time.deltaTime * fillSpeed;

        return 0f;
    }

    public override bool InProgress()
    {
        return progress < time;
    }
}