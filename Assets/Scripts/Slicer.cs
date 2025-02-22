using UnityEngine;

public class Slicer : Appliance {
    [SerializeReference] private QuickTimeEvent useApplianceQTE = new MashQTE();

    // private void Start() {
    //     useApplianceQTE = new MashQTE();
    // }


    protected override void Update()
    {
        base.Update();    

        if(working)
        {
            if(useApplianceQTE.QTE(zPressed, xPressed, cPressed, moveInput, this) > 0f)
            {
                Debug.Log("Success!");
            }
        }
    }
}