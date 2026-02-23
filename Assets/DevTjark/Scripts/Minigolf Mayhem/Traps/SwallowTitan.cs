using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class SwallowTitan : MonoBehaviour
{
    [FoldoutGroup("Settings", expanded: true)]
    [SerializeField] private float m_accelerationForce = 100f;
    
    [SerializeField] private Transform swallowTarget;
    [SerializeField] private BoxCollider triggerZone;
    [SerializeField] private ParticleSystem windParticles;

    private void OnTriggerStay(Collider _other)
    {
        if (!_other.CompareTag("Player")) return;
        
        if (_other.TryGetComponent<Rigidbody>(out var _rb))
        {
            var swallowDirection = (swallowTarget.position - _rb.transform.position).normalized;
            
            _rb.AddForce(swallowDirection * m_accelerationForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }
    
    private void OnTriggerExit(Collider _other)
    {
        if (!_other.CompareTag("Player")) return;

        if (_other.TryGetComponent<Rigidbody>(out var _rb))
        {
            _rb.useGravity = true;
        }
    }
    
    public void EnableTriggerZone() => triggerZone.enabled = true;
    public void DisableTriggerZone() => triggerZone.enabled = false;
    
    public void PlayWindParticles() => windParticles.gameObject.SetActive(true);
    
    public void StopWindParticles() => windParticles.gameObject.SetActive(false);
    
    public void ResetWindParticles() => windParticles.Clear();
}
