using UnityEngine;

[System.Serializable]
public abstract class QuickTimeEvent
{
    public abstract float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent);
    public abstract bool  InProgress();
    public abstract void  CreateUI(Transform parent);
    public abstract void  DestroyUI();
    public abstract void  Cancel();
}
