using System;
using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class SwaggySnapshotsGameManager : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: true)]
    [SerializeField] private float startMoveDuration = 5f;
    [SerializeField] private float danceMoveDuration = 3f;
    
    [SerializeField]
    private UnityEvent onDanceMoveChanged;
    
    private CountdownTimer m_startMoveTimer;
    private CountdownTimer m_danceMoveSwitchTimer;

    private void Awake()
    {
        m_startMoveTimer = new CountdownTimer(startMoveDuration);
        m_startMoveTimer.OnTimerStop += () => m_danceMoveSwitchTimer.Start();
        
        m_danceMoveSwitchTimer = new CountdownTimer(danceMoveDuration);
        m_danceMoveSwitchTimer.OnTimerStart += () => onDanceMoveChanged.Invoke();
        m_danceMoveSwitchTimer.OnTimerStop += () => 
        {
            m_danceMoveSwitchTimer.Reset();
            m_danceMoveSwitchTimer.Start();
        };
    }

    private void Start()
    {
        m_startMoveTimer.Start();
    }
}
