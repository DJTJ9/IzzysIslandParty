using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Ball Materials Collection", menuName = "Scriptable Objects/Ball Materials Collection", order = 1)]
public class SO_BowlingBallMaterials : SerializedScriptableObject
{
    public Dictionary<BallType, Material> BallMaterials = new Dictionary<BallType, Material>();
}