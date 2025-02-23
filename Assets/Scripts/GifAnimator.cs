using UnityEngine;
using UnityEngine.UI;

public class GifAnimator : MonoBehaviour
{
    public Sprite[] frames;          // Array of frames from the GIF
    public float framesPerSecond = 10f;
    private Image imageComponent;

    void Awake()
    {
        imageComponent = GetComponent<Image>();
    }

    void Update()
    {
        if (frames.Length == 0) return;
        int index = (int)(Time.time * framesPerSecond) % frames.Length;
        imageComponent.sprite = frames[index];
    }
}
