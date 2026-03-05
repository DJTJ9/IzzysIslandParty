using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Swaggy Snapshots Player Collection", menuName = "Scriptable Objects/Swaggy Snapshots/Current Player Collection", order = 1)]
public class SO_SwaggySnapshotsPlayerCollection : SerializedScriptableObject
{
    public List<SO_PlayerSwaggySnapshots> Players = new();
}
