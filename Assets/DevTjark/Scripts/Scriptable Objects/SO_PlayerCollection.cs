using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Bowling Battle Player Collection", menuName = "Scriptable Objects/Bowling Battle/Player Collection", order = 1)]
public class SO_PlayerCollection : SerializedScriptableObject
{
    public List<SO_Player> Players = new();
}
