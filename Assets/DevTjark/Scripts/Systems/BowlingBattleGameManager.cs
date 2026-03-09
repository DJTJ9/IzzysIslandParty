using System;
using UnityEngine;
using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BowlingBattleGameManager : MonoBehaviour
{
    [SerializeField] private UnityEvent onGameStart;
    [SerializeField] private UnityEvent onPreparationPhaseStart;
    [SerializeField] private UnityEvent onReleaseBall;
    [SerializeField] private UnityEvent onRoundEnd;
    [SerializeField] private UnityEvent onGameEnd;
    [SerializeField] private UnityEvent onLevelLoaded;

    [FoldoutGroup("Round Settings", expanded: true)] [SerializeField] private float preparationPhaseDuration = 10f;
    [SerializeField] private float roundDuration = 15f;
    [SerializeField] private int maxRounds = 3;

    [HideInInspector] public static float PreparationPhaseTimer;
    [HideInInspector] public static float RoundTimer;

    private int m_roundIndex = 1;

    private CountdownTimer m_joinPhaseTimer;
    private CountdownTimer m_preparationPhaseTimer;
    private CountdownTimer m_roundTimer;


    private void Start()
    {
        ResetRoundIndex();

        InstantiateCountdownTimers();
        SubscribeToCountdownTimersActions();
        onLevelLoaded.Invoke();
    }

    private void OnEnable()
    {
        FreezeTimeScale();
        onLevelLoaded.Invoke();
    }

    private void OnDisable()
    {
        UnsubscribeFromCountdownTimersActions();
    }

    /// <summary>
    /// Updates the preparation and round timers, ensuring they reflect the timers' current states.
    /// </summary>
    private void Update()
    {
        PreparationPhaseTimer = m_preparationPhaseTimer.IsRunning ? m_preparationPhaseTimer.CurrentTime : preparationPhaseDuration;
        RoundTimer = m_roundTimer.IsRunning ? m_roundTimer.CurrentTime : roundDuration;
    }

    /// <summary>
    /// Processes countdown timer ticks in fixed intervals to ensure accurate updates for join phase, preparation phase, and round timers.
    /// </summary>
    private void FixedUpdate()
    {
        m_joinPhaseTimer.Tick(Time.deltaTime);
        m_preparationPhaseTimer.Tick(Time.deltaTime);
        m_roundTimer.Tick(Time.deltaTime);
    }

    /// <summary>
    /// Invokes the game start event to notify that the game has begun.
    /// </summary>
    public void StartGame()
    {
        onGameStart.Invoke();
    }

    /// <summary>
    /// Starts the preparation phase by invoking the relevant event and beginning the preparation timer.
    /// </summary>
    public void StartPreparationPhase()
    {
        onPreparationPhaseStart.Invoke();
        m_preparationPhaseTimer.Start();
    }
    
    /// <summary>
    /// Initializes countdown timers for the preparation and round phases with their respective durations.
    /// </summary>
    private void InstantiateCountdownTimers()
    {
        m_preparationPhaseTimer = new CountdownTimer(preparationPhaseDuration);
        m_roundTimer = new CountdownTimer(roundDuration);
    }

    /// <summary>
    /// Subscribes actions to the countdown timers to trigger ball release and round end events when the timers stop.
    /// </summary>
    private void SubscribeToCountdownTimersActions()
    {
        m_preparationPhaseTimer.OnTimerStop += ReleaseBall;
        m_roundTimer.OnTimerStop += EndRound;
    }

    /// <summary>
    /// Unsubscribes actions from the countdown timers, stopping them from triggering additional events.
    /// </summary>
    private void UnsubscribeFromCountdownTimersActions()
    {
        m_preparationPhaseTimer.OnTimerStop -= ReleaseBall;
        m_roundTimer.OnTimerStop -= EndRound;
    }

    /// <summary>
    /// Ends the current round, handles round logic, and moves to the next round if applicable.
    /// If it's the final round, it triggers game-end logic.
    /// </summary>
    private void EndRound()
    {
        HandleRoundEnd();

        ++m_roundIndex;
    }

    /// <summary>
    /// Invokes the ball release event and starts the round timer.
    /// Called when the preparation timer ends.
    /// </summary>
    private void ReleaseBall()
    {
        onReleaseBall.Invoke();
        m_roundTimer.Start();
    }

    /// <summary>
    /// Handles the logic for ending a round. Restarts the preparation phase if there are remaining rounds 
    /// or ends the game if the maximum number of rounds is reached.
    /// </summary>
    private void HandleRoundEnd()
    {
        if (m_roundIndex == maxRounds)
        {
            onRoundEnd.Invoke();
            onGameEnd.Invoke();
            return;
        }

        onRoundEnd.Invoke();
        StartPreparationPhase();
    }

    /// <summary>
    /// Resets the round index to 1, typically at the start of a new game.
    /// </summary>
    private void ResetRoundIndex() => m_roundIndex = 1;

    /// <summary>
    /// Freezes the game's time scale.
    /// </summary>
    public void FreezeTimeScale() => Time.timeScale = 0f;

    /// <summary>
    /// Unfreezes the game's time scale.
    /// </summary>
    public void UnfreezeTimeScale() => Time.timeScale = 1f;
}