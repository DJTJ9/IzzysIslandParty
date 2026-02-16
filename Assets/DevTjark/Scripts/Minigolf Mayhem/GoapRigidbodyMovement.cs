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
    [SerializeField] private float shootForce;
    [SerializeField] private float shootCooldown;
    [FoldoutGroup("Jump Settings", expanded: true)]
    [SerializeField] private float jumpPower;
    [SerializeField] private float jumpCooldown;
    
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
        Vector3 start = rb.position;
        Vector3 toTarget = target - start;

        float mass = rb.mass;
        Vector3 gravity = Physics.gravity;

        // --- Horizontale Bewegung ---
        Vector3 horizontal = new Vector3(toTarget.x, 0f, toTarget.z);
        float horizontalDistance = horizontal.magnitude;

        float horizontalSpeedFactor = 20f;  // Tuning
        float t = horizontalDistance / horizontalSpeedFactor;

        // Horizontal velocity
        Vector3 vHorizontal = horizontal / t;

        // --- Vertikale Bewegung ---
        float y = toTarget.y;

        float vY = (y - 0.5f * gravity.y * t * t) / t;

        Vector3 v0 = vHorizontal + Vector3.up * vY;

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
}
