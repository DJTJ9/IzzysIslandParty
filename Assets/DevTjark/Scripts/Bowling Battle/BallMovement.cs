using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent (typeof(CharacterController), typeof(PlayerInput), typeof(Rigidbody))]
public class BallMovement : MonoBehaviour
{
    [Header("Input")]
    private Vector2 m_moveInput;
    private bool m_jumpInput;

    [Header("Movement Settings")]
    [SerializeField] private float m_moveSpeed = 5f;
    
    private InputAction m_moveInputAction;
    private InputAction m_jumpInputAction;
    private InputAction m_pauseInputAction;
    private InputAction m_unpauseInputAction;

    [Header("References")]
    private CharacterController controller;
    private PlayerInput playerInput;
    private Rigidbody rb;

    [SerializeField] private UnityEvent onPause;
    [SerializeField] private UnityEvent onUnpause;
    
    private Transform m_startPosition;
    
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        playerInput.enabled = true;
        
        m_startPosition = transform;
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
        Vector3 move = new Vector3(m_moveInput.x, m_moveInput.y, 0);

        move *= m_moveSpeed;
        
        controller.Move(move * Time.deltaTime);
    }

    private void GetMoveDirection()
    {
        m_moveInput = m_moveInputAction.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext _context)
    {
            if (controller == null) return;
            controller.enabled = false;
            rb.freezeRotation = false;
            rb.useGravity = true;
            m_moveInput = Vector2.zero;
            rb.linearVelocity = Vector3.zero;
    }
    
    public void OnReleaseBall()
    {
            if (controller == null) return;
            controller.enabled = false;
            rb.freezeRotation = false;
            rb.useGravity = true;
            m_moveInput = Vector2.zero;
            rb.linearVelocity = Vector3.zero;
    }
    
    private void MapInputActions() 
    {
        m_moveInputAction = playerInput.actions["Move"];

        m_jumpInputAction = playerInput.actions["Jump"];
        m_jumpInputAction.started += OnJump;

        m_pauseInputAction = playerInput.actions["Pause"];
        m_pauseInputAction.started += OnPause;

        m_unpauseInputAction = playerInput.actions["Unpause"];
        m_unpauseInputAction.started += OnUnpause;
    }
    
    private void OnPause(InputAction.CallbackContext _context)
    {
        onPause.Invoke();
    }
    
    private void OnUnpause(InputAction.CallbackContext _context)
    {
        onUnpause.Invoke();
    }

    private void UnmapInputActions()
    {
        m_jumpInputAction.started -= OnJump;
        m_pauseInputAction.started -= OnPause;
        m_unpauseInputAction.started -= OnUnpause;
    }

    public void ResetComponents()
    {
        controller.enabled = true;
        playerInput.enabled = true;
        rb.freezeRotation = true;
        rb.useGravity = false;
        transform.position = m_startPosition.position;
    }

    public void SwitchToPlayerInputMap()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }
    
    public void SwitchToUIInputMap()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }
    
    private void SetCameraForPlayerInput()
    {
        playerInput.camera = Camera.main;
    }
}
