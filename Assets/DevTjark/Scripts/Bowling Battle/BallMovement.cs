using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent (typeof(CharacterController), typeof(PlayerInput), typeof(Rigidbody))]
public class BallMovement : MonoBehaviour
{
    [Header("Input")]
    private Vector2 moveInput;
    private bool jumpInput;

    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    
    private InputAction moveInputAction;
    private InputAction jumpInputAction;

    [Header("References")]
    private CharacterController controller;
    private PlayerInput playerInput;
    private Rigidbody rb;
    
    private Transform startPosition;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        playerInput.enabled = true;
        
        startPosition = transform;
    }

    private void OnEnable()
    {
        GameStartConfiguration();
    }
    
    private void OnDisable()
    {
        UnmapInputActions();
    }

    void FixedUpdate()
    {
        if (controller.enabled) Movement();
    }

    public void GameStartConfiguration()
    {
        MapInputActions();
        ResetComponents();
        SetCameraForPlayerInput();
    }
    
    private void Movement()
    {
        StartPositionMovement();
    }

    private void StartPositionMovement()
    {
        GetMoveDirection();
        Vector3 move = new Vector3(moveInput.x, moveInput.y, 0);

        move *= moveSpeed;
        
        controller.Move(move * Time.deltaTime);
    }

    private void GetMoveDirection()
    {
        moveInput = moveInputAction.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Started)
        {
            if (controller == null) return;
            controller.enabled = false;
            rb.freezeRotation = false;
            rb.useGravity = true;
            moveInput = Vector2.zero;
            rb.linearVelocity = Vector3.zero;
        }
    }
    
    public void OnReleaseBall()
    {
            if (controller == null) return;
            controller.enabled = false;
            rb.freezeRotation = false;
            rb.useGravity = true;
            moveInput = Vector2.zero;
            rb.linearVelocity = Vector3.zero;
    }
    
    private void MapInputActions() 
    {
        moveInputAction = playerInput.actions["Move"];

        jumpInputAction = playerInput.actions["Jump"];
        jumpInputAction.started += OnJump;
    }

    private void UnmapInputActions()
    {
        moveInputAction.started -= OnJump;
    }

    public void ResetComponents()
    {
        controller.enabled = true;
        playerInput.enabled = true;
        rb.freezeRotation = true;
        rb.useGravity = false;
        transform.position = startPosition.position;
    }
    
    private void SetCameraForPlayerInput()
    {
        playerInput.camera = Camera.main;
    }
}
