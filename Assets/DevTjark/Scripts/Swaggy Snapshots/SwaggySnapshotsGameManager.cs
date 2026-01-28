using System;
using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class SwaggySnapshotsGameManager : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: true)]
    [SerializeField] private float startMoveDuration = 15f;
    [SerializeField] private float roundTime = 15f;
    [SerializeField] private float danceMoveDuration = 3f;
    [SerializeField] private float photoShowDuration = 10f;
    
    [SerializeField]
    private UnityEvent onDanceMoveChanged;
    
    private CountdownTimer m_startMoveTimer;
    private CountdownTimer m_danceMoveSwitchTimer;
    
    public static float StartMoveDuration = 15f;
    public static float RoundTime = 15f;
    
    [SerializeField] private UnityEvent onRoundEnd;
    [SerializeField] private UnityEvent onGameEnd;
    
    private CountdownTimer m_roundTimer;
    private CountdownTimer m_photoShowTimer;

    private void Awake()
    {
        StartMoveDuration = startMoveDuration;
        RoundTime = roundTime;
        
        m_startMoveTimer = new CountdownTimer(StartMoveDuration);
        m_startMoveTimer.OnTimerStop += () =>
        {
            m_danceMoveSwitchTimer.Start();
            m_roundTimer.Start();
        };
        
        m_danceMoveSwitchTimer = new CountdownTimer(danceMoveDuration);
        m_danceMoveSwitchTimer.OnTimerStart += () => onDanceMoveChanged.Invoke();
        m_danceMoveSwitchTimer.OnTimerStop += () => 
        {
            m_danceMoveSwitchTimer.Reset();
            m_danceMoveSwitchTimer.Start();
        };
        
        m_roundTimer = new CountdownTimer(RoundTime);
        m_roundTimer.OnTimerStop += OnRoundEnd;
        
        m_photoShowTimer = new CountdownTimer(photoShowDuration);
        m_photoShowTimer.OnTimerStop += () => onGameEnd.Invoke();
    }

    private void Start()
    {
        m_startMoveTimer.Start();
    }
    
    
    private void OnRoundEnd()
    {
        onRoundEnd.Invoke();
        m_photoShowTimer.Start();
    }
}
