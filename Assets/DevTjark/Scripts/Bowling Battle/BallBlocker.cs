using UnityEngine;

public class BallBlocker : MonoBehaviour
{
    [SerializeField] private float returnForce = 30f;
    [SerializeField] private Transform returnPoint;

    /// <summary>
    /// Detects when a collider enters the trigger zone.
    /// Stops the object's movement and adds a force to push it towards the return point
    /// with a small randomized offset.
    /// </summary>
    /// <param name="_other">The collider entering the trigger zone.</param>
    private void OnTriggerEnter(Collider _other)
    {
        var rb = _other.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.AddForce((returnPoint.position + new Vector3(Random.Range(-1,1), Random.Range(-1,1), Random.Range(-1,1)) - transform.position).normalized * returnForce, ForceMode.Impulse);
    }

}