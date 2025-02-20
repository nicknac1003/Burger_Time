using UnityEngine;

public class Appliance : Storage
{
    [SerializeField] private Breakable breakable;

    [Tooltip("The QuickTimeEvent to use when interacting with this appliance.")]
    [SerializeReference] private QuickTimeEvent useApplianceQTE = new SliderQTE();

    private bool working  = true;

    public bool InUse() => useApplianceQTE.InProgress();

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
            if(useApplianceQTE.QTE(zPressed, xPressed, cPressed, moveInput, this) > 0f)
            {
                Debug.Log("Success!");
            }
        }
    }
}