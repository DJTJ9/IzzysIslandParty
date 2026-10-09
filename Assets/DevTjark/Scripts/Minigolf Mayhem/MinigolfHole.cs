using System;
using System.Collections.Generic;
using DG.Tweening;
using ImprovedTimers;
using Player.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class MinigolfHole : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: true)]
    [SerializeField] private float waitForLastPlayerTime = 10f;
    [SerializeField] private float classicWaitForLastPlayerTime = 30f;

    [FoldoutGroup("Easter Egg Positions", expanded: false)]
    [SerializeField] private Transform[] easterEggPositions;
    
    [SerializeField] private SO_PlayerCollectionRacingGames currentPlayersSO;
    public static event Action<int> onMinigolfPlayerFinished;
    [SerializeField] private UnityEvent onGameEnd;
    
    private MinigolfMayhemGameManager minigolfMayhemGameManager;
    private CountdownTimer countdownTimer;
    
    private const int k_MaxPlayerCount = 4;
    private static readonly HashSet<int> s_finishedPlayers = new();
    private bool m_gameEnded;
    private bool classicMode;
    private bool raceMode;

    private void Start()
    {
        minigolfMayhemGameManager = FindFirstObjectByType<MinigolfMayhemGameManager>();
        classicMode = minigolfMayhemGameManager.ClassicMode;
        raceMode = minigolfMayhemGameManager.RaceMode;
        countdownTimer = new CountdownTimer(classicMode ? classicWaitForLastPlayerTime : waitForLastPlayerTime, autoTick: true);
        countdownTimer.OnTimerStop += EndGame;
    }

    private void OnEnable()
    {
        s_finishedPlayers.Clear();
        m_gameEnded = false;
    }

    private void OnDestroy()
    {
        countdownTimer?.Dispose();
    }

    /// <summary>
    /// Returns whether the player with the given index has holed out in the current game.
    /// </summary>
    public static bool HasFinished(int _playerIndex) => s_finishedPlayers.Contains(_playerIndex);

    /// <summary>
    /// Handles players entering the minigolf hole. Each player counts once (identified by PlayerIndex),
    /// gets moved to the next easter egg slot and frozen. Classic starts the end countdown at the first finisher,
    /// race at the third. The game ends immediately once all players are in.
    /// </summary>
    /// <param name="_other">The player entering the trigger zone.</param>
    private void OnTriggerEnter(Collider _other)
    {
        if (!_other.CompareTag("Player")) return;
        if (!_other.TryGetComponent<Controller>(out var controller)) return;
        if (!s_finishedPlayers.Add(controller.PlayerIndex)) return;

        var finishedCount = s_finishedPlayers.Count;

        _other.transform.DOMove(easterEggPositions[finishedCount - 1].position, 2);
        if (_other.TryGetComponent<Rigidbody>(out var _rb))
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints.FreezePosition;
        }

        onMinigolfPlayerFinished?.Invoke(controller.PlayerIndex);

        if (finishedCount >= k_MaxPlayerCount)
            EndGame();
        else if (classicMode && finishedCount == 1)
            countdownTimer.Start();
        else if (raceMode && finishedCount == k_MaxPlayerCount - 1)
            countdownTimer.Start();

        if (_other.TryGetComponent<GoalCameraController>(out var _goalCameraController))
        {
            _goalCameraController.SetGoalCameraValues();
        }

        if (_other.TryGetComponent<PlayerControllerMinigolfMayhem>(out var _playerController))
        {
            _playerController.SwitchToUIInputMap();
        }
    }

    /// <summary>
    /// Ends the game exactly once, either when the countdown runs out or when all players finished.
    /// </summary>
    private void EndGame()
    {
        if (m_gameEnded) return;
        m_gameEnded = true;

        countdownTimer.Stop();
        onGameEnd.Invoke();
    }
}
