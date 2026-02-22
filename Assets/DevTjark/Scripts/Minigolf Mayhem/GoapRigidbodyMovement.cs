using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent (typeof(GroundChecker))]
public class GoapRigidbodyMovement : Controller
{
    [FoldoutGroup("Push Settings", expanded: true)]
    [SerializeField] private float pushForce;
    [SerializeField] private float pushCooldown;
    
    [FoldoutGroup("Shoot Settings", expanded: true)]
    [SerializeField] private float shootForceFactor;
    [SerializeField] private float shootCooldown;
    
    [FoldoutGroup("Jump Settings", expanded: true)]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    
    [FoldoutGroup("Hit Impulse Settings", expanded: true)]
    [SerializeField] private float horizontalImpactForce;
    [SerializeField] private float verticalImpactForce;
    [SerializeField] private float impulseCooldown;
    
    // [SerializeField] private float maxSpeed;
    // [SerializeField] private float jumpSpeedModifier = 1;
    // [SerializeField] private float fallSpeedModifier = 1;

    private Rigidbody rb;
    private GroundChecker groundChecker;

    private Vector3 moveDirection;
    private bool m_impulseApplied;
    
    private CountdownTimer impulseCooldownTimer;
    
    // private bool canMove = true;
    // private bool canShoot = true;
    // private bool canJump = true;
    //
    // private CountdownTimer pushCooldownTimer;
    // private CountdownTimer shootCooldownTimer;
    // private CountdownTimer jumpCooldownTimer;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        InitializeGroundChecker();
        
        impulseCooldownTimer = new CountdownTimer(impulseCooldown);
        impulseCooldownTimer.OnTimerStop += () => m_impulseApplied = false;
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
        var currentVelocity = _rb.linearVelocity;
        if (rb.linearVelocity.magnitude > currentVelocity.magnitude) return;

        _rb.AddForce(impulse, ForceMode.Impulse);
        ConsoleProDebug.LogToFilter($"NPC applied {impulse} impulse to {other.name}", "Debug");
    }
    
    private void OnTriggerExit(Collider other)
    {
        m_impulseApplied = false;
    }

    public void Shoot(Vector3 _targetPosition)
    {
        if (!m_isActive) return;
        if (!groundChecker.IsGrounded) return;

        // var direction = (_targetPosition - rb.transform.position).normalized;
        
        var impulse = CalculateImpulse(rb, _targetPosition);
        rb.AddForce(impulse, ForceMode.Impulse);
    }
    
    private Vector3 CalculateImpulse(Rigidbody _rb, Vector3 target)
    {
        var start = _rb.position;
        var toTarget = target - start;

        var mass = _rb.mass;
        var gravity = Physics.gravity;

        var horizontal = new Vector3(toTarget.x, 0f, toTarget.z);
        var horizontalDistance = horizontal.magnitude;
        var horizontalSpeedFactor = shootForceFactor + Random.Range(-5f, 5f);
        var t = horizontalDistance / horizontalSpeedFactor;
        var vHorizontal = horizontal / t;

        var y = toTarget.y;
        var vVertical = (y - 0.5f * gravity.y * t * t) / t;
        var v0 = vHorizontal + Vector3.up * vVertical;

        return mass * v0;
    }

    private void InitializeGroundChecker()
    {
        groundChecker = GetComponent<GroundChecker>();
        // groundChecker.groundCheckPosition = new Vector3(0f, -1f, 0f);
        // groundChecker.groundCheckSize = new Vector3(0.7f, 0.1f, 0.7f);
        // groundChecker.groundCheckLayerMask = LayerMask.GetMask("Ground");
    }
}
