using UnityEngine;

[CreateAssetMenu(fileName = "Bowling Ball", menuName = "Scriptable Objects/Bowling Ball", order = 1)]
public class BowlingBallSO : ScriptableObject
{
    public GameObject ball;
    public float weight;
    public float pointMultiplier;
}