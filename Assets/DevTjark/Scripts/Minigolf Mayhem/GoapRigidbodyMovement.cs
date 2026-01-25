using ImprovedTimers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent (typeof(GroundChecker))]
public class GoapRigidbodyMovement : MonoBehaviour
{
    [FormerlySerializedAs("pushForce")]
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
        if (!groundChecker.IsGrounded) return;
        
        // Ray ray = cam.ScreenPointToRay(
        //     new Vector3(Screen.width / 2f, Screen.height / 2f, 0f)
        // );
        //
        // Vector3 targetPoint = ray.GetPoint(500f);

        Vector3 direction = (_targetPosition - rb.transform.position).normalized;

        rb.AddForce(direction * shootForce, ForceMode.Impulse);
    }

    private void InitializeGroundChecker()
    {
        groundChecker = GetComponent<GroundChecker>();
        // groundChecker.groundCheckPosition = new Vector3(0f, -1f, 0f);
        // groundChecker.groundCheckSize = new Vector3(0.7f, 0.1f, 0.7f);
        // groundChecker.groundCheckLayerMask = LayerMask.GetMask("Ground");
    }
}
