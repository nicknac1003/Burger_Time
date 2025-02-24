using UnityEngine;
using UnityEngine.Video;
using System.IO;

public class LocalVideoPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string videoFileName = "video.mp4"; // Your video file in StreamingAssets

    void Start()
    {
        // Combine the StreamingAssets path with your video file name.
        string filePath = Path.Combine(Application.streamingAssetsPath, videoFileName);
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = filePath;

        // Prepare and play the video once it's ready.
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += (vp) =>
        {
            vp.Play();
        };
    }
}
