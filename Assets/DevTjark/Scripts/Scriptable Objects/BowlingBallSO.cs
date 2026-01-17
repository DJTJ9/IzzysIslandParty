using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Bowling Ball", menuName = "Scriptable Objects/Bowling Ball", order = 1)]
public class BowlingBallSO : SerializedScriptableObject
{
    public GameObject PlayerPrefab;
    public BallType CurrentBallType;
    public GameScoreSO PlayerScore;
}