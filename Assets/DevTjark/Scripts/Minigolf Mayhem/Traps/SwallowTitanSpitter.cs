using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SwallowTitanSpitter : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: true)]
    [SerializeField] private float m_spittingForce = 100f;
    
    [SerializeField] private Transform spitTarget;
    [SerializeField] private BoxCollider triggerZone;

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
    
    public void EnableSpittingTriggerZone() => triggerZone.enabled = true;
    public void DisableSpittingTriggerZone() => triggerZone.enabled = false;
}
