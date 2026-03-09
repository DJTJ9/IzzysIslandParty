using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpeedBooster : MonoBehaviour
{
    [FoldoutGroup("Speed Boost Settings", expanded: true)]
    [SerializeField] private float speedBoost = 5f;

    /// <summary>
    /// Continuously applies a speed boost force to rigidbodies within the trigger zone,
    /// pushing them in the opposite direction of the transform's right vector.
    /// </summary>
    /// <param name="_other">The collider interacting with the trigger zone.</param>
    private void OnTriggerStay(Collider _other)
    {
        if (!_other.TryGetComponent<Rigidbody>(out var rb)) return;
        
        rb.AddForce(-transform.right * speedBoost, ForceMode.Acceleration);
    }
}