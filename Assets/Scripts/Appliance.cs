using UnityEngine;

public class Appliance : Interactable
{
    [SerializeField] private Breakable breakable;

    [SerializeReference] private QuickTimeEvent useApplianceQTE = null;

    private bool working  = true;
    private bool zPressed = false;

    public override void InteractZ(bool pressed) => zPressed = pressed;

    void Update()
    {
        if(breakable.CanBreak())
        {
            breakable.HandleBreaking();
            breakable.HandleRepairing(zPressed, this);
            working = breakable.IsBroken() == false;
        }   

        if(working)
        {
            useApplianceQTE.PerformQTE(zPressed, false, false, Vector2.zero, this);

            // print out score from QTE once finished
            //if(useApplianceQTE.InProgress() == false)
            //{
            //    //
            //}
        }
    }
}