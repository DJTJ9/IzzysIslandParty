using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent (typeof(GroundChecker))]
public class GoapRigidbodyMovement : Controller, IRigidbodyMovement
{
    [FoldoutGroup("Push Settings", expanded: true)]
    [SerializeField] private float pushForce;
    [SerializeField] private float pushCooldown;
    
    [FoldoutGroup("Shoot Settings", expanded: true)]
    [SerializeField] private float shootForce;
    [SerializeField] private float shootCooldown;
    
    [FoldoutGroup("Jump Settings", expanded: true)]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    
    [FoldoutGroup("Hit Impulse Settings", expanded: true)]
    [SerializeField] private float horizontalImpactForce;
    [SerializeField] private float verticalImpactForce;
    
    // [SerializeField] private float maxSpeed;
    // [SerializeField] private float jumpSpeedModifier = 1;
    // [SerializeField] private float fallSpeedModifier = 1;

    private Rigidbody rb;
    private GroundChecker groundChecker;

    private Vector3 moveDirection;
    
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
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (other.TryGetComponent<IRigidbodyMovement>(out var _rbm)) return;
        if (_rbm.GetCurrentVelocity() > rb.linearVelocity.magnitude) return;
            
        var impactDirection = rb.linearVelocity.normalized;
        var impulse = impactDirection * (horizontalImpactForce * rb.linearVelocity.magnitude)
                      + Vector3.up * (verticalImpactForce * rb.linearVelocity.magnitude);
        
        if (!other.TryGetComponent<Rigidbody>(out var _rb)) return;
        _rb.AddForce(impulse, ForceMode.Impulse);
    }

    public void Shoot(Vector3 _targetPosition)
    {
        if (!m_isActive) return;
        if (!groundChecker.IsGrounded) return;

        var direction = (_targetPosition - rb.transform.position).normalized;
        
        var impulse = CalculateImpulse(rb, _targetPosition);
        rb.AddForce(impulse, ForceMode.Impulse);
    }
    
    private Vector3 CalculateImpulse(Rigidbody _rb, Vector3 target)
    {
        var start = _rb.position;
        var toTarget = target - start;

        var mass = _rb.mass;
        var gravity = Physics.gravity;

        // --- Horizontale Bewegung ---
        var horizontal = new Vector3(toTarget.x, 0f, toTarget.z);
        var horizontalDistance = horizontal.magnitude;

        var horizontalSpeedFactor = 20f;  // Tuning
        var t = horizontalDistance / horizontalSpeedFactor;

        // Horizontal velocity
        var vHorizontal = horizontal / t;

        // --- Vertikale Bewegung ---
        var y = toTarget.y;

        var vY = (y - 0.5f * gravity.y * t * t) / t;

        var v0 = vHorizontal + Vector3.up * vY;

        return mass * v0;
        
        // var start = _rb.position;
        // var toTarget = target - start;
        // var distance = toTarget.magnitude;
        //
        // var speedFactor = 5f;
        //
        // var t = distance / speedFactor;
        //
        // var gravity = Physics.gravity;
        //
        // var v0 = (toTarget - gravity * (0.5f * t * t)) / t;
        //
        // return _rb.mass * v0;
    }

    private void InitializeGroundChecker()
    {
        groundChecker = GetComponent<GroundChecker>();
        // groundChecker.groundCheckPosition = new Vector3(0f, -1f, 0f);
        // groundChecker.groundCheckSize = new Vector3(0.7f, 0.1f, 0.7f);
        // groundChecker.groundCheckLayerMask = LayerMask.GetMask("Ground");
    }

    public float GetCurrentVelocity()
    {
        return rb.linearVelocity.magnitude;
    }
}
