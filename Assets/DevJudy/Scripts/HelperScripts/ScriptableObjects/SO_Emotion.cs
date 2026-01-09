using enums;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Emotion")]
    public class SO_Emotion : ScriptableObject
    {
        public EEmotion Emotion;
        public Material IconMaterial;
    }
}
