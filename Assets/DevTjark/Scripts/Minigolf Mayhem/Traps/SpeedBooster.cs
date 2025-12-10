using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpeedBooster : MonoBehaviour
{
    [FoldoutGroup("Speed Boost Settings", expanded: true)]
    [SerializeField] private float speedBoost = 5f;

    private void OnTriggerStay(Collider _other)
    {
        if (!_other.TryGetComponent<Rigidbody>(out var rb)) return;
        
        rb.AddForce(-transform.right * speedBoost, ForceMode.Acceleration);
    }
}