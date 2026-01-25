using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Ball Weights Collection", menuName = "Scriptable Objects/Ball Weights Collection", order = 1)]
public class SO_BowlingBallWeights : SerializedScriptableObject
{
    public Dictionary<BallType, float> BallWeights = new Dictionary<BallType, float>();
}
