using System;
using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class SwaggySnapshotsGameManager : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: true)]
    public static float StartMoveDuration = 5f;
    [SerializeField] private float danceMoveDuration = 3f;
    
    [SerializeField]
    private UnityEvent onDanceMoveChanged;
    
    private CountdownTimer m_startMoveTimer;
    private CountdownTimer m_danceMoveSwitchTimer;
    
    public static float RoundTime = 15;
    
    [SerializeField] private UnityEvent onRoundEnd;
    
    private CountdownTimer roundTimer;
    
    private void OnRoundEnd()
    {
        onRoundEnd.Invoke();
    }

    private void Awake()
    {
        m_startMoveTimer = new CountdownTimer(StartMoveDuration);
        m_startMoveTimer.OnTimerStop += () =>
        {
            m_danceMoveSwitchTimer.Start();
            roundTimer.Start();
        };
        
        m_danceMoveSwitchTimer = new CountdownTimer(danceMoveDuration);
        m_danceMoveSwitchTimer.OnTimerStart += () => onDanceMoveChanged.Invoke();
        m_danceMoveSwitchTimer.OnTimerStop += () => 
        {
            m_danceMoveSwitchTimer.Reset();
            m_danceMoveSwitchTimer.Start();
        };
        
        roundTimer = new CountdownTimer(RoundTime);
        roundTimer.OnTimerStop += OnRoundEnd;
    }

    private void Start()
    {
        m_startMoveTimer.Start();
    }
}
