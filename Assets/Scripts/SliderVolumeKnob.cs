using UnityEngine;
using TMPro;

public class SliderTextUpdater : MonoBehaviour
{
    public TMP_Text volumeText;

    // This function will be called by the slider's On Value Changed event
    public void UpdateVolumeText(float sliderValue)
    {
        int percentage = Mathf.RoundToInt(sliderValue * 100);
        volumeText.text = "Music Volume: " + percentage;
    }
}
