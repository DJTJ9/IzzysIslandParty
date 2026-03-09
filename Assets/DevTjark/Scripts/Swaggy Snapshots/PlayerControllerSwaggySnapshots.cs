using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControllerSwaggySnapshots : Controller
{
    // public int PlayerIndex;

    private InputAction m_takePhotoInputAction;
    private InputAction m_pauseInputAction;
    private InputAction m_unpauseInputAction;
    
    // private PlayerInput playerInput;
    private PhotoCapture photoCapture;

    public static event Action<int> onTakePhoto;
    [SerializeField] private UnityEvent onPause;
    [SerializeField] private UnityEvent onUnpause;
    [SerializeField] private UnityEvent onGameStart;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        photoCapture = GetComponent<PhotoCapture>();
    }

    /// <summary>
    /// Invokes the static photo taken event with the player's index, notifying
    /// other components of the action.
    /// </summary>
    /// <param name="_playerIndex">The index of the player who took the photo.</param>
    public static void InvokePhotoTaken(int _playerIndex)
    {
        onTakePhoto?.Invoke(_playerIndex);
    }

    /// <summary>
    /// Processes the "Take Photo" input to capture a photo if the player is allowed to take one.
    /// The event is dispatched upon success.
    /// </summary>
    /// <param name="_context">The context of the "Take Photo" input action.</param>
    public void OnTakePhoto(InputAction.CallbackContext _context)
    {
        if (!_context.started) return;
        if (!photoCapture.CanTakePhoto()) return;
        
        Debug.Log($"OnTakePhoto wurde aufgerufen für Spieler {PlayerIndex}");
        onTakePhoto?.Invoke(PlayerIndex);
    }

    /// <summary>
    /// Pauses the game when a valid pause input action is detected and invokes
    /// the assigned pause Unity event.
    /// </summary>
    /// <param name="_context">The context of the pause input action.</param>
    public void OnPause(InputAction.CallbackContext _context)
    {
        if (!pauseInputEnabled) return;
        if (!_context.started) return;

        onPause.Invoke();
    }

    /// <summary>
    /// Unpauses the game when a valid unpause input action is detected and triggers
    /// the associated unpause Unity event.
    /// </summary>
    /// <param name="_context">The context of the unpause input action.</param>
    public void OnUnpause(InputAction.CallbackContext _context)
    {
        if (!pauseInputEnabled) return;
        if (!_context.started) return;

        onUnpause.Invoke();
    }

    /// <summary>
    /// Triggers the `onGameStart` Unity event, indicating the start of the game.
    /// </summary>
    /// <param name="_context">The context of the start game input action.</param>
    public void OnStartGame(InputAction.CallbackContext _context)
    {
        onGameStart.Invoke();
    }
    
    /// <summary>
    /// Switches the player's current input action map to "SwaggySnapshots," enabling
    /// controls specific to the Swaggy Snapshots game mode.
    /// </summary>
    public override void SwitchToPlayerInputMap()
    {
        playerInput.SwitchCurrentActionMap("SwaggySnapshots");
    }

    /// <summary>
    /// Locks the mouse cursor, typically for gameplay purposes, making the cursor invisible and immovable.
    /// </summary>
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Unlocks the mouse cursor, making it usable freely in the UI or external interactions.
    /// </summary>
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
