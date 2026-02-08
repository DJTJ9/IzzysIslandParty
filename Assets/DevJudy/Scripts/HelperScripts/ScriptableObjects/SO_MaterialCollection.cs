using System;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Collections/Material Collection")]
    [Serializable]
    public class SO_MaterialCollection : ScriptableObject
    {
        public Material[] Materials;
    }
}