using UnityEngine;

public class Jukebox : Interactable
{
    // Reference to the AudioSource component
    public AudioSource audioSource;

    // Serialized list of AudioClips
    public AudioClip[] clips;
    public float volume = 0.5f;

    // Keeps track of the current clip index
    private int currentClipIndex = 0;


    void Start()
    {
        if (clips.Length > 0)
        {
            // Optionally, start playing the first clip immediately
            audioSource.clip = clips[currentClipIndex];
            audioSource.Play();
        }
        audioSource.volume = volume;
    }
    void Update()
    {
        // Check if a clip is assigned and it's no longer playing
        if (audioSource.clip != null && !audioSource.isPlaying)
        {
            NextClip();
        }
    }

    protected override void OnZ()
    {
        Debug.Log("Jukebox was interacted with");
        // Play the current clip
        ToggleMusic();
    }

    void ToggleMusic()
    {
        // Toggle the music on/off
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }
    }

    // This method can be hooked up to a "Next Clip" button
    public void NextClip()
    {
        if (clips.Length == 0)
            return;

        // Stop the current clip if it's playing
        audioSource.Stop();

        // Move to the next clip, wrapping around if needed
        currentClipIndex = (currentClipIndex + 1) % clips.Length;

        // Set and play the next clip
        audioSource.clip = clips[currentClipIndex];
        audioSource.Play();
    }
}