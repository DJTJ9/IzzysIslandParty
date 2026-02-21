using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerControllerMinigolfMayhem : Controller
{
    [Header("Movement")]
    private RigidbodyMovement rigidbodyMovement;

    [HideInInspector] public int ShootCount;

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
        ShootCount = 0;
    }

    public override void SwitchToPlayerInputMap()
    {
        playerInput.SwitchCurrentActionMap("MinigolfMayhem");
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
        if (!m_isActive) return;
        
        rigidbodyMovement.Move(_context.ReadValue<Vector2>());
    }

    public void OnShootInput(InputAction.CallbackContext _context)
    {
        if (!m_isActive) return;

        rigidbodyMovement.StartCharging(_context);
        ++ShootCount;
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