using System;
using Player;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    public int PlayerIndex;
    
    [FoldoutGroup("Unity Events", expanded: false)]
    [SerializeField] private UnityEvent onPause;
    [FoldoutGroup("Unity Events")]
    [SerializeField] private UnityEvent onUnpause;
    [FoldoutGroup("Unity Events")]
    [SerializeField] private UnityEvent onGameStart;

    protected bool m_isActive = true;
    protected bool m_hasJoinedGame;
    protected bool m_noGoingBack;
    private bool pauseInputEnabled;

    protected PlayerInput playerInput;
    
    private void Start()
    {
        m_hasJoinedGame = true;
    }
    
    private void OnEnable()
    {
        EnableController();
    }

    private void OnDisable()
    {
        DisableController();
    }

    public int GetPlayerIndex() => PlayerIndex;
    
    public void SetPlayerIndex(int _playerIndex) => PlayerIndex = _playerIndex;

    public virtual void OnNPCJoined(SO_PlayerRacingGames _player) { }
    
    /// <summary>
    /// Pauses the game upon detecting a valid pause input action and invokes the pause event.
    /// </summary>
    /// <param name="_context">The context of the pause input action.</param>
    public void OnPause(InputAction.CallbackContext _context)
    {
        if (!pauseInputEnabled) return;
        if (!_context.started) return;
        
        onPause.Invoke();
    }
    
    /// <summary>
    /// Unpauses the game upon detecting a valid unpause input action and invokes the unpause event.
    /// </summary>
    /// <param name="_context">The context of the unpause input action.</param>
    public void OnUnpause(InputAction.CallbackContext _context)
    {
        if (!pauseInputEnabled) return;
        if (!_context.started) return;
        
        onUnpause.Invoke();
    }
    
    /// <summary>
    /// Invokes the game start event when the start game input is triggered.
    /// </summary>
    /// <param name="_context">The context of the start game action.</param>
    public void OnStartGame(InputAction.CallbackContext _context)
    {
        if (!m_hasJoinedGame) return;
        if (!_context.started) return;
        
        onGameStart.Invoke();
    }
    
    public void OnControllerSelectionBack(InputAction.CallbackContext _context)
    {
        if (!_context.started || m_noGoingBack) return;
        
        AsyncLevelLoader.Instance.LoadScene(SceneNames.MainMenu);
    }
    
    public void SwitchToUIInputMap()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

    public virtual void SwitchToPlayerInputMap()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }
    
    public void EnableController() => m_isActive = true;
    public void DisableController() => m_isActive = false;
    public void EnablePauseInput() => pauseInputEnabled = true;
    public void DisablePauseInput() => pauseInputEnabled = false;
    
    public void NoGoingBack() => m_noGoingBack = true;
    
    public void CanGoBack() => m_noGoingBack = false;
}
