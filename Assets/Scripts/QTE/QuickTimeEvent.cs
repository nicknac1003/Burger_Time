using UnityEngine;
using System.Collections.Generic;
[System.Serializable]
public abstract class QuickTimeEvent
{
    [Tooltip("Should the player be locked in place during the QTE?")]
    [SerializeField] public List<AudioClip> progressSounds;
    [SerializeField] public List<AudioClip> errorSounds;
    [SerializeField] public AudioSource audioSource;
    [SerializeField] bool locksPlayerInPlace = true;
    [Range(0f, 1f)]
    [SerializeField]
    private float progressVolume = 0.75f;
    [Range(0f, 1f)]
    [SerializeField]
    private float errorVolume = 0.45f;

    public QuickTimeEvent(bool locksPlayer = true)
    {
        locksPlayerInPlace = locksPlayer;
    }

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
    public float QTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent)
    {
        float score = PerformQTE(zPressed, xPressed, cPressed, moveInput, parent);

        if (locksPlayerInPlace)
        {
            if (InProgress())
            {
                PlayerController.LockPlayer();
            }
            else
            {
                //Debug.Log($"QTE performed by: {parent.gameObject.name}");
                //PlayerController.UnlockPlayer();
            }
        }

        return score;
    }
    public void PlayProgressSound()
    {
        audioSource.Stop();
        audioSource.PlayOneShot(progressSounds[Random.Range(0, progressSounds.Count)], progressVolume);
    }
    public void PlayErrorSound()
    {
        audioSource.PlayOneShot(errorSounds[Random.Range(0, errorSounds.Count)], errorVolume);
    }

    protected abstract float PerformQTE(bool zPressed, bool xPressed, bool cPressed, Vector2 moveInput, Interactable parent);

    protected abstract void CreateUI(Transform anchor);
    protected abstract void DestroyUI();

    protected abstract void StartQTE(Interactable parent);

    /// <summary>
    /// Use this to cancel the QTE. Interally this is used to clean up once completed.
    /// </summary>
    /// <param name="parent"></param>
    public abstract void EndQTE(Interactable parent);
}
