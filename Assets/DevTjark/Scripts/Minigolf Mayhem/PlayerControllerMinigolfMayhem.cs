using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControllerMinigolfMayhem : Controller
{
    [Header("Movement")]
    private RigidbodyMovement rigidbodyMovement;

    [Header("Input")]
    // private PlayerInput playerInput;

    // [Header("Settings")]
    // [SerializeField] private float lookSensitivity = 2;

    [FoldoutGroup("Events", expanded: true)] 
    [SerializeField] private UnityEvent OnPause;
    [SerializeField] private UnityEvent OnUnpause;

    private void Awake()
    {
        rigidbodyMovement = GetComponent<RigidbodyMovement>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        playerInput.SwitchCurrentActionMap("MinigolfMayhem");
    }


    /// <summary>
    /// Sets cursor lock mode on left click to locked and on escape to none.
    /// Gets move direction from input and moves rigidbody into this direction.
    /// Rotates the rigidbody horizontally if cursor lock mode is locked.
    /// </summary>
    private void Update()
    {
        // if (Mouse.current.rightButton.wasPressedThisFrame) Cursor.lockState = CursorLockMode.Locked;
        // if (Mouse.current.rightButton.wasPressedThisFrame) Cursor.lockState = CursorLockMode.None;

        // var moveDirection = GetMoveDirectionFromInput();
        // rigidbodyMovement.Move(moveDirection);

        // if (Cursor.lockState == CursorLockMode.Locked) {
        // var rotation = GetRotationFromInput();
        // rigidbodyMovement.RotateHorizontal(rotation.x * lookSensitivity);
        // }
    }

    /// <summary>
    /// Maps the input actions
    /// Subcribes methods to their matching input actions
    /// </summary>
    // private void MapInputActions()
    // {
    //     //     moveInputAction = playerInput.actions["Move"];
    //     //     moveInputAction.started += OnMoveInput;
    //     //
    //     //     jumpInputAction = playerInput.actions["Jump"];
    //     //     jumpInputAction.started += OnJumpInput;
    //     //
    //     //     lookInputAction = playerInput.actions["Look"];
    //     //
    //     //     shootInputAction = playerInput.actions["LeftMouse"];
    //     //     shootInputAction.started += OnShootInput;
    //     //
    //     //     pauseInputAction = playerInput.actions["Pause"];
    //     //     pauseInputAction.started += OnPauseInput;
    //     //     
    //     //     unpauseInputAction = playerInput.actions["Unpause"];
    //     //     unpauseInputAction.started += OnUnpauseInput;
    // }
    public override void SwitchToPlayerInputMap()
    {
        playerInput.SwitchCurrentActionMap("MinigolfMayhem");
    }

    // public void SwitchToUIInputMap()
    // {
    //     playerInput.SwitchCurrentActionMap("UI");
    // }

    public void OnPauseInput(InputAction.CallbackContext _context)
    {
        OnPause.Invoke();
    }

    public void OnUnpauseInput(InputAction.CallbackContext _context)
    {
        OnUnpause.Invoke();
    }

    public void OnMoveInput(InputAction.CallbackContext _context)
    {
        if (!m_isActive) return;
        
        rigidbodyMovement.Move(_context.ReadValue<Vector2>());
    }

    public void OnShootInput(InputAction.CallbackContext _context)
    {
        if (!m_isActive) return;

        rigidbodyMovement.StartCharging(_context);
        // rigidbodyMovement.Shoot();
    }

    public void OnJumpInput(InputAction.CallbackContext _context)
    {
        if (!m_isActive) return;

        rigidbodyMovement.Jump();
    }

    public void LockMouseCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockMouseCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}