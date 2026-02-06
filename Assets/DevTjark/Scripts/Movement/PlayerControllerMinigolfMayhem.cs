using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControllerMinigolfMayhem : Controller
{
    // public int PlayerIndex { get; private set; }

    [Header("Movement")]
    private RigidbodyMovement rigidbodyMovement;

    public CameraRotator CameraRotator;

    [Header("Input")]
    private PlayerInput playerInput;

    [Header("Settings")]
    [SerializeField]
    private float lookSensitivity = 2;

    [FoldoutGroup("Events", expanded: true)]
    [SerializeField]
    private UnityEvent OnPause;

    [SerializeField]
    private UnityEvent OnUnpause;

    private InputAction moveInputAction;
    private InputAction jumpInputAction;
    private InputAction lookInputAction;
    private InputAction shootInputAction;
    private InputAction pauseInputAction;
    private InputAction unpauseInputAction;

    private void Awake()
    {
        rigidbodyMovement = GetComponent<RigidbodyMovement>();
        playerInput = GetComponent<PlayerInput>();

        MapInputActions();
    }

    private void OnEnable()
    {
        playerInput.SwitchCurrentActionMap("Player");
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
        // RigidbodyMovement.Move(moveDirection);

        // if (Cursor.lockState == CursorLockMode.Locked) {
        var rotation = GetRotationFromInput();
        rigidbodyMovement.RotateHorizontal(rotation.x * lookSensitivity);
        // }
    }

    /// <summary>
    /// Rotates camera vertically if cursor lock mode is locked.
    /// </summary>
    private void LateUpdate()
    {
        if (CameraRotator != null)
            UpdateCamera();
    }

    /// <summary>
    /// Gets rotation from input
    /// Rotates camera in the direction of the rotation input
    /// </summary>
    private void UpdateCamera()
    {
        var rotation = GetRotationFromInput();
        CameraRotator.Rotate(rotation.y);
    }

    public void Initialize(int playerIndex)
    {
        PlayerIndex = playerIndex;
    }

    /// <summary>
    /// Maps the input actions
    /// Subcribes methods to their matching input actions
    /// </summary>
    private void MapInputActions()
    {
        //     moveInputAction = playerInput.actions["Move"];
        //     moveInputAction.started += OnMoveInput;
        //
        //     jumpInputAction = playerInput.actions["Jump"];
        //     jumpInputAction.started += OnJumpInput;
        //
        lookInputAction = playerInput.actions["Look"];
        //
        //     shootInputAction = playerInput.actions["LeftMouse"];
        //     shootInputAction.started += OnShootInput;
        //
        //     pauseInputAction = playerInput.actions["Pause"];
        //     pauseInputAction.started += OnPauseInput;
        //     
        //     unpauseInputAction = playerInput.actions["Unpause"];
        //     unpauseInputAction.started += OnUnpauseInput;
    }

    public void SwitchToPlayerInputMap()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }

    public void SwitchToUIInputMap()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

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
        rigidbodyMovement.Move(_context.ReadValue<Vector2>());
    }

    public void OnShootInput(InputAction.CallbackContext _context)
    {
        rigidbodyMovement.Shoot();
    }

    public void OnJumpInput(InputAction.CallbackContext _context)
    {
        rigidbodyMovement.Jump();
    }

    public void OnLookInput(InputAction.CallbackContext _context)
    {
        var rotation = _context.ReadValue<Vector2>();
        rigidbodyMovement.RotateHorizontal(rotation.x * lookSensitivity);
        UpdateCamera();
    }

    /// <summary>
    /// Gets the horizontal move direction from the input
    /// Converts this input into a 3D vector and returns it
    /// </summary>
    public Vector3 GetMoveDirectionFromInput()
    {
        var moveInput = moveInputAction.ReadValue<Vector2>();
        return new Vector3(moveInput.x, 0f, moveInput.y);
    }

    /// <summary>
    /// Gets the rotation input and returns it
    /// </summary>
    public Vector2 GetRotationFromInput()
    {
        return lookInputAction.ReadValue<Vector2>();
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