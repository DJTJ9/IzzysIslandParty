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

    private void FixedUpdate()
    {
        if (controller.enabled) Movement();
    }

    private void GameStartConfiguration()
    {
        ResetComponents();
    }
    
    private void Movement()
    {
        StartPositionMovement();
    }
    
    /// <summary>
    /// Moves the player based on the current input vector (X and Y values), 
    /// applying them using the CharacterController.
    /// </summary>
    private void StartPositionMovement()
    {
        var move = new Vector3(m_moveInput.x, m_moveInput.y, 0);
    
        move *= m_moveSpeed;
        
        controller.Move(move * Time.deltaTime);
    }

    /// <summary>
    /// Captures and updates the movement input vector when the user moves the player.
    /// </summary>
    /// <param name="_context">The context of the movement input action.</param>
    public void OnMove(InputAction.CallbackContext _context)
    {
        m_moveInput = _context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Disables player movement and simulates ball release by enabling gravity 
    /// and resetting the Rigidbody's velocity.
    /// </summary>
    public void OnReleaseBall()
    {
            if (controller == null) return;
            controller.enabled = false;
            rb.freezeRotation = false;
            rb.useGravity = true;
            m_moveInput = Vector2.zero;
            rb.linearVelocity = Vector3.zero;
    }
    
    /// <summary>
    /// Invokes the game start event when the start game input is triggered.
    /// </summary>
    /// <param name="_context">The context of the start game action.</param>
    public void OnStartGame(InputAction.CallbackContext _context)
    {
        if (!_context.started) return;
        
        onGameStart.Invoke();
    }

    /// <summary>
    /// Pauses the game upon detecting a valid pause input action and invokes the pause event.
    /// </summary>
    /// <param name="_context">The context of the pause input action.</param>
    public void OnPause(InputAction.CallbackContext _context)
    {
        if (!pauseInputEnabled) return;
        if (!_context.started) return;
        
        onPause.Invoke();
    }
    
    /// <summary>
    /// Unpauses the game upon detecting a valid unpause input action and invokes the unpause event.
    /// </summary>
    /// <param name="_context">The context of the unpause input action.</param>
    public void OnUnpause(InputAction.CallbackContext _context)
    {
        if (!pauseInputEnabled) return;
        if (!_context.started) return;
        
        onUnpause.Invoke();
    }

    /// <summary>
    /// Resets all key player components (Rigidbody, CharacterController, PlayerInput) 
    /// and repositions the player to their spawn point, preparing them for the game.
    /// </summary>
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

    /// <summary>
    /// Switches the player's current input action map to the "Player" map, enabling player-specific controls.
    /// </summary>
    public override void SwitchToPlayerInputMap()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }

    /// <summary>
    /// Assigns a new `SO_Player` scriptable object to the player and updates their position to the specified spawn point.
    /// </summary>
    /// <param name="_playerSO">The scriptable object containing the player's data.</param>
    public void BindPlayerSO(SO_Player _playerSO)
    {
        playerSO = _playerSO;
        transform.position = playerSO.SpawnPoint;
    }
}
