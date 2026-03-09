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
        ResetScore();
    }

    /// <summary>
    /// Checks the score for the current object by calculating if it has fallen.
    /// If fallen, adds points based on the current ball type and its multiplier.
    /// </summary>
    public void CheckScore() 
    {
        var dotProduct = Vector3.Dot(transform.up, Vector3.up);
        var isFallen = dotProduct < isFallenDotProductThreshold;
        
        if (isFallen)
        {
            scoreSO.Value += pointMultiplierSO.BallPointMultipliers[ballTypeChecker.GetCurrentBallType()];
        }
    }
    
    /// <summary>
    /// Resets the game score to zero, typically called to restart or reset the game state.
    /// </summary>
    public void ResetScore() => scoreSO.Value = 0f;
}