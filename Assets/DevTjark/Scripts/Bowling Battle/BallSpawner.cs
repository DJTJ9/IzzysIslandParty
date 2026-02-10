using System;
using DependencyInjection;
using UnityEngine;
using UnityEngine.Serialization;

public class BallSpawner : MonoBehaviour, IDependencyProvider
{
    public GameObject    CurrentBallInstance { get; private set; }
    public SO_PlayerBowlingBattle CurrentBattle       { get; private set; }
    
    [SerializeField] private BowlingBallCollectionSO ballCollectionSO;

    [Provide] BallSpawner ProvideBallSpawner() => this;
    
    // private void Awake()
    // {
    //     CreateAndSetFirstBallInstance();
    // }

    // private void CreateAndSetFirstBallInstance()
    // {
    //     CurrentBallInstance = Instantiate(ballCollectionSO.BowlingBalls[BallType.Basketball].PlayerPrefab,
    //         transform.position, transform.rotation);
    //
    //     CurrentBattle = ballCollectionSO.BowlingBalls[BallType.Basketball];
    // }

    public void SpawnBall(SO_PlayerBowlingBattle _battle)
    {
        if (CurrentBallInstance != null)
            Destroy(CurrentBallInstance);
        
        CurrentBallInstance = Instantiate(_battle.PlayerReference, transform.position, transform.rotation);
        CurrentBattle = _battle;
    }
    
    public void RespawnBall() => SpawnBall(CurrentBattle);
}