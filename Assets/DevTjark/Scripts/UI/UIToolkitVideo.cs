using UnityEngine;
using UnityEngine.Video;

public class UIToolkitVideo : MonoBehaviour
{
    public VideoPlayer VideoPlayer;
    public RenderTexture RenderTexture;
    
    private void Start()
    {
        VideoPlayer.targetTexture = RenderTexture;
    }
    
    public void PlayVideo() => VideoPlayer.Play();
    
    public void SetVideoClip(VideoClip clip) => VideoPlayer.clip = clip;
}
