using UnityEngine;
using ImprovedTimers;
using Sirenix.OdinInspector;

[RequireComponent (typeof(GroundChecker))]
public class RigidbodyMovement : MonoBehaviour
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

    private new Transform transform;
    private new Rigidbody rigidbody;
    private GroundChecker groundChecker;
    private Camera cam;

    private Vector3 moveDirection;
    private bool canMove = true;
    private bool canShoot = true;
    private bool canJump = true;
    
    private CountdownTimer pushCooldownTimer;
    private CountdownTimer shootCooldownTimer;
    private CountdownTimer jumpCooldownTimer;
    

    private void Awake()
    {
        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
        groundChecker = GetComponent<GroundChecker>();
        cam = Camera.main;
        
        pushCooldownTimer = new CountdownTimer(pushCooldown);
        pushCooldownTimer.OnTimerStop += EnableMovement;
        
        shootCooldownTimer = new CountdownTimer(shootCooldown);
        shootCooldownTimer.OnTimerStop += EnableShooting;
        
        jumpCooldownTimer = new CountdownTimer(jumpCooldown);
        jumpCooldownTimer.OnTimerStop += EnableJumping;
    }
    
    // private void FixedUpdate()
    // {
    //     // UpdateHorizontalMovement();
    //     // UpdateVerticalMovement();
    // }

    /// <summary>
    /// Recieves a move direction
    /// </summary>
    public void Move(Vector3 _direction)
    {
        if (!canMove) return;
        
        var camFwd = cam.transform.forward;   
        var camRight = cam.transform.right;

        camFwd.y = 0f; camRight.y = 0f;
        camFwd.Normalize(); camRight.Normalize();

        var worldDir = camRight * _direction.x + camFwd * _direction.z;
        
        rigidbody.AddForce(worldDir.normalized * pushForce, ForceMode.Impulse);
        
        pushCooldownTimer.Reset();
        pushCooldownTimer.Start();
        canMove = false;
    }

    public void Jump()
    {
        if (!canJump) return;
        if (!groundChecker.IsGrounded) return;
        
        rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            
        jumpCooldownTimer.Reset();
        jumpCooldownTimer.Start();
        canJump = false;
    }
    
    public void Shoot()
    {
        if (!canShoot) return;
        if (!groundChecker.IsGrounded) return;
        
        Ray ray = cam.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
        );

        Vector3 targetPoint = ray.GetPoint(500f);

        Vector3 direction = (targetPoint - rigidbody.transform.position).normalized;

        rigidbody.AddForce(direction * shootForce, ForceMode.Impulse);
        
        shootCooldownTimer.Reset();
        shootCooldownTimer.Start();
        canShoot = false;
    }


    private void EnableMovement()
    {
        canMove = true;
    }
    
    private void EnableShooting()
    {
        canShoot = true;
    }

    private void EnableJumping()
    {
        canJump = true;
    }
    
    // /// <summary>
    // /// Collects the current Velocity of the rigidbody and sets the speed
    // /// Transforms moving direction from local space to world space
    // /// Collects the speed difference to target velocity and clamps the max velocity
    // /// Sets force mode to VelocityChange
    // /// </summary>
    // private void UpdateHorizontalMovement()
    // {
    //     Vector3 currentVelocity = rigidbody.linearVelocity;
    //     Vector3 targetVelocity = new Vector3(moveDirection.x, 0f , moveDirection.z);
    //     targetVelocity *= pushForce;
    //
    //     targetVelocity = transform.TransformDirection(targetVelocity);
    //
    //     Vector3 velocityChange = targetVelocity - currentVelocity;
    //     velocityChange = new Vector3(velocityChange.x, 0f, velocityChange.z);
    //     velocityChange = Vector3.ClampMagnitude(velocityChange, maxSpeed);
    //
    //     rigidbody.AddForce(velocityChange, ForceMode.Force);
    // }
    //
    // /// <summary>
    // /// Recieves the current rotation
    // /// Sets the rotation to a target rotation
    // /// </summary>
    // public void RotateHorizontal(float _rotation)
    // {
    //     var currentRotation = rigidbody.rotation.eulerAngles;
    //     var targetRotation = currentRotation + new Vector3(0f, _rotation, 0f);
    //     rigidbody.rotation = Quaternion.Euler(targetRotation);
    // }
    //
    // /// <summary>
    // /// Modifies jump and fall speed
    // /// </summary>
    // private void UpdateVerticalMovement()
    // {
    //     if (rigidbody.linearVelocity.y < 0)
    //         rigidbody.linearVelocity += Vector3.up * (Physics.gravity.y * (fallSpeedModifier - 1) * Time.fixedDeltaTime);
    //
    //     if (rigidbody.linearVelocity.y > 0)
    //         rigidbody.linearVelocity += Vector3.up * (Physics.gravity.y * jumpSpeedModifier * Time.fixedDeltaTime);
    // }
}
