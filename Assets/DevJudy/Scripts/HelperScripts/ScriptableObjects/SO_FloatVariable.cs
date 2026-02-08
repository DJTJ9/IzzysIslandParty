using System;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Float Variable")]
    [Serializable]
    public class SO_FloatVariable : ScriptableObject
    {
        public float Value;
    }
}