using UnityEngine;

[System.Serializable]
public abstract class QuickTimeEvent
{
    public abstract float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput);
    public abstract bool  InProgress();
}
