using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Dictionaries/SoundDictionary")]
public class SO_SoundDictionary : SerializedScriptableObject
{
    public Dictionary<string, AudioClip> LevelAudios = new Dictionary<string, AudioClip>();
}
