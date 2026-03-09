using Sirenix.OdinInspector;
using UnityEngine;

public class BallBlocker : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: true)]
    [SerializeField] private float m_returnForce = 30f;
    [SerializeField] private float m_randomRangeX = 5f;
    [SerializeField] private float m_randomRangeY = 5f;
    
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
        rb.AddForce((returnPoint.position + new Vector3(Random.Range(-m_randomRangeX,m_randomRangeX), Random.Range(-m_randomRangeY,m_randomRangeY), 0f) - transform.position).normalized * m_returnForce, ForceMode.Impulse);
    }

}