using System;
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
    
    [FoldoutGroup("Easter Egg Positions", expanded: false)]
    [SerializeField] private Transform[] easterEggPositions;
    
    [SerializeField] private SO_PlayerCollectionRacingGames currentPlayersSO;
    public static event Action<int> onMinigolfPlayerFinished;
    [SerializeField] private UnityEvent onGameEnd;
    
    private MinigolfMayhemGameManager minigolfMayhemGameManager;
    private CountdownTimer countdownTimer;
    
    private const int k_MaxPlayerCount = 4;
    private int m_finishedPlayers;
    private bool classicMode;
    private bool raceMode;

    private void Start()
    {
        minigolfMayhemGameManager = FindFirstObjectByType<MinigolfMayhemGameManager>();
        classicMode = minigolfMayhemGameManager.ClassicMode;
        raceMode = minigolfMayhemGameManager.RaceMode;
        countdownTimer = new CountdownTimer(waitForLastPlayerTime);
        countdownTimer.OnTimerStop += () => onGameEnd.Invoke();
    }

    private void OnEnable()
    {
        m_finishedPlayers = 0;
    }

    private void FixedUpdate()
    {
        countdownTimer.Tick(Time.deltaTime);
    }

    /// <summary>
    /// Handles player interactions when they enter the minigolf hole.
    /// Moves players to specific positions, updates player states, applies game mode-specific logic
    /// and triggers events for scoring. Also handles camera adjustments.
    /// </summary>
    /// <param name="_other">The player entering the trigger zone.</param>
    private void OnTriggerEnter(Collider _other)
    {
        if (!_other.CompareTag("Player")) return; 
        
        _other.transform.DOMove(easterEggPositions[m_finishedPlayers / 2].position, 2);
        if (_other.TryGetComponent<Rigidbody>(out var _rb))
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.constraints = RigidbodyConstraints.FreezePosition;
            ++m_finishedPlayers;
        }
        
        if (_other.TryGetComponent<Controller>(out var controller))
        {
            onMinigolfPlayerFinished?.Invoke(controller.PlayerIndex);
            // controller.DisableController();
            
            if (raceMode && m_finishedPlayers / 2 == k_MaxPlayerCount - 1)
            {
                countdownTimer.Start();
            }
            
            if (classicMode && m_finishedPlayers / 2 == k_MaxPlayerCount)
            {
                onGameEnd.Invoke();
            }
        }

        if (_other.TryGetComponent<GoalCameraController>(out var _goalCameraController))
        {
            _goalCameraController.SetGoalCameraValues();
        }
        
        if (_other.TryGetComponent<PlayerControllerMinigolfMayhem>(out var _playerController))
        {
            _playerController.SwitchToUIInputMap();
        }
    }
}
