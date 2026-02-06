using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControllerSwaggySnapshots : Controller
{
    // public int PlayerIndex;

    private InputAction m_takePhotoInputAction;
    private InputAction m_pauseInputAction;
    private InputAction m_unpauseInputAction;
    
    private PlayerInput playerInput;

    [SerializeField] private GameEventInt onTakePhoto;
    [SerializeField] private UnityEvent onPause;
    [SerializeField] private UnityEvent onUnpause;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void OnTakePhoto(InputAction.CallbackContext _obj)
    {
        onTakePhoto.Raise(PlayerIndex);
    }

    public void OnPause(InputAction.CallbackContext _obj)
    {
        onPause.Invoke();
    }

    public void OnUnpause(InputAction.CallbackContext _obj)
    {
        onUnpause.Invoke();
    }
    
    public void SwitchToPlayerInputMap()
    {
        playerInput.SwitchCurrentActionMap("SwaggySnapshots");
    }
    
    public void SwitchToUIInputMap()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

    public int GetPlayerIndex() => PlayerIndex;
    public void SetPlayerIndex(int _playerIndex) => PlayerIndex = _playerIndex;
}
