using System;
using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent (typeof(GroundChecker))]
public class GoapRigidbodyMovement : Controller
{
    [FoldoutGroup("Push Settings", expanded: true)]
    [SerializeField] private float pushForce;
    [SerializeField] private float pushCooldown;
    
    [FoldoutGroup("Shoot Settings", expanded: true)]
    public float MovementCooldown;
    [SerializeField] private float shootForceFactor;
    
    [FoldoutGroup("Jump Settings", expanded: true)]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    
    [FoldoutGroup("Hit Impulse Settings", expanded: true)]
    [SerializeField] private float horizontalImpactForce;
    [SerializeField] private float verticalImpactForce;
    [SerializeField] private float impulseCooldown;

    [HideInInspector] public int ShotsTaken;

    private Rigidbody rb;
    private GroundChecker groundChecker;

    private Vector3 moveDirection;
    private bool m_canMove = true;
    private bool m_impulseApplied;
    
    [HideInInspector] public CountdownTimer MovementCooldownTimer;
    private CountdownTimer impulseCooldownTimer;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        groundChecker = GetComponent<GroundChecker>();
        
        MovementCooldownTimer = new CountdownTimer(MovementCooldown);
        MovementCooldownTimer.OnTimerStop += EnableMovement;
        impulseCooldownTimer = new CountdownTimer(impulseCooldown);
        impulseCooldownTimer.OnTimerStop += () => m_impulseApplied = false;
    }

    private void FixedUpdate()
    {
        MovementCooldownTimer.Tick(Time.deltaTime);
        impulseCooldownTimer.Tick(Time.deltaTime);
    }

    /// <summary>
    /// Applies an impulse force to colliding players, based on the current direction and velocity,
    /// if cooldown conditions allow. Prevents impulses beyond a certain frequency.
    /// </summary>
    /// <param name="other">The collider that triggered the interaction.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (m_impulseApplied) return;
        impulseCooldownTimer.Start();

        var impactDirection = rb.linearVelocity.normalized;
        var impulse = impactDirection * (horizontalImpactForce * rb.linearVelocity.magnitude)
                      + Vector3.up * (verticalImpactForce * rb.linearVelocity.magnitude);

        if (!other.TryGetComponent<Rigidbody>(out var _rb)) return;
        var currentVelocity = _rb.linearVelocity;
        if (rb.linearVelocity.magnitude > currentVelocity.magnitude) return;

        _rb.AddForce(impulse, ForceMode.Impulse);
    }
    
    /// <summary>
    /// Resets the impulse application flag when an object leaves the trigger zone.
    /// </summary>
    /// <param name="other">The collider leaving the trigger zone.</param>
    private void OnTriggerExit(Collider other)
    {
        m_impulseApplied = false;
    }

    /// <summary>
    /// Executes the "shoot" action by calculating and applying an impulse force to the rigidbody
    /// to reach the target position. Tracks the number of shots taken and initiates a movement cooldown.
    /// </summary>
    /// <param name="_targetPosition">The target position the shot will aim for.</param>
    public void Shoot(Vector3 _targetPosition)
    {
        if (!m_isActive) return;
        if (!groundChecker.IsGrounded) return;
        if (!m_canMove) return;
        
        var impulse = CalculateImpulse(rb, _targetPosition);
        rb.AddForce(impulse, ForceMode.Impulse);
        
        ++ShotsTaken;
        MovementCooldownTimer.Reset();
        MovementCooldownTimer.Start();
        m_canMove = false;
    }
    
    /// <summary>
    /// Calculates an impulse vector required to move the rigidbody to the target position,
    /// considering factors like mass, gravity, and random variations in horizontal speed.
    /// </summary>
    /// <param name="_rb">The rigidbody being moved.</param>
    /// <param name="target">The desired target position.</param>
    /// <returns>A vector representing the force to apply for movement.</returns>
    private Vector3 CalculateImpulse(Rigidbody _rb, Vector3 target)
    {
        var start = _rb.position;
        var toTarget = target - start;

        var mass = _rb.mass;
        var gravity = Physics.gravity;

        var horizontal = new Vector3(toTarget.x, 0f, toTarget.z);
        var horizontalDistance = horizontal.magnitude;
        var horizontalSpeedFactor = shootForceFactor + Random.Range(-2f, 2f);
        var t = horizontalDistance / horizontalSpeedFactor;
        var vHorizontal = horizontal / t;

        var y = toTarget.y;
        var vVertical = (y - 0.5f * gravity.y * t * t) / t;
        var v0 = vHorizontal + Vector3.up * vVertical;

        return mass * v0;
    }
    
    /// <summary>
    /// Enables movement functionality by resetting the "can move" flag,
    /// usually called after a movement cooldown ends.
    /// </summary>
    private void EnableMovement()
    {
        m_canMove = true;
    }
}
