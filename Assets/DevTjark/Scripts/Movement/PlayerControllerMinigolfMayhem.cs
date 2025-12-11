using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerMinigolfMayhem : MonoBehaviour {
    [Header("Movement")]
    private RigidbodyMovement rigidbodyMovement;
    // public CameraRotator CameraRotator;
    
    [Header("Input")]
    private PlayerInput playerInput;

    // [Header("Settings")]
    // [SerializeField] private float lookSensitivity = 2;

    private InputAction moveInputAction;
    private InputAction jumpInputAction;
    private InputAction lookInputAction;
    private InputAction shootInputAction;

    private void Awake() {
        rigidbodyMovement = GetComponent<RigidbodyMovement>();
        playerInput = GetComponent<PlayerInput>();
        
        MapInputActions();
    }

    /// <summary>
    /// Sets cursor lock mode on left click to locked and on escape to none.
    /// Gets move direction from input and moves rigidbody into this direction.
    /// Rotates the rigidbody horizontally if cursor lock mode is locked.
    /// </summary>
    private void Update() {
        if (Mouse.current.rightButton.wasPressedThisFrame) Cursor.lockState = CursorLockMode.Locked;
        if (Keyboard.current.escapeKey.wasPressedThisFrame) Cursor.lockState = CursorLockMode.None;

        // var moveDirection = GetMoveDirectionFromInput();
        // RigidbodyMovement.Move(moveDirection);
        
        // if (Cursor.lockState == CursorLockMode.Locked) {
        //     var rotation = GetRotationFromInput();
        //     rigidbodyMovement.RotateHorizontal(rotation.x * lookSensitivity);
        // }
    }

    // /// <summary>
    // /// Rotates camera vertically if cursor lock mode is locked.
    // /// </summary>
    // private void LateUpdate() {
    //     if (Cursor.lockState == CursorLockMode.Locked) {
    //         if (CameraRotator != null)
    //             UpdateCamera();
    //     }
    // }

    /// <summary>
    /// Gets rotation from input
    /// Rotates camera in the direction of the rotation input
    /// </summary>
    private void UpdateCamera() {
        // var rotation = GetRotationFromInput();
        // CameraRotator.Rotate(rotation.y);
    }

    /// <summary>
    /// Maps the input actions
    /// Subcribes methods to their matching input actions
    /// </summary>
    private void MapInputActions() {
        moveInputAction = playerInput.actions["Move"];
        moveInputAction.started += OnMoveInput;

        jumpInputAction = playerInput.actions["Jump"];
        jumpInputAction.started += OnJumpInput;

        lookInputAction = playerInput.actions["Look"];

        shootInputAction = playerInput.actions["Shoot"];
        shootInputAction.started += OnShootInput;
    }

    private void OnMoveInput(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started)
            rigidbodyMovement.Move(GetMoveDirectionFromInput());
    }
    
    private void OnShootInput(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started)
            rigidbodyMovement.Shoot();
    }

    private void OnJumpInput(InputAction.CallbackContext _context) {
        if (_context.phase == InputActionPhase.Started)
            rigidbodyMovement.Jump();
    }

    /// <summary>
    /// Gets the horizontal move direction from the input
    /// Converts this input into a 3D vector and returns it
    /// </summary>
    private Vector3 GetMoveDirectionFromInput() {
        var moveInput = moveInputAction.ReadValue<Vector2>();
        return new Vector3(moveInput.x, 0f, moveInput.y);

    }

    /// <summary>
    /// Gets the rotation input and returns it
    /// </summary>
    private Vector2 GetRotationFromInput() {
        return lookInputAction.ReadValue<Vector2>();
    }
}
