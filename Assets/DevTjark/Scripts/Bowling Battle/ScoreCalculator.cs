using DependencyInjection;
using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    [SerializeField] private float isFallenDotProductThreshold = 0.7f;
    [SerializeField] private GameScoreSO scoreSO;
    
    [Inject] private BallSpawner ballSpawner;

    public void ScoreChecker() 
    {
        var dotProduct = Vector3.Dot(transform.up, Vector3.up);
        var isFallen = dotProduct < isFallenDotProductThreshold;
        
        if (isFallen)
        {
            scoreSO.Value += ballSpawner.CurrentBallSO.pointMultiplier;
        }
    }
}