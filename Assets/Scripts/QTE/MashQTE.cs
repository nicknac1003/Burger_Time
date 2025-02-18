using UnityEngine;

public class MashQTE : QuickTimeEvent
{
    [Tooltip("The number of times the player must press the button to succeed.")]
    [Range(3, 20)][SerializeField] private int mashCount = 10;
    
    public override void CreateUI(Transform parent)
    {
        throw new System.NotImplementedException();
    }

    public override void DestroyUI()
    {
        throw new System.NotImplementedException();
    }

    public override bool InProgress()
    {
        throw new System.NotImplementedException();
    }

    public override float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent)
    {
        throw new System.NotImplementedException();
    }
}