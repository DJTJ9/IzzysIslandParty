using ScriptableObjects;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControllerMinigolfMayhem : Controller
{
    [Header("Movement")]
    private RigidbodyMovement rigidbodyMovement;

    private MinigolfMayhemCameraController cameraController;

    [SerializeField] private SO_FloatVariable mouseSensitivityX;
    [SerializeField] private SO_FloatVariable mouseSensitivityY;

    public bool m_isGamepad;
    private bool m_registeredInput;

    // [FoldoutGroup("Events", expanded: true)] 
    // [SerializeField] private UnityEvent OnPause;
    // [SerializeField] private UnityEvent OnUnpause;
    // [SerializeField] private UnityEvent onGameStart;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rigidbodyMovement = GetComponent<RigidbodyMovement>();
        cameraController = GetComponent<MinigolfMayhemCameraController>();
    }

    // /// <summary>
    // /// Activates the "MinigolfMayhem" input action map and resets the shoot count.
    // /// Runs every time the player component is enabled.
    // /// </summary>
    // private void OnEnable()
    // {
    //     playerInput.SwitchCurrentActionMap("MinigolfMayhem");
    // }

    /// <summary>
    /// Switches the input action map for the player to "MinigolfMayhem," enabling minigolf-specific controls.
    /// </summary>
    public override void SwitchToPlayerInputMap()
    {
        playerInput.SwitchCurrentActionMap("MinigolfMayhem");
    }

    // /// <summary>
    // /// Invokes the pause event when a valid pause action is triggered.
    // /// </summary>
    // /// <param name="_context">The context of the pause input action.</param>
    // public void OnPauseInput(InputAction.CallbackContext _context)
    // {
    //     if (!pauseInputEnabled) return;
    //     if (!_context.started) return;
    //     
    //     OnPause.Invoke();
    // }
    //
    // /// <summary>
    // /// Invokes the unpause event when a valid unpause action is triggered.
    // /// </summary>
    // /// <param name="_context">The context of the unpause input action.</param>
    // public void OnUnpauseInput(InputAction.CallbackContext _context)
    // {
    //     if (!pauseInputEnabled) return;
    //     if (!_context.started) return;
    //     
    //     OnUnpause.Invoke();
    // }

    /// <summary>
    /// Moves the player based on the movement input received. 
    /// Movement is handled by the `RigidbodyMovement` component.
    /// </summary>
    /// <param name="_context">The context of the movement input action.</param>
    public void OnMoveInput(InputAction.CallbackContext _context)
    {
        if (!m_isActive) return;

        rigidbodyMovement.Move(_context.ReadValue<Vector2>());
    }

    /// <summary>
    /// Handles the shooting input by starting the charging process in `RigidbodyMovement`
    /// and increments the shoot count.
    /// </summary>
    /// <param name="_context">The context of the shooting input action.</param>
    public void OnShootInput(InputAction.CallbackContext _context)
    {
        if (!m_isActive) return;

        rigidbodyMovement.StartCharging(_context);
        // ++ShootCount;
    }

    /// <summary>
    /// Detects and processes the jump input. Triggers the jump logic in the `RigidbodyMovement`.
    /// </summary>
    /// <param name="_context">The context of the jump input action.</param>
    public void OnJumpInput(InputAction.CallbackContext _context)
    {
        if (!m_isActive) return;
        if (!_context.started) return;

        rigidbodyMovement.Jump();
    }

    public void OnLookInput(InputAction.CallbackContext _context)
    {
        if (m_registeredInput)
            return;

        var device = _context.control.device;

        if (device is Gamepad gamepad)
        {
            m_isGamepad = true;
        }
        else if (device is Keyboard keyboard)
        {
            m_isGamepad = false;
        }

        m_registeredInput = true;
    }

    /// <summary>
    /// Enables the rear view camera mode when the action is performed,
    /// and disables it when the action is canceled.
    /// </summary>
    /// <param name="_context">The context of the rear view input action.</param>
    public void OnRearViewInput(InputAction.CallbackContext _context)
    {
        if (_context.performed)
        {
            cameraController.EnableRearView();
        }

        if (_context.canceled)
        {
            cameraController.DisableRearView();
        }
    }

    /// <summary>
    /// Locks the mouse cursor, typically for gameplay.
    /// </summary>
    public void LockMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// Unlocks the mouse cursor, allowing the user to use it freely.
    /// </summary>
    public void UnlockMouseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}