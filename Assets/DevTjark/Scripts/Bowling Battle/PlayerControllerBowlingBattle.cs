using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DG.Tweening;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;

[RequireComponent (typeof(CharacterController), typeof(Rigidbody))]
public class PlayerControllerBowlingBattle : Controller
{
    private int m_playerIndex;
    
    [Header("Input")]
    private Vector2 m_moveInput;
    private bool m_jumpInput;

    [Header("Movement Settings")]
    [SerializeField] private float m_moveSpeed = 5f;
    
    private InputAction m_moveInputAction;
    private InputAction m_pauseInputAction;
    private InputAction m_unpauseInputAction;
    private InputAction m_startInputAction;
    
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private InputSystemUIInputModule inputModule;
    private CharacterController controller;
    private PlayerInput playerInput;
    private Rigidbody rb;

    [SerializeField] private UnityEvent onPause;
    [SerializeField] private UnityEvent onUnpause;
    [SerializeField] private UnityEvent onGameStart;
    
    [SerializeField] private SO_Player playerSO;
    [SerializeField] private SO_PlayerCollection playerCollectionBB;
    [SerializeField] private SO_PlayerInputs playerInputsSO;
    
    private void Start()
    {
        // playerCollectionBB.Players[m_playerIndex].InitializePlayer(gameObject, playerCollectionBB.Players[m_playerIndex], m_playerIndex);
        // BindAndEnablePlayerInput();
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        // playerInput = playerInputsSO.PlayerInputs[NPCIndex];
        rb = GetComponent<Rigidbody>();
        GameStartConfiguration();
    }

    private void OnEnable()
    {
    }
    
    private void OnDisable()
    {
        UnmapInputActions();
    }

    private void FixedUpdate()
    {
        if (controller.enabled) Movement();
    }

    public void GameStartConfiguration()
    {
        MapInputActions();
        ResetComponents();
    }
    
    private void Movement()
    {
        StartPositionMovement();
    }

    private void StartPositionMovement()
    {
        GetMoveDirection();
        var move = new Vector3(m_moveInput.x, m_moveInput.y, 0);

        move *= m_moveSpeed;
        
        controller.Move(move * Time.deltaTime);
    }

    public void GetMoveDirection()
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

        m_pauseInputAction = playerInput.actions["Pause"];
        m_pauseInputAction.started += OnPause;

        m_unpauseInputAction = playerInput.actions["Unpause"];
        m_unpauseInputAction.started += OnUnpause;

        m_startInputAction = playerInput.actions["StartGame"];
        m_startInputAction.started += OnStartGame;
    }

    public void OnStartGame(InputAction.CallbackContext _context)
    {
        onGameStart.Invoke();
    }

    public void OnPause(InputAction.CallbackContext _context)
    {
        onPause.Invoke();
    }
    
    public void OnUnpause(InputAction.CallbackContext _context)
    {
        onUnpause.Invoke();
    }

    private void UnmapInputActions()
    {
        m_pauseInputAction.started -= OnPause;
        m_unpauseInputAction.started -= OnUnpause;
    }

    public void ResetComponents()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (controller == null) controller = GetComponent<CharacterController>();
        
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        controller.enabled = false;
        transform.position = playerSO.SpawnPoint;
        transform.rotation = Quaternion.identity;

        controller.enabled = true;
        rb.freezeRotation = true;
        rb.useGravity = false;
        
        if (playerInput == null) return;
        playerInput.enabled = true;
    }

    public void SwitchToPlayerInputMap()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }
    
    public void SwitchToUIInputMap()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void BindPlayerSO(SO_Player _playerSO)
    {
        playerSO = _playerSO;
        transform.position = playerSO.SpawnPoint;
    }
    
    public void BindAndEnablePlayerInput(PlayerInput _playerInput)
    {
        playerInput = _playerInput;
        playerInput.enabled = true;
        playerInput.SwitchCurrentActionMap("Player");
        MapInputActions();
    }

    public void BindAndEnablePlayerInput()
    {
        playerInput = playerInputsSO.PlayerInputs[m_playerIndex];
        playerInput.enabled = true;
        SwitchToPlayerInputMap();
        MapInputActions();
    }

    public void SetCameraForPlayerInput()
    {
        playerInput.camera = transform.parent.GetComponentInChildren<Camera>();
    }

    public void SetUIInputModuleToMultiplayerEventSystem()
    {
        playerInput.uiInputModule = inputModule;
    }
    
    public int GetPlayerIndex() => m_playerIndex;
    public void SetPlayerIndex(int _playerIndex) => m_playerIndex = _playerIndex;
}
