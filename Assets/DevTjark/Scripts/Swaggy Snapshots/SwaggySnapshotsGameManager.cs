using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class SwaggySnapshotsGameManager : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: true)] 
    [SerializeField] private float m_startMoveDuration = 15f;
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

    /// <summary>
    /// Initializes timers, resets time scale and invokes the level loaded event.
    /// Resets player scores and prepares them for the game.
    /// </summary>
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

    /// <summary>
    /// Disposes of all active timers to ensure proper cleanup 
    /// when the game manager is disabled.
    /// </summary>
    private void OnDisable()
    {
        m_startMoveTimer.Dispose();
        m_danceMoveSwitchTimer.Dispose();
        m_roundTimer.Dispose();
        m_photoShowTimer.Dispose();
    }
    
    /// <summary>
    /// Advances all countdown timers by the fixed time delta. 
    /// Handles logic for start moves, dance switches and photo show countdowns.
    /// </summary>
    private void FixedUpdate()
    {
        UpdateRoundTime();
        // m_startMoveTimer.Tick(Time.deltaTime);
        // m_danceMoveSwitchTimer.Tick(Time.deltaTime);
        // m_roundTimer.Tick(Time.deltaTime);
        // m_photoShowTimer.Tick(Time.deltaTime);
    }

    /// <summary>
    /// Starts and initializes all required timers for the game,
    /// including preparation for dance moves, round progression and photo display events.
    /// </summary>
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
    
    // <summary>
    /// Updates the remaining round time dynamically when the game is active. 
    /// Ensures accurate tracking of the countdown timer for the current round.
    /// </summary>
    private void UpdateRoundTime()
    {
        RoundTime = !m_roundTimer.IsRunning ? m_roundTime : m_roundTimer.CurrentTime;
    }

    /// <summary>
    /// Starts the game by initiating the start move timer.
    /// Prepares the game's first phase before transitioning to the main rounds.
    /// </summary>

    public void StartGame()
    {
        m_startMoveTimer.Start();
    }
    
    /// <summary>
    /// Handles logic for ending a round, invokes the round end event
    /// and transitions to the photo show phase.
    /// </summary>
    private void OnRoundEnd()
    {
        onRoundEnd.Invoke();
        m_photoShowTimer.Start();
    }

    /// <summary>
    /// Freezes the game's time scale, effectively pausing the game.
    /// </summary>
    public void FreezeTimeScale()   => Time.timeScale = 0f;
    
    /// <summary>
    /// Resumes the game by unfreezing the time scale and allowing time to progress normally.
    /// </summary>
    public void UnfreezeTimeScale() => Time.timeScale = 1f;
}