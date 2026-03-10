using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Dance Moves", menuName = "Scriptable Objects/Swaggy Snapshots/Dance Moves", order = 1)]
public class DanceMovesSO : SerializedScriptableObject
{
    public List<AnimationClip> allDanceMoves = new List<AnimationClip>();
    public List<AnimationClip> coolDanceMoves = new List<AnimationClip>();
    public List<AnimationClip> cringeDanceMoves = new List<AnimationClip>();
    
    public bool IsCoolDanceMove(AnimationClip _clip) => coolDanceMoves.Contains(_clip);
    public bool IsCringeDanceMove(AnimationClip _clip) => cringeDanceMoves.Contains(_clip);
}
