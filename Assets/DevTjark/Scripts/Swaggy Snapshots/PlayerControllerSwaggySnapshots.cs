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
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        photoCapture = GetComponent<PhotoCapture>();
    }

    public static void InvokePhotoTaken(int _playerIndex)
    {
        onTakePhoto?.Invoke(_playerIndex);
    }

    
    public void OnTakePhoto(InputAction.CallbackContext _context)
    {
        if (!_context.started) return;
        if (!photoCapture.CanTakePhoto()) return;
        
        Debug.Log($"OnTakePhoto wurde aufgerufen für Spieler {PlayerIndex}");
        onTakePhoto?.Invoke(PlayerIndex);
    }

    public void OnPause(InputAction.CallbackContext _context)
    {
        if (!_context.started) return;

        onPause.Invoke();
    }

    public void OnUnpause(InputAction.CallbackContext _context)
    {
        if (!_context.started) return;

        onUnpause.Invoke();
    }
    
    public override void SwitchToPlayerInputMap()
    {
        playerInput.SwitchCurrentActionMap("SwaggySnapshots");
    }
}
