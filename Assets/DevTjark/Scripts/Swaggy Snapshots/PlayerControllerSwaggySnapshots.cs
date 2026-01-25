using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControllerSwaggySnapshots : MonoBehaviour
{
    public int PlayerIndex;

    private InputAction m_takePhotoInputAction;
    private InputAction m_pauseInputAction;
    private InputAction m_unpauseInputAction;
    
    private PlayerInput playerInput;

    [SerializeField] private UnityEvent onTakePhoto;
    [SerializeField] private UnityEvent onPause;
    [SerializeField] private UnityEvent onUnpause;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }
    
    private void MapInputActions() 
    {
        m_takePhotoInputAction = playerInput.actions["TakePhoto"];
        // m_takePhotoInputAction.started += OnTakePhoto;

        m_pauseInputAction = playerInput.actions["Pause"];
        m_pauseInputAction.started += OnPause;

        m_unpauseInputAction = playerInput.actions["Unpause"];
        m_unpauseInputAction.started += OnUnpause;
    }

    public void OnTakePhoto(InputAction.CallbackContext _obj)
    {
        onTakePhoto.Invoke();
    }

    private void OnPause(InputAction.CallbackContext _obj)
    {
        onPause.Invoke();
    }

    private void OnUnpause(InputAction.CallbackContext _obj)
    {
        onUnpause.Invoke();
    }
}
