using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine.InputSystem.UI;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController), typeof(Rigidbody))]
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
    private CharacterController controller;
    // private PlayerInput playerInput;
    private Rigidbody rb;

    [FoldoutGroup("Unity Events", expanded: false)]
    [SerializeField] private UnityEvent onPause;
    [SerializeField] private UnityEvent onUnpause;
    [SerializeField] private UnityEvent onGameStart;

    [FoldoutGroup("Scriptable Objects", expanded: false)]
    [SerializeField] private SO_Player playerSO;
    [SerializeField] private SO_PlayerCollection playerCollectionBB;

private void Start()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        GameStartConfiguration();
    }

    // private void OnDisable()
    // {
    //     UnmapInputActions();
    // }

    private void FixedUpdate()
    {
        if (controller.enabled) Movement();
    }

    public void GameStartConfiguration()
    {
        // MapInputActions();
        ResetComponents();
    }
    
    private void Movement()
    {
        StartPositionMovement();
    }
    
    private void StartPositionMovement()
    {
        var move = new Vector3(m_moveInput.x, m_moveInput.y, 0);
    
        move *= m_moveSpeed;
        
        controller.Move(move * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext _context)
    {
        m_moveInput = _context.ReadValue<Vector2>();
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
    
    // private void MapInputActions() 
    // {
    //     m_moveInputAction = playerInput.actions["Move"];
    //
    //     m_pauseInputAction = playerInput.actions["Pause"];
    //     m_pauseInputAction.started += OnPause;
    //
    //     m_unpauseInputAction = playerInput.actions["Unpause"];
    //     m_unpauseInputAction.started += OnUnpause;
    //
    //     m_startInputAction = playerInput.actions["StartGame"];
    //     m_startInputAction.started += OnStartGame;
    // }

    // private void UnmapInputActions()
    // {
    //     m_pauseInputAction.started -= OnPause;
    //     m_unpauseInputAction.started -= OnUnpause;
    // }

    public void OnStartGame(InputAction.CallbackContext _context)
    {
        if (!_context.started) return;
        
        onGameStart.Invoke();
    }

    public void OnPause(InputAction.CallbackContext _context)
    {
        if (!pauseInputEnabled) return;
        if (!_context.started) return;
        
        onPause.Invoke();
    }
    
    public void OnUnpause(InputAction.CallbackContext _context)
    {
        if (!pauseInputEnabled) return;
        if (!_context.started) return;
        
        onUnpause.Invoke();
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

    public override void SwitchToPlayerInputMap()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }

    public void BindPlayerSO(SO_Player _playerSO)
    {
        playerSO = _playerSO;
        transform.position = playerSO.SpawnPoint;
    }
}
