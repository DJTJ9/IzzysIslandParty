using System;
using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SwaggySnapshotsGameManager : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: true)] [SerializeField] private float m_startMoveDuration = 15f;
    [SerializeField] private float m_roundTime = 15f;
    [SerializeField] private float m_danceMoveDuration = 3f;
    [SerializeField] private float m_photoShowDuration = 10f;


    private CountdownTimer m_startMoveTimer;
    private CountdownTimer m_danceMoveSwitchTimer;

    public static float StartMoveDuration = 5f;
    public static float RoundTime = 15f;

    [SerializeField] private SO_SwaggySnapshotsPlayerCollection playersSO;

    [FoldoutGroup("Events", expanded: false)] 
    [SerializeField] private UnityEvent onDanceMoveChanged;
    [SerializeField] private UnityEvent onGameStart;
    [SerializeField] private UnityEvent onRoundStart;
    [SerializeField] private UnityEvent onRoundEnd;
    [SerializeField] private UnityEvent onGameEnd;
    [SerializeField] private UnityEvent onLevelLoaded;
    
    private CountdownTimer m_roundTimer;
    private CountdownTimer m_photoShowTimer;

    private void OnEnable()
    {
        InitializeTimers();
        onLevelLoaded.Invoke();
        FreezeTimeScale();
        

        StartMoveDuration = m_startMoveDuration;
        RoundTime = m_roundTime;

        foreach (var player in playersSO.Players)
        {
            player.PlayerScore.ResetScore();
            player.ResetScoreValues();
        }
    }

    private void OnDisable()
    {
        m_startMoveTimer.Dispose();
        m_danceMoveSwitchTimer.Dispose();
        m_roundTimer.Dispose();
        m_photoShowTimer.Dispose();
    }

    private void Update()
    {
        RoundTime = !m_roundTimer.IsRunning ? m_roundTime : m_roundTimer.CurrentTime;

// #if !UNITY_EDITOR
//         m_startMoveTimer.Tick(Time.deltaTime);
//         m_danceMoveSwitchTimer.Tick(Time.deltaTime);
//         m_roundTimer.Tick(Time.deltaTime);
//         m_photoShowTimer.Tick(Time.deltaTime);
// #endif
    }

    private void FixedUpdate()
    {
        m_startMoveTimer.Tick(Time.deltaTime);
        m_danceMoveSwitchTimer.Tick(Time.deltaTime);
        m_roundTimer.Tick(Time.deltaTime);
        m_photoShowTimer.Tick(Time.deltaTime);
    }

    private void InitializeTimers()
    {
        m_startMoveTimer = new CountdownTimer(m_startMoveDuration);
        m_startMoveTimer.OnTimerStop += () =>
        {
            m_danceMoveSwitchTimer.Start();
            m_roundTimer.Start();
            onRoundStart.Invoke();
        };

        m_danceMoveSwitchTimer = new CountdownTimer(m_danceMoveDuration);
        m_danceMoveSwitchTimer.OnTimerStart += () => onDanceMoveChanged.Invoke();
        m_danceMoveSwitchTimer.OnTimerStop += () =>
        {
            m_danceMoveSwitchTimer.Reset();
            m_danceMoveSwitchTimer.Start();
        };

        m_roundTimer = new CountdownTimer(m_roundTime);
        m_roundTimer.OnTimerStop += OnRoundEnd;

        m_photoShowTimer = new CountdownTimer(m_photoShowDuration);
        m_photoShowTimer.OnTimerStop += () => onGameEnd.Invoke();
    }

    public void StartGame()
    {
        m_startMoveTimer.Start();
    }


    private void OnRoundEnd()
    {
        onRoundEnd.Invoke();
        m_photoShowTimer.Start();
    }

    public void FreezeTimeScale()   => Time.timeScale = 0f;
    public void UnfreezeTimeScale() => Time.timeScale = 1f;
}