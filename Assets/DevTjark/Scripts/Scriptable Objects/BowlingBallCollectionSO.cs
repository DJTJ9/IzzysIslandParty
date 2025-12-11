using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Bowling Balls Collection", menuName = "Scriptable Objects/Bowling Balls Collection", order = 1)]
public class BowlingBallCollectionSO : SerializedScriptableObject
{
    public Dictionary<BallType, BowlingBallSO> BowlingBalls = new Dictionary<BallType, BowlingBallSO>();
}