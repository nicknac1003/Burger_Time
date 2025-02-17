using UnityEngine;

public class Appliance : Interactable
{
    [SerializeField] private Breakable breakable; // leave null if not breakable
    [SerializeField] private QuickTimeEvent useApplianceQTE;

    public override void InteractZ(bool held)
    {
        if(breakable != null && breakable.IsBroken())
        {
            breakable.SetRepairing(held);
        }
    }

    void Start()
    {
        breakable.Break();   
    }

    void Update()
    {
        if(breakable != null)
        {
            breakable.HandleBreaking();
            breakable.HandleRepairing();
        }
    }
}