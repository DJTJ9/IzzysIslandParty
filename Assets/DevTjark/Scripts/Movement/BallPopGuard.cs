using PopGuard;
using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// Suppresses unexplained upward velocity spikes of a rolling ball (ghost collisions at
/// track-piece seams). Legitimate impulses (shoot, jump, bump) must call
/// <see cref="AllowUpwardImpulse"/> before applying their force.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class BallPopGuard : MonoBehaviour
{
    [FoldoutGroup("Pop Guard Settings", expanded: true)]
    [SerializeField] private float maxUpwardDeltaPerStep = 6f;
    [SerializeField] private float legitImpulseGrace = 0.2f;
    [SerializeField] private bool logClamps = true;

    private readonly UpwardSpikeFilter filter = new UpwardSpikeFilter();
    private Rigidbody rb;
    private string lastContact = "none";

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        var velocity = rb.linearVelocity;
        var previousVy = filter.LastVy;

        if (!filter.Step(velocity.y, Time.fixedDeltaTime, maxUpwardDeltaPerStep, out var filteredVy)) return;

        if (logClamps)
            Debug.Log($"[BallPopGuard] {name}: clamped Δvy {velocity.y - previousVy:F2} m/s " +
                      $"(vy {velocity.y:F2} → {filteredVy:F2}), last contact: {lastContact}", this);

        velocity.y = filteredVy;
        rb.linearVelocity = velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        RememberContact(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        RememberContact(collision);
    }

    /// <summary>
    /// Opens the grace window so the next intended upward impulse is not clamped.
    /// </summary>
    public void AllowUpwardImpulse()
    {
        filter.AllowUpwardImpulse(legitImpulseGrace);
    }

    /// <summary>
    /// Stores parent/name of the last touched collider for the clamp log.
    /// </summary>
    /// <param name="collision">The current collision.</param>
    private void RememberContact(Collision collision)
    {
        var contactTransform = collision.collider.transform;
        lastContact = contactTransform.parent != null
            ? $"{contactTransform.parent.name}/{contactTransform.name}"
            : contactTransform.name;
    }
}
