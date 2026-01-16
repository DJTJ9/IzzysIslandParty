using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    [SerializeField] private float isFallenDotProductThreshold = 0.7f;
    [SerializeField] private BowlingBallSwapper ballSwapper;
    [SerializeField] private GameScoreSO scoreSO;
    [SerializeField] private SO_BowlingBallPointMultipliers pointMultiplierSO;

    public void CheckScore() 
    {
        var dotProduct = Vector3.Dot(transform.up, Vector3.up);
        var isFallen = dotProduct < isFallenDotProductThreshold;
        
        if (isFallen)
        {
            scoreSO.Value += pointMultiplierSO.BallPointMultipliers[ballSwapper.GetCurrentBallType()];
        }
    }
}