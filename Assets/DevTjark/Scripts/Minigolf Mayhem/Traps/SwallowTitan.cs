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

    /// <summary>
    /// Continuously applies a pulling force to the player within the trigger zone,
    /// drawing them toward the swallow target.
    /// </summary>
    /// <param name="_other">The collider within the trigger zone.</param>
    private void OnTriggerStay(Collider _other)
    {
        if (!_other.CompareTag("Player")) return;
        
        if (_other.TryGetComponent<Rigidbody>(out var _rb))
        {
            var swallowDirection = (swallowTarget.position - _rb.transform.position).normalized;
            
            _rb.AddForce(swallowDirection * m_accelerationForce * Time.fixedDeltaTime, ForceMode.Acceleration);
        }
    }
    
    /// <summary>
    /// Enables the trigger zone to detect player presence.
    /// </summary>
    public void EnableTriggerZone() => triggerZone.enabled = true;
    
    /// <summary>
    /// Disables the trigger zone to stop detecting player presence.
    /// </summary>
    public void DisableTriggerZone() => triggerZone.enabled = false;
    
    /// <summary>
    /// Activates the wind particle effect.
    /// </summary>
    public void PlayWindParticles() => windParticles.gameObject.SetActive(true);
    
    /// <summary>
    /// Deactivates the wind particle effect.
    /// </summary>
    public void StopWindParticles() => windParticles.gameObject.SetActive(false);
    
    /// <summary>
    /// Clears the wind particle effect to reset its state.
    /// </summary>
    public void ResetWindParticles() => windParticles.Clear();
}
