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

    [FormerlySerializedAs("m_preparationPhaseDuration")]
    [FoldoutGroup("Round Settings", expanded: true)]
    [SerializeField] private float preparationPhaseDuration = 10f;
    [SerializeField] private float roundDuration = 15f;
    [SerializeField] private int maxRounds = 3;

    [HideInInspector] public static float PreparationPhaseTimer;
    
    private int m_roundIndex = 1;

    private CountdownTimer m_preparationPhaseTimer;
    private CountdownTimer m_roundTimer;


private void Start()
    {
        
        ResetRoundIndex();
        
        InstantiateCountdownTimers();
        SubscribeToCountdownTimersActions();
        
        onGameStart.Invoke();
    }

    private void OnDisable()
    {
        UnsubscribeFromCountdownTimersActions();
    }

    private void Update()
    {
        m_preparationPhaseTimer.Tick(Time.deltaTime);
        m_roundTimer.Tick(Time.deltaTime);

        PreparationPhaseTimer = m_preparationPhaseTimer.CurrentTime;
    }

    public void StartPreparationPhase()
    {
        onPreparationPhaseStart.Invoke();
        m_preparationPhaseTimer.Reset();
        m_preparationPhaseTimer.Start();
    }

    private void InstantiateCountdownTimers()
    {
        m_preparationPhaseTimer = new CountdownTimer(preparationPhaseDuration);
        m_roundTimer = new CountdownTimer(roundDuration);
    }

    private void SubscribeToCountdownTimersActions()
    {
        m_preparationPhaseTimer.OnTimerStop += ReleaseBall;
        
        m_roundTimer.OnTimerStop += EndRound;
    }

    private void UnsubscribeFromCountdownTimersActions()
    {
        m_preparationPhaseTimer.OnTimerStop -= ReleaseBall;
        
        m_roundTimer.OnTimerStop -= EndRound;
    }

    private void EndRound()
    {
        HandleRoundEnd();
        
        ++m_roundIndex;
    } 

    private void ReleaseBall()
    {
        onReleaseBall.Invoke();
        m_roundTimer.Reset();
        m_roundTimer.Start();
    }

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
    
    private void ResetRoundIndex() => m_roundIndex = 1;
}
