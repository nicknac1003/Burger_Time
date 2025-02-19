using UnityEngine;

public class Appliance : Storage
{
    [SerializeField] private Breakable breakable;

    [Tooltip("The QuickTimeEvent to use when interacting with this appliance.")]
    [SerializeReference] private QuickTimeEvent useApplianceQTE = new SliderQTE();

    private bool working  = true;
    private bool zPressed = false;
    private bool xPressed = false;
    private bool cPressed = false;

    public bool InUse() => useApplianceQTE.InProgress();
    public override void InteractZ(bool pressed) => zPressed = pressed;
    public override void InteractX(bool pressed) => xPressed = pressed;
    public override void InteractC(bool pressed) => cPressed = pressed;

    void Update()
    {
        if(breakable.CanBreak())
        {
            breakable.HandleBreaking(this);
            breakable.HandleRepairing(zPressed, this);
            working = breakable.IsBroken() == false;
        }   

        if(working)
        {
            if(useApplianceQTE.PerformQTE(zPressed, xPressed, cPressed, Vector2.zero, this) > 0f)
            {
                Debug.Log("Success!");
            }
        }
    }
}