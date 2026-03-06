using System.Collections.Generic;
using enums;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SerializedObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Dictionaries/SpriteDictionary")]
    public class SO_SpriteDictionary : SerializedScriptableObject
    {
        public Dictionary<EControlScheme, Sprite> Dictionary = new Dictionary<EControlScheme, Sprite>();
    }
}
