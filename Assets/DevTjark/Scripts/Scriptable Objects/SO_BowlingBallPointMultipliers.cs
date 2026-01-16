using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Ball Point Multipliers Collection", menuName = "Scriptable Objects/Ball Point Multipliers Collection", order = 1)]
public class SO_BowlingBallPointMultipliers : SerializedScriptableObject
{
    public Dictionary<BallType, float> BallPointMultipliers = new Dictionary<BallType, float>();
}
