using System;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Collections/Mesh Collection")]
    [Serializable]
    public class SO_MeshCollection : ScriptableObject
    {
       public Mesh[] Meshes;
    }
}
