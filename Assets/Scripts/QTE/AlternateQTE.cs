using UnityEngine;

public class AlternameQTE : QuickTimeEvent
{
    public override float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent)
    {
        return 0;
    }

    public override bool InProgress()
    {
        return false;
    }

    public override void CreateUI(Transform parent)
    {
        return;
    }

    public override void DestroyUI()
    {
        return;
    }

    public override void Cancel()
    {
        throw new System.NotImplementedException();
    }
}