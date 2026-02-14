using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Game Preview Clips", menuName = "Scriptable Objects/General/Game Preview Clips", order = 1)]
public class SO_GamePreviewClips : SerializedScriptableObject
{
    public Dictionary<GamePreviewClips, VideoClip> PreviewClips = new Dictionary<GamePreviewClips, VideoClip>();

}
