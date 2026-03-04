using System;
using ImprovedTimers;
using Player;
using Player.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class MinigolfHole : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: true)]
    [SerializeField] private float waitForLastPlayerTime = 10f;
    
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

    private void OnTriggerEnter(Collider _other)
    {
        if (!_other.CompareTag("Player")) return;
        
        if (_other.TryGetComponent<Controller>(out var controller))
        {
            onMinigolfPlayerFinished?.Invoke(controller.PlayerIndex);
            controller.DisableController();
            
            ++m_finishedPlayers;
            ConsoleProDebug.LogToFilter($"Players finished: {m_finishedPlayers}", "Debug");
            
            if (raceMode && m_finishedPlayers / 2 == k_MaxPlayerCount - 1)
            {
                countdownTimer.Start();
                ConsoleProDebug.LogToFilter("Game End Countdown started!", "Event");
            }
            
            if (classicMode && m_finishedPlayers / 2 == k_MaxPlayerCount)
            {
                onGameEnd.Invoke();
            }
        }
        
        if (_other.TryGetComponent<PlayerControllerMinigolfMayhem>(out var _playerController))
        {
            _playerController.SwitchToUIInputMap();
        }
        
        _other.transform.parent.GetComponentInChildren<PlayerUIMinigolfMayhem>().StopTimer();
            
        // _other.gameObject.SetActive(false);
    }
}
