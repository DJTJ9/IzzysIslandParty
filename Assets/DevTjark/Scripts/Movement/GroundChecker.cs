using UnityEngine;

public class GroundChecker : MonoBehaviour {
    public bool isActive = true;

    [Header("Settings")]
    public LayerMask groundCheckLayerMask;
    public Vector3 groundCheckPosition;
    public Vector3 groundCheckSize;

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

    /// <summary>
    /// Checks if the object is grounded by detecting overlap with ground layers
    /// using a box at a defined position and size. Updates the `IsGrounded` property.
    /// </summary>
    private void CheckForGround() {
        IsGrounded = Physics.OverlapBox(transform.position + groundCheckPosition, groundCheckSize / 2, Quaternion.identity, groundCheckLayerMask).Length > 0;
    }

    /// <summary>
    /// Draws a visualization of the ground check box in the Scene View
    /// to assist with debugging ground detection behavior.
    /// </summary>
    private void OnDrawGizmosSelected() {
        transform = GetComponent<Transform>();

        Gizmos.DrawCube(transform.position + groundCheckPosition, groundCheckSize);
    }
}