using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Scene Collection", menuName = "Scriptable Objects/Scene Collection", order = 1)]
public class SceneCollectionSO : SerializedScriptableObject
{
    public Dictionary<SceneNames, string> Scenes = new Dictionary<SceneNames, string>();
}
