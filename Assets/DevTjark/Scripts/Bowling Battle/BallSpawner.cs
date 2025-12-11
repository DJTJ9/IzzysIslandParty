using System;
using DependencyInjection;
using UnityEngine;
using UnityEngine.Serialization;

public class BallSpawner : MonoBehaviour, IDependencyProvider
{
    public GameObject    CurrentBallInstance { get; private set; }
    public BowlingBallSO CurrentBallSO       { get; private set; }
    
    [SerializeField] private BowlingBallCollectionSO ballCollectionSO;

    [Provide] BallSpawner ProvideBallSpawner() => this;
    
    private void Awake()
    {
        CreateAndSetFirstBallInstance();
    }

    private void CreateAndSetFirstBallInstance()
    {
        CurrentBallInstance = Instantiate(ballCollectionSO.BowlingBalls[BallType.Basketball].ball,
            transform.position, transform.rotation);

        CurrentBallSO = ballCollectionSO.BowlingBalls[BallType.Basketball];
    }

    public void SpawnBall(BowlingBallSO _ballSO)
    {
        if (CurrentBallInstance != null)
            Destroy(CurrentBallInstance);
        
        CurrentBallInstance = Instantiate(_ballSO.ball, transform.position, transform.rotation);
        CurrentBallSO = _ballSO;
    }
    
    public void RespawnBall() => SpawnBall(CurrentBallSO);
}