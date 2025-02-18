using UnityEngine;

[System.Serializable]
public abstract class QuickTimeEvent
{
    /// <summary>
    /// Is this QTE in progress?
    /// </summary>
    public abstract bool InProgress();

    /// <summary>
    /// Perform the Quick Time Event. Call this every update.
    /// </summary>
    /// <param name="zPressed"></param>
    /// <param name="xPressed"></param>
    /// <param name="cPressed"></param>
    /// <param name="moveInput"></param>
    /// <param name="parent"></param>
    /// <returns>
    /// Float representing the score of the QTE  
    /// </returns>
    public abstract float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent);
    
    protected abstract void CreateUI(Transform anchor);
    protected abstract void DestroyUI();

    protected abstract void StartQTE(Interactable parent);

    /// <summary>
    /// Use this to cancel the QTE. Interally this is used to clean up once completed.
    /// </summary>
    /// <param name="parent"></param>
    public abstract void EndQTE(Interactable parent);
}
