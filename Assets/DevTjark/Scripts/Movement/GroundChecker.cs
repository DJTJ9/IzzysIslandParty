using UnityEngine;

public class GroundChecker : MonoBehaviour {
    public bool isActive = true;

    [Header("Settings")]
    [SerializeField] private LayerMask groundCheckLayerMask;
    [SerializeField] private Vector3 groundCheckPosition;
    [SerializeField] private Vector3 groundCheckSize;

    [field: SerializeField]
    public bool IsGrounded { get; private set; }

    private new Transform transform;

    private void Awake() {
        transform = GetComponent<Transform>();
    }

    private void Update() {
        if (isActive)
            CheckForGround();
    }

    private void CheckForGround() {
        IsGrounded = Physics.OverlapBox(transform.position + groundCheckPosition, groundCheckSize / 2, Quaternion.identity, groundCheckLayerMask).Length > 0;
    }

    private void OnDrawGizmosSelected() {
        transform = GetComponent<Transform>();

        Gizmos.DrawCube(transform.position + groundCheckPosition, groundCheckSize);
    }
}