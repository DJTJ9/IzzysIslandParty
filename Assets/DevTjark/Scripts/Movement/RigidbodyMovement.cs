using UnityEngine;
using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GroundChecker))]
public class RigidbodyMovement : MonoBehaviour
{
    [HideInInspector] public float CurrentShootForce;
    
    [FoldoutGroup("Push Settings", expanded: true)] 
    [SerializeField] private float pushForce;
    [SerializeField] private float pushCooldown;

    [FoldoutGroup("Shoot Settings", expanded: true)] 
    public float MovementCooldown;
    [SerializeField] public float minShootForce;
    [SerializeField] public float maxShootForce;
    [SerializeField] private float shootForceChangeSpeed;
    [SerializeField] private float shootHeight;

    [FoldoutGroup("Jump Settings", expanded: true)] 
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;

    [FoldoutGroup("Hit Impulse Settings", expanded: true)] 
    [SerializeField] private float horizontalImpactForce;
    [SerializeField] private float verticalImpactForce;
    [SerializeField] private float impulseCooldown;

    [SerializeField] private Camera playerCamera;
    private Rigidbody rb;
    private GroundChecker groundChecker;

    [HideInInspector] public int ShotsTaken;
    
    private Vector3 m_moveDirection;
    private bool m_canMove = true;
    private bool m_isCharging;
    private bool m_isIncreasing = true;
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
        impulseCooldownTimer.OnTimerStart += () => m_impulseApplied = true;
        impulseCooldownTimer.OnTimerStop += () => m_impulseApplied = false;

        CurrentShootForce = minShootForce;
    }

    private void FixedUpdate()
    {
        UpdateChargePower();
        MovementCooldownTimer.Tick(Time.deltaTime);
        impulseCooldownTimer.Tick(Time.deltaTime);
    }

    /// <summary>
    /// Handles impulse application when colliding with another player,
    /// applying a force if cooldown restrictions are respected.
    /// </summary>
    /// <param name="other">The collider entering the trigger zone.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (m_impulseApplied) return;
        impulseCooldownTimer.Start();

        var impactDirection = rb.linearVelocity.normalized;
        var impulse = impactDirection * (horizontalImpactForce * rb.linearVelocity.magnitude)
                      + Vector3.up * (verticalImpactForce * rb.linearVelocity.magnitude);

        if (!other.TryGetComponent<Rigidbody>(out var _rb)) return;
        var currentVelocity = Mathf.Abs(_rb.linearVelocity.magnitude);
        if (rb.linearVelocity.magnitude < currentVelocity) return;

        _rb.AddForce(impulse, ForceMode.Impulse);
    }

    /// <summary>
    /// Starts or completes the process of charging and shooting. Shooting
    /// applies a force toward a target defined by the camera's direction.
    /// </summary>
    /// <param name="_context">The input context controlling the action.</param>
    public void StartCharging(InputAction.CallbackContext _context)
    {
        if (_context.started)
        {
            m_isCharging = true;
            CurrentShootForce = minShootForce;
            m_isIncreasing = true;
        }
        else if (_context.canceled)
        {
            Shoot();
            m_isCharging = false;
        }
    }

    /// <summary>
    /// Moves the player in the given direction based on camera orientation,
    /// applying an impulse force and starting a movement cooldown.
    /// </summary>
    /// <param name="_direction">The direction of movement.</param>
    public void Move(Vector3 _direction)
    {
        if (!m_canMove) return;

        var camFwd = playerCamera.transform.forward;
        var camRight = playerCamera.transform.right;

        camFwd.y = 0f;
        camRight.y = 0f;
        camFwd.Normalize();
        camRight.Normalize();

        var worldDir = camRight * _direction.x + camFwd * _direction.y;

        rb.AddForce(worldDir.normalized * pushForce, ForceMode.Impulse);

        MovementCooldownTimer.Reset();
        MovementCooldownTimer.Start();
        m_canMove = false;
    }

    /// <summary>
    /// Makes the player jump with an upward impulse, ensuring movement cooldown
    /// and grounding restrictions are respected.
    /// </summary>
    public void Jump()
    {
        if (!m_canMove) return;
        if (!groundChecker.IsGrounded) return;
    
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    
        MovementCooldownTimer.Reset();
        MovementCooldownTimer.Start();
        m_canMove = false;
    }

    /// <summary>
    /// Shoots the player toward a target defined by the camera's center,
    /// applying a calculated force and updating shot statistics.
    /// </summary>
    private void Shoot()
    {
        if (!m_canMove) return;
        if (!groundChecker.IsGrounded) return;

        var screenCenter = playerCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0f));

        var ray = playerCamera.ScreenPointToRay(screenCenter);

        var targetPoint = ray.GetPoint(500f);
        targetPoint.y = shootHeight;

        var direction = (targetPoint - rb.transform.position).normalized;

        rb.AddForce(direction * CurrentShootForce, ForceMode.Impulse);

        ++ShotsTaken;
        CurrentShootForce = minShootForce;
        MovementCooldownTimer.Reset();
        MovementCooldownTimer.Start();
        m_canMove = false;
    }

    /// <summary>
    /// Adjusts the current shoot force during the charging process. 
    /// Alternates between increasing and decreasing the force within the defined minimum 
    /// and maximum range, based on `shootForceChangeSpeed`.
    /// </summary>
    private void UpdateChargePower()
    {
        if (!m_isCharging) return;

        if (m_isIncreasing)
        {
            CurrentShootForce += shootForceChangeSpeed * Time.deltaTime;

            if (CurrentShootForce >= maxShootForce)
            {
                CurrentShootForce = maxShootForce;
                m_isIncreasing = false;
            }
        }
        else
        {
            CurrentShootForce -= shootForceChangeSpeed * Time.deltaTime;

            if (CurrentShootForce <= minShootForce)
            {
                CurrentShootForce = minShootForce;
                m_isIncreasing = true;
            }
        }
    }

    /// <summary>
    /// Enables player movement after a cooldown ends, resetting the movement flag.
    /// </summary>
    private void EnableMovement()
    {
        m_canMove = true;
    }
}