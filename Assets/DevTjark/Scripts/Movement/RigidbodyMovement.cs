using UnityEngine;
using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GroundChecker))]
public class RigidbodyMovement : MonoBehaviour
{
    [FoldoutGroup("Push Settings", expanded: true)] 
    [SerializeField] private float pushForce;
    [SerializeField] private float pushCooldown;

    [FoldoutGroup("Shoot Settings", expanded: true)] 
    [SerializeField] public float minShootForce;
    [SerializeField] public float maxShootForce;
    [SerializeField] private float shootForceChangeSpeed;
    [SerializeField] private float shootCooldown;
    [SerializeField] private float shootHeight;

    [FoldoutGroup("Jump Settings", expanded: true)] 
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;

    [FoldoutGroup("Hit Impulse Settings", expanded: true)] 
    [SerializeField] private float horizontalImpactForce;
    [SerializeField] private float verticalImpactForce;
    [SerializeField] private float impulseCooldown;


    [SerializeField] private Camera cam;
    private Rigidbody rb;
    private GroundChecker groundChecker;

    public float CurrentShootForce;
    private Vector3 m_moveDirection;
    private bool m_canMove = true;
    private bool m_canShoot = true;
    private bool m_canJump = true;
    private bool m_isCharging;
    private bool m_isIncreasing = true;
    private bool m_impulseApplied;

    private CountdownTimer pushCooldownTimer;
    private CountdownTimer shootCooldownTimer;
    private CountdownTimer jumpCooldownTimer;
    private CountdownTimer impulseCooldownTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        groundChecker = GetComponent<GroundChecker>();

        pushCooldownTimer = new CountdownTimer(pushCooldown);
        pushCooldownTimer.OnTimerStop += EnableMovement;

        shootCooldownTimer = new CountdownTimer(shootCooldown);
        shootCooldownTimer.OnTimerStop += EnableShooting;

        jumpCooldownTimer = new CountdownTimer(jumpCooldown);
        jumpCooldownTimer.OnTimerStop += EnableJumping;

        impulseCooldownTimer = new CountdownTimer(impulseCooldown);
        impulseCooldownTimer.OnTimerStart += () => m_impulseApplied = true;
        impulseCooldownTimer.OnTimerStop += () => m_impulseApplied = false;

        CurrentShootForce = minShootForce;
    }

    private void FixedUpdate()
    {
        UpdateChargePower();
    }

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
        ConsoleProDebug.LogToFilter($"Player applied {impulse} impulse to {other.name}", "Debug");
    }

    // private void OnTriggerExit(Collider other)
    // {
    //     m_impulseApplied = false;
    // }

    public void StartCharging(InputAction.CallbackContext _context)
    {
        if (!groundChecker.IsGrounded) return;

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
    /// Receives a move direction
    /// </summary>
    public void Move(Vector3 _direction)
    {
        if (!m_canMove) return;

        var camFwd = cam.transform.forward;
        var camRight = cam.transform.right;

        camFwd.y = 0f;
        camRight.y = 0f;
        camFwd.Normalize();
        camRight.Normalize();

        var worldDir = camRight * _direction.x + camFwd * _direction.y;

        rb.AddForce(worldDir.normalized * pushForce, ForceMode.Impulse);

        pushCooldownTimer.Reset();
        pushCooldownTimer.Start();
        m_canMove = false;
    }

    public void Jump()
    {
        if (!m_canJump) return;
        if (!groundChecker.IsGrounded) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        jumpCooldownTimer.Reset();
        jumpCooldownTimer.Start();
        m_canJump = false;
    }

    private void Shoot()
    {
        if (!m_canShoot) return;
        if (!groundChecker.IsGrounded) return;

        var screenCenter = cam.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0f));

        var ray = cam.ScreenPointToRay(screenCenter);

        var targetPoint = ray.GetPoint(500f);
        targetPoint.y = shootHeight;

        var direction = (targetPoint - rb.transform.position).normalized;

        rb.AddForce(direction * CurrentShootForce, ForceMode.Impulse);

        CurrentShootForce = minShootForce;
        shootCooldownTimer.Reset();
        shootCooldownTimer.Start();
        m_canShoot = false;
    }

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

    private void EnableMovement()
    {
        m_canMove = true;
    }

    private void EnableShooting()
    {
        m_canShoot = true;
    }

    private void EnableJumping()
    {
        m_canJump = true;
    }

    // /// <summary>
    // /// Collects the current Velocity of the rb and sets the speed
    // /// Transforms moving direction from local space to world space
    // /// Collects the speed difference to target velocity and clamps the max velocity
    // /// Sets force mode to VelocityChange
    // /// </summary>
    // private void UpdateHorizontalMovement()
    // {
    //     Vector3 currentVelocity = rb.linearVelocity;
    //     Vector3 targetVelocity = new Vector3(m_moveDirection.x, 0f , m_moveDirection.z);
    //     targetVelocity *= pushForce;
    //
    //     targetVelocity = transform.TransformDirection(targetVelocity);
    //
    //     Vector3 velocityChange = targetVelocity - currentVelocity;
    //     velocityChange = new Vector3(velocityChange.x, 0f, velocityChange.z);
    //     velocityChange = Vector3.ClampMagnitude(velocityChange, maxSpeed);
    //
    //     rb.AddForce(velocityChange, ForceMode.Force);
    // }
    //
    // /// <summary>
    // /// Recieves the current rotation
    // /// Sets the rotation to a target rotation
    // /// </summary>
    // public void RotateHorizontal(float _rotation)
    // {
    //     var currentRotation = rb.rotation.eulerAngles;
    //     var targetRotation = currentRotation + new Vector3(0f, _rotation, 0f);
    //     rb.rotation = Quaternion.Euler(targetRotation);
    // }
    //
    // /// <summary>
    // /// Modifies jump and fall speed
    // /// </summary>
    // private void UpdateVerticalMovement()
    // {
    //     if (rb.linearVelocity.y < 0)
    //         rb.linearVelocity += Vector3.up * (Physics.gravity.y * (fallSpeedModifier - 1) * Time.fixedDeltaTime);
    //
    //     if (rb.linearVelocity.y > 0)
    //         rb.linearVelocity += Vector3.up * (Physics.gravity.y * jumpSpeedModifier * Time.fixedDeltaTime);
    // }
    public float GetCurrentVelocity()
    {
        return rb.linearVelocity.magnitude;
    }
}