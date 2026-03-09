using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SwallowTitanSpitter : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: true)]
    [SerializeField] private float m_spittingForce = 100f;
    
    [SerializeField] private Transform spitTarget;
    [SerializeField] private BoxCollider triggerZone;

    /// <summary>
    /// Detects when a player enters the trigger zone.
    /// If the player has a Rigidbody, applies a spitting force
    /// to move the player towards the spit target.
    /// </summary>
    /// <param name="_other">The collider that entered the trigger zone.</param>
    private void OnTriggerEnter(Collider _other)
    {
        if (!_other.CompareTag("Player")) return;

        if (_other.TryGetComponent<Rigidbody>(out var _rb))
        {
            var spitDirection = (spitTarget.position - _rb.transform.position).normalized;
            
            _rb.useGravity = true;
            _rb.AddForce(spitDirection * m_spittingForce, ForceMode.Impulse);
        }
    }
    
    /// <summary>
    /// Enables the spitting trigger zone to allow detection of player entry.
    /// </summary>
    public void EnableSpittingTriggerZone() => triggerZone.enabled = true;
    
    /// <summary>
    /// Disables the spitting trigger zone to prevent detection of player entry.
    /// </summary>
    public void DisableSpittingTriggerZone() => triggerZone.enabled = false;
}
