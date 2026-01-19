using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ScoreCalculator : MonoBehaviour
{
    [SerializeField] private float isFallenDotProductThreshold = 0.7f;
    [SerializeField] private GameScoreSO scoreSO;
    [SerializeField] private BallTypeChecker ballTypeChecker;
    [SerializeField] private SO_BowlingBallPointMultipliers pointMultiplierSO;

    private void OnEnable()
    {
        scoreSO.Value = 0f;
    }

    public void CheckScore() 
    {
        var dotProduct = Vector3.Dot(transform.up, Vector3.up);
        var isFallen = dotProduct < isFallenDotProductThreshold;
        
        if (isFallen)
        {
            scoreSO.Value += pointMultiplierSO.BallPointMultipliers[ballTypeChecker.GetCurrentBallType()];
        }
    }
    
    public void ResetScore() => scoreSO.Value = 0f;
}