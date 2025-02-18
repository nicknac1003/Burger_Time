using UnityEngine;

public class AlternateQTE : QuickTimeEvent
{
    public override bool InProgress()
    {
        throw new System.NotImplementedException();
    }

    public override float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent)
    {
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }
    public override void EndQTE(Interactable parent)
    {
        throw new System.NotImplementedException();
    }
}