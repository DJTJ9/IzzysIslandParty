using UnityEngine;
using UnityEngine.Video;

public class UIToolkitVideo : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public RenderTexture RenderTexture;
    
    /// <summary>
    /// Initializes the video player by assigning the specified render texture as its target output.
    /// </summary>
    private void Start()
    {
        VideoPlayer.targetTexture = RenderTexture;
    }
    
    /// <summary>
    /// Starts playing the video assigned to the video player.
    /// </summary>
    public void PlayVideo() => VideoPlayer.Play();
    
    /// <summary>
    /// Sets a new video clip for the video player to play.
    /// </summary>
    /// <param name="clip">The video clip to set for playback.</param>
    public void SetVideoClip(VideoClip clip) => VideoPlayer.clip = clip;
    
    /// <summary>
    /// Stops the video currently playing on the video player.
    /// </summary>
    public void StopVideo() => VideoPlayer.Stop();
}
