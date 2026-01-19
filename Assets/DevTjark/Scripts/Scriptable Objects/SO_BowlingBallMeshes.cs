using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Ball Meshes Collection", menuName = "Scriptable Objects/Ball Meshes Collection", order = 1)]
public class SO_BowlingBallMeshes : SerializedScriptableObject
{
    public Dictionary<BallType, Mesh> BallMeshes = new Dictionary<BallType, Mesh>();
}
