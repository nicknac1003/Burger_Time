using UnityEngine;

public class CookQTE : QuickTimeEvent {

    public float maxCookTime = 20f;

    public float cookTimer = 0f;

    public bool isCooking = false;

    public CookQTE() : base(false) { }

    public override bool InProgress()
    {
        return cookTimer > 0f;
    }

    protected override float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent)
    {
        if(isCooking == true && zPressed) 
            return cookTimer / maxCookTime;
        
        if(isCooking == false && zPressed)
            StartQTE(parent);

        if(isCooking == true && zPressed == false){
            cookTimer += Time.deltaTime;
        }


        return 0f;
    }

    protected override void CreateUI(Transform anchor)
    {
        throw new System.NotImplementedException();
    }

    protected override void DestroyUI()
    {
        throw new System.NotImplementedException();
    }

    protected override void StartQTE(Interactable parent)
    {
        isCooking = true;
    }

    public override void EndQTE(Interactable parent)
    {
        throw new System.NotImplementedException();
    }
}