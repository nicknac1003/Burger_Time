using UnityEngine;

public class Jukebox : Interactable
{
    public static Jukebox Instance { get; private set; }

    // Reference to the AudioSource component
    public AudioSource audioSource;

    // Serialized list of AudioClips
    public AudioClip[] clips;
    public float volume = 0.5f;

    // Keeps track of the current clip index
    private int currentClipIndex = 0;

    private bool isOn = true;
    public bool IsOn() => isOn;
    public void SetOn(bool on) => isOn = on;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

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
        if (audioSource.clip != null && !audioSource.isPlaying && isOn)
        {
            NextClip();
        }
    }

    protected override void OnZ()
    {
        // Play the current clip
        ToggleMusic();
    }

    public static void ToggleMusic()
    {
        // Toggle the music on/off
        if (Instance.audioSource.isPlaying)
        {
            Instance.audioSource.Pause();
        }
        else
        {
            Instance.audioSource.Play();
        }
    }

    // This method can be hooked up to a "Next Clip" button
    private void NextClip()
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
    public void SetVolume(float volume)
    {
        this.volume = volume;
        audioSource.volume = volume;
    }
}